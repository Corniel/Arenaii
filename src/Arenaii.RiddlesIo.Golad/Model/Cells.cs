using Arenaii.RiddlesIo.Golad.Data;
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
        public Cells(int height, int width)
        {
            Size = height * width;
            Height = height;
            Width = width;
            cells = new Cell[Size];
            State = new byte[Size];

            InitializeCells(height, width);
            InitializeNeighbors(height, width);
        }

        public Cell this[int index] => cells[index];

        public Cell this[int row, int col]
        {
            get => cells[row * Width + col];
        }

        private void InitializeCells(int rows, int cols)
        {
            var index = 0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var neighbors = 8;
                    if (row == 0 || row == rows - 1)
                    {
                        neighbors -= 3;
                    }
                    if (col == 0 || col == cols - 1)
                    {
                        neighbors -= 3;
                    }
                    if (neighbors == 2)
                    {
                        neighbors = 3;
                    }
                    cells[index] = new Cell(this, index++, row, col, neighbors);
                }
            }
        }

        private void InitializeNeighbors(int rows, int cols)
        {
            var index = 0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var cell = cells[index++];

                    var rMin = Math.Max(0, row - 1);
                    var rMax = Math.Min(rows - 1, row + 1);
                    var cMin = Math.Max(0, col - 1);
                    var cMax = Math.Min(cols - 1, col + 1);

                    var i = 0;
                    for (var r = rMin; r <= rMax; r++)
                    {
                        for (var c = cMin; c <= cMax; c++)
                        {
                            if (r == cell.Row && c == cell.Col)
                            {
                                continue;
                            }
                            cell.Neighbors[i++] = this[r, c];
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

        private Cell[] cells { get; }
        internal byte[] State { get; }

        public bool Alive => State.Any(state => state != Player.None);

        public void Kill(Cell cell) => State[cell.Index] = Player.None;

        public void Birth(byte player, Cell child, Cell father, Cell mother)
        {
            State[father.Index] = Player.None;
            State[mother.Index] = Player.None;
            State[child.Index] = player;
        }

        public bool Apply(string move)
        {
            if (string.IsNullOrEmpty(move))
            {
                return false;
            }
            if (move == "pass")
            {
                return Apply(Move.Pass);
            }

            if (move.StartsWith("kill "))
            {
                var split = move.Substring(5).Split(',');

                if (split.Length == 2 &&
                    int.TryParse(split[0], out int row) &&
                    row >= 1 && row <= Height &&
                    int.TryParse(split[1], out int col) &&
                    col >= 1 && col <= Width)
                {
                    return Apply(Move.Kill(this[row - 1, col - 1]));
                }
                return false;
            }

            if (move.StartsWith("birth "))
            {
                var parts = move.Substring(6).Split(' ');

                if (parts.Length == 3)
                {
                    var child = parts[0].Split(',');
                    var father = parts[1].Split(',');
                    var mother = parts[2].Split(',');

                    if (child.Length == 2 &&
                        father.Length == 2 &&
                        mother.Length == 2 &&

                        int.TryParse(child[0], out int child_row) &&
                        child_row >= 1 && child_row <= Height &&
                        int.TryParse(child[1], out int child_col) &&
                        child_col >= 1 && child_col <= Width &&

                        int.TryParse(father[0], out int father_row) &&
                        father_row >= 1 && father_row <= Height &&
                        int.TryParse(father[1], out int father_col) &&
                        father_col >= 1 && father_col <= Width &&

                        int.TryParse(mother[0], out int mother_row) &&
                        mother_row >= 1 && mother_row <= Height &&
                        int.TryParse(mother[1], out int mother_col) &&
                        mother_col >= 1 && mother_col <= Width)
                    {
                        return Apply(Move.Birth(
                            this[child_row - 1, child_col - 1].Index,
                            this[father_row - 1, father_col - 1].Index,
                            this[mother_row - 1, mother_col - 1].Index));
                    }
                }
            }

            return false;
        }
        public bool Apply(Move move)
        {
            // We can't kill what is dead.
            if (move.IsKill())
            {
                if (this[move.Index].IsDead)
                {
                    return false;
                }
                Kill(this[move.Index]);
            }

            if (move.IsBirth())
            {
                // We can't revoke a cell that is alive.
                if (this[move.Index].IsAlive)
                {
                    return false;
                }
                // It should be two different cells that die.
                if (move.Father == move.Mother)
                {
                    return false;
                }
                var player = P0ToMove ? Player.Player0 : Player.Player1;

                // Wrong owner (of not even alive).
                if (this[move.Mother].Owner != player || this[move.Father].Owner != player)
                {
                    return false;
                }
                Birth(player, this[move.Index], this[move.Father], this[move.Mother]);
            }
            Ply++;
            Process();

            return true;
        }

        public void Process() => Process(new byte[Size]);
        public void Process(byte[] buffer)
        {
            for (var index = 0; index < Size; index++)
            {
                buffer[index] = cells[index].Next;
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

        public override string ToString()
        {
            var sb = new StringBuilder(Size * 2);

            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    var cell = this[row, col];

                    if (cell.IsDead)
                    {
                        sb.Append('.');
                    }
                    else
                    {
                        sb.Append(cell.Owner - 1);
                    }
                    if (col != Width - 1)
                    {
                        sb.Append(',');
                    }
                }
                if (row != Height - 1)
                {
                    sb.Append(';');
                }
            }
            return sb.ToString();
        }

        public void ToConsole()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    var owner = this[row, col].Owner;
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
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
            Console.Write($"Round: {Round,-3}"   );
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Player0);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(Player1);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
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

        public IEnumerator<Cell> GetEnumerator() => ((ICollection<Cell>)cells).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static Cells Generate(GoladSettings settings, IGenerator rnd)
        {
            var universe = new Cells(settings.Height, settings.Width);

            var todo = settings.InitialPlayerCount;
            while (todo > 0)
            {
                var index = rnd.Next(universe.Size);
                var cell = universe[index];
                if (cell.IsDead)
                {
                    universe.State[index] = Player.Player0;
                    var other = universe[cell.Row, settings.Width - cell.Col - 1];
                    universe.State[other.Index] = Player.Player1;
                    todo--;
                }
            }
            return universe;
        }
    }
}
