using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Arenaii.AIGames.LightRiders
{
    public class Board
    {
        private readonly FieldType[,] Fields = new FieldType[16, 16];

        public Board()
        {
            SetPlayer0(new Point(7, 03));
            SetPlayer1(new Point(7, 12));
        }

        public int Round
        {
            get
            {
                var round = 0;
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        if (Fields[x, y] > FieldType.Empty) { round++; }
                    }
                }
                return (round / 2) - 1;
            }
        }

        public int Score
        {
            get
            {
                var score = 0;
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        if /**/ (Fields[x, y].IsRed()) { score++; }
                        else if (Fields[x, y].IsGreen()) { score--; }
                    }
                }
                return score;
            }
        }


        public Point Player0 { get; private set; }
        public Point Player1 { get; private set; }

        public FieldType this[Point point]
        {
            get { return Fields[point.X, point.Y]; }
            set { Fields[point.X, point.Y] = value; }
        }


        private void SetPlayer0(Point point)
        {
            Player0 = point;
            this[point] = FieldType.Red;
        }
        private void SetPlayer1(Point point)
        {
            Player1 = point;
            this[point] = FieldType.Green;
        }

        public void ToConsole()
        {
            UpdateLayout();
            Console.WriteLine();
            Ident(); ColorMarker(ConsoleColor.White, "o----------------o");
            Console.WriteLine();
            for (var x = 0; x < 16; x++)
            {
                Ident(); ColorMarker(ConsoleColor.White, "|");
                for (var y = 0; y < 16; y++)
                {
                    if (Player0.X == x && Player0.Y == y)
                    {
                        ColorMarker(ConsoleColor.Red, "X");
                    }
                    else if (Player1.X == x && Player1.Y == y)
                    {
                        ColorMarker(ConsoleColor.Green, "X");
                    }
                    else
                    {
                        switch (Fields[x, y])
                        {
                            case FieldType.Red:
                                ColorField(ConsoleColor.Red);
                                break;

                            case FieldType.Green:
                                ColorField(ConsoleColor.Green);
                                break;

                            case FieldType.CloserToRed:
                                ColorMarker(ConsoleColor.DarkRed, "+");
                                break;

                            case FieldType.CloserToGreen:
                                ColorMarker(ConsoleColor.DarkGreen, "+");
                                break;

                            default:
                                ColorMarker(ConsoleColor.DarkGray, "·");
                                break;
                        }
                    }
                }
                ColorMarker(ConsoleColor.White, "|");
                Console.WriteLine();
            }
            Ident(); ColorMarker(ConsoleColor.White, "o----------------o");
            Console.WriteLine();
            Ident(); ColorMarker(ConsoleColor.Gray, "["); ColorMarker(ConsoleColor.White, Round.ToString("000")); ColorMarker(ConsoleColor.Gray, "]");
            var sc = Score;
            if (sc == 0)
            {
                ColorMarker(ConsoleColor.Gray, "   =0");
            }
            else if(sc > 0)
            {
                ColorMarker(ConsoleColor.Red, string.Format("{0,5}", '+' + sc.ToString()));
            }
            else
            {
                ColorMarker(ConsoleColor.Green, string.Format("{0,5}", '+' + (-sc).ToString()));
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public BoardState Apply(string move0, string move1)
        {
            var m0 = GetMove(move0);
            var m1 = GetMove(move1);

            m0 = Apply(Player0, m0, FieldType.Red);
            m1 = Apply(Player1, m1, FieldType.Green);

            if (m0 == Move.None || m1 == Move.None)
            {
                if (m0 == Move.None)
                {
                    return m1 == Move.None ? BoardState.Draw : BoardState.Player2;
                }
                return BoardState.Player1;
            }

            SetPlayer0(Player0);
            SetPlayer1(Player1);

            if (Player0 == Player1)
            {
                return BoardState.Draw;
            }
            return BoardState.None;
        }

        private Move Apply(Point point, Move move, FieldType color)
        {
            if (move == Move.None)
            {
                return Move.None;
            }
            var p = point.Move(move);

            if (!p.IsOnBoard() || this[p] > FieldType.Empty)
            {
                return Move.None;
            }

            if (color == FieldType.Red)
            {
                Player0 = p;
            }
            else
            {
                Player1 = p;
            }
            return move;
        }

        private void UpdateLayout()
        {
            MarkEmptyFieldsAsUnreachable();

            var done = new HashSet<Point>();
            done.Add(Player0);
            done.Add(Player1);

            var q0 = new Queue<Point>(new[] { Player0 });
            var q1 = new Queue<Point>(new[] { Player1 });

            while (q0.Count != 0 || q1.Count != 0)
            {
                LoopNeighbors(q0, done, FieldType.CloserToRed);
                LoopNeighbors(q1, done, FieldType.CloserToGreen);
            }
        }

        private void LoopNeighbors(Queue<Point> queue, HashSet<Point> done, FieldType marker)
        {
            var count = queue.Count;
            for (var i = 0; i < count; i++)
            {
                var point = queue.Dequeue();

                foreach (var neighbor in point.GetNeighbors())
                {
                    if (done.Contains(neighbor) || this[neighbor] > FieldType.Empty) { continue; }

                    done.Add(neighbor);
                    this[neighbor] = marker;
                    queue.Enqueue(neighbor);
                }
            }
        }

        private void MarkEmptyFieldsAsUnreachable()
        {
            for (var x = 0; x < 16; x++)
            {
                for (var y = 0; y < 16; y++)
                {
                    if (Fields[x, y] <= FieldType.Empty) { Fields[x, y] = FieldType.Unreachable; }
                }
            }
        }

        private Move GetMove(string move)
        {
            Move m;
            if (Enum.TryParse(move, out m))
            {
                return m;
            }
            return Move.None;
        }

        public string GetGameUpdate()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetRoundUpdate());
            sb.AppendLine(GetFieldUpdate());
            return sb.ToString();
        }

        public string GetFieldUpdate()
        {
            var chars = new char[256];
            var index = 0;

            for (var x = 0; x < 16; x++)
            {
                for (var y = 0; y < 16; y++)
                {
                    if (Player0.X == x && Player0.Y == y) { chars[index++] = '0'; }
                    else if (Player1.X == x && Player1.Y == y) { chars[index++] = '1'; }
                    else if (Fields[x, y] <= FieldType.Empty) { chars[index++] = '.'; }
                    else { chars[index++] = 'x'; }
                }
            }
            return "update game field " + string.Join(",", chars);
        }

        public string GetRoundUpdate()
        {
            return string.Format("update game round {0}", Round);
        }

        private static void ColorMarker(ConsoleColor color, string str)
        {
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void ColorField(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void Ident()
        {
            Console.Write("  ");
        }
    }
}
