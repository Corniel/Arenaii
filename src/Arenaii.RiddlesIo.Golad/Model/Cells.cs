﻿using Arenaii.RiddlesIo.Golad.Data;
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
            cells = new Cell[Size];
            matrix = new Cell[width, height];
            State = new byte[Size];

            InitializeCells();
            InitializeNeighbors();
        }

        public Cell this[int index] => cells[index];

        public Cell this[int x, int y] => matrix[x, y];

        private void InitializeCells()
        {
            var index = 0;
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
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
                    cells[index++] = cell;
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

        private readonly Cell[] cells;
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

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (x != 0 && y != 0)
                    {
                        sb.Append(',');
                    }
                    var cell = this[x, y];

                    if (cell.IsDead)
                    {
                        sb.Append('.');
                    }
                    else
                    {
                        sb.Append(cell.Owner - 1);
                    }
                }
            }
            return sb.ToString();
        }

        public void ToConsole()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var owner = this[x, y].Owner;
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

        public IEnumerator<Cell> GetEnumerator() => ((ICollection<Cell>)cells).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        public static Cells Generate(GoladSettings settings, IGenerator rnd)
        {
            var universe = new Cells(settings.Width, settings.Height);

            var todo = settings.InitialPlayerCount;
            while (todo > 0)
            {
                var index = rnd.Next(universe.Size);
                var cell = universe[index];
                if (cell.IsDead)
                {
                    universe.State[index] = Player.Player0;
                    var other = universe[settings.Width - cell.X - 1, settings.Height - cell.Y - 1];
                    universe.State[other.Index] = Player.Player1;
                    todo--;
                }
            }
            return universe;
        }
    }
}
