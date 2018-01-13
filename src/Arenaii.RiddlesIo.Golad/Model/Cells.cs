using Arenaii.RiddlesIo.Golad.Data;
using Arenaii.RiddlesIo.Golad.Moves;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random;

namespace Arenaii.RiddlesIo.Golad.Model
{
    public class Cells : IEnumerable<Cell>
    {
        public Cells(int width, int height)
        {
            Size = height * width;
            Height = height;
            Width = width;
            collection = new Cell[Size];
            matrix = new Cell[width, height];
            State = new byte[Size];

            InitializeCells();
            InitializeNeighbors();
        }

        public Cell this[int index] => collection[index];

        public Cell this[int x, int y] => matrix[x, y];

        private void InitializeCells()
        {
            var index = 0;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var neighbors = 8;
                    if (x == 0 || x == Width - 1)
                    {
                        neighbors -= 3;
                    }
                    if (y == 0 || y == Height - 1)
                    {
                        neighbors -= 3;
                    }
                    if (neighbors == 2)
                    {
                        neighbors = 3;
                    }
                    var cell = new Cell(this, index, x, y, neighbors);
                    collection[index++] = cell;
                    matrix[x, y] = cell;
                }
            }
        }

        private void InitializeNeighbors()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var cell = matrix[x, y];

                    var xMin = Math.Max(0, x - 1);
                    var xMax = Math.Min(Width - 1, x + 1);
                    var yMin = Math.Max(0, y - 1);
                    var yMax = Math.Min(Height - 1, y + 1);

                    var i = 0;
                    for (var x_ = xMin; x_ <= xMax; x_++)
                    {
                        for (var y_ = yMin; y_ <= yMax; y_++)
                        {
                            if (x_ == cell.X && y_ == cell.Y)
                            {
                                continue;
                            }
                            cell.Neighbors[i++] = this[x_, y_];
                        }
                    }
                }
            }
        }

        public int Size { get; }
        public int Height { get; }
        public int Width { get; }

        public int Player0 => State.Count(state => state == Player.Player0);
        public int Player1 => State.Count(state => state == Player.Player1);
        public byte Outcome(GoladSettings settings)
        {
            byte outcome = 0;

            foreach (var owner in State)
            {
                outcome |= owner;
                if (outcome == Player.Draw)
                {
                    return Round == settings.MaximumRounds ? Player.Draw : Player.None;
                }
            }
            return outcome == Player.None ? Player.Draw : outcome;
        }

        public int Ply { get; private set; }
        public int Round => Ply / 2;

        public bool P0ToMove => (Ply % 2) == 0;

        internal byte[] State { get; }

        private readonly Cell[] collection;
        private readonly Cell[,] matrix;

        public bool Alive => State.Any(state => state != Player.None);

        public bool Apply(IMove move)
        {
            if (move.Apply(this))
            {
                Ply++;
                Process();
                return true;
            }
            return false;
        }

        public void Process() => Process(new byte[Size]);
        public void Process(byte[] buffer)
        {
            for (var index = 0; index < Size; index++)
            {
                buffer[index] = collection[index].Next;
            }
            SetState(buffer);
        }

        public byte[] CopyState()
        {
            var state = new byte[State.Length];
            Buffer.BlockCopy(State, 0, state, 0, state.Length);
            return state;
        }
        public void SetState(byte[] state)
        {
            Buffer.BlockCopy(state, 0, State, 0, State.Length);
        }

        public override string ToString() => string.Join(",", this.Select(cell => ".01"[cell.Current]));

        public void ToConsole(bool visualizeNext)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var cell = this[x, y];

                    if (cell.Current == cell.Next || !visualizeNext)
                    {
                        ToStableConsoleState(cell.Current);
                    }
                    else
                    {
                        ToUnstableConsolState(cell);
                    }
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
            Console.Write($"Round: {Round,-3}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Player0);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Player1);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void ToStableConsoleState(byte owner)
        {
            if (owner == Player.None)
            {
                Console.BackgroundColor = ConsoleColor.White;
            }
            else if (owner == Player.Player0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            else if (owner == Player.Player1)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
            }
            Console.Write("  ");
        }
        private static void ToUnstableConsolState(Cell cell)
        {
            Console.BackgroundColor = ConsoleColor.White;
            var owner = cell.IsAlive ? cell.Current : cell.Next;
            Console.ForegroundColor = owner == Player.Player0 ? ConsoleColor.Red : ConsoleColor.Blue;
            if (cell.IsAlive)
            {
                Console.Write("##");
            }
            else
            {
                Console.Write("..");
            }
        }

        public string GetGameUpdate()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"update game round {Round}");
            sb.AppendLine($"update game field {ToString()}");
            sb.AppendLine($"update player0 living_cells {Player0}");
            sb.AppendLine($"update player1 living_cells {Player1}");
            return sb.ToString();
        }

        #region IEnumerator

        public IEnumerator<Cell> GetEnumerator() => ((ICollection<Cell>)collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        /// <summary>Generates a random starting position.</summary>
        public static Cells Generate(GoladSettings settings, IGenerator rnd)
        {
            var cells = new Cells(settings.Width, settings.Height);

            var todo = settings.InitialPlayerCount;
            while (todo > 0)
            {
                // Select one cell in the top half of the board.
                var index = rnd.Next(cells.Size / 2);
                var cell = cells[index];
                if (cell.IsDead)
                {
                    cells.State[index] = Player.Player0;
                    var other = cells[settings.Width - cell.X - 1, settings.Height - cell.Y - 1];
                    cells.State[other.Index] = Player.Player1;
                    todo--;
                }
            }
            return cells;
        }

        /// <summary>Parses a position.</summary>
        public static Cells Parse(GoladSettings settings, string str)
        {
            var cells = new Cells(settings.Width, settings.Height);

            var index = 0;
            foreach(var ch in str)
            {
                var value = ".01".IndexOf(ch);
                if(value != -1)
                {
                    cells.State[index++] = (byte)value;
                }
            }
            return cells;
        }
    }
}
