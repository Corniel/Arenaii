using System;
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
                        if (Fields[x, y] != FieldType.Empty) { round++; }
                    }
                }
                return (round / 2) - 1;
            }
        }


        public Point Player0 { get; private set; }
        public Point Player1 { get; private set; }

        private void SetPlayer0(Point point)
        {
            Player0 = point;
            Fields[point.X, point.Y] = FieldType.Red;
        }
        private void SetPlayer1(Point point)
        {
            Player1 = point;
            Fields[point.X, point.Y] = FieldType.Blue;
        }

        public void ToConsole()
        {
            Console.WriteLine();
            for (var x = 0; x < 16; x++)
            {
                Ident();
                for (var y = 0; y < 16; y++)
                {
                    if (Player0.X == x && Player0.Y == y)
                    {
                        ColorMarker(ConsoleColor.Red);
                    }
                    else if (Player1.X == x && Player1.Y == y)
                    {
                        ColorMarker(ConsoleColor.Blue);
                    }
                    else
                    {
                        switch (Fields[x, y])
                        {
                            case FieldType.Red:
                                ColorField(ConsoleColor.Red);
                                break;

                            case FieldType.Blue:
                                ColorField(ConsoleColor.Blue);
                                break;

                            default:
                                ColorField(ConsoleColor.Black);
                                break;
                        }
                    }
                    WriteVerticalMarker(y);
                }
                WriteHorizontalMarker(x);
            }

            Console.WriteLine();
        }

        private static void WriteHorizontalMarker(int x)
        {
            Console.WriteLine();

            if (x % 4 == 3 && x != 15)

            {
                Ident(); Console.WriteLine("----o----o----o----");
            }
        }
        private static void WriteVerticalMarker(int y)
        {
            if (y % 4 == 3 && y != 15)
            {
                Console.Write('|');
            }
        }

        public BoardState Apply(string move0, string move1)
        {
            var m0 = GetMove(move0);
            var m1 = GetMove(move1);

            m0 = Apply(Player0, m0, FieldType.Red);
            m1 = Apply(Player1, m1, FieldType.Blue);

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

            if (p.X < 0 || p.X > 15 ||
                p.Y < 0 || p.Y > 15 ||
                Fields[p.X, p.Y] != FieldType.Empty)
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
                    else if (Fields[x, y] == FieldType.Empty) { chars[index++] = '.'; }
                    else { chars[index++] = 'x'; }
                }
            }
            return "update game field " + string.Join(",", chars);
        }

        public string GetRoundUpdate()
        {
            return string.Format("update game round {0}", Round);
        }

        private static void ColorMarker(ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write("X");
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
    internal static class PointExtensions
    {
        public static Point Move(this Point p, Move move)
        {
            switch (move)
            {
                case LightRiders.Move.up: /*   */ return new Point(p.X - 1, p.Y + 0);
                case LightRiders.Move.right: /**/ return new Point(p.X + 0, p.Y + 1);
                case LightRiders.Move.down: /* */ return new Point(p.X + 1, p.Y + 0);
                case LightRiders.Move.left: /* */ return new Point(p.X + 0, p.Y - 1);
                default: return p;
            }
        }
    }
}
