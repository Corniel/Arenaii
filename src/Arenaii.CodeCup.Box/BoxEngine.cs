using Arenaii.CodeCup.Box.Data;
using Arenaii.Platform;
using System.Collections.Generic;

namespace Arenaii.CodeCup.Box;

public sealed class BoxEngine : IEngine<BoxCompetition, BoxSettings>
{
    public Random Rnd { get; } = new MathNet.Numerics.Random.MersenneTwister();

    public Match Simulate(Pairing pairing, BoxCompetition competition)
    {
        var settings = competition.Settings;
        var timeMax = settings.TimeLimit;
        var colors = Rnd.NextColors();

        using var bot1 = ConsoleBot.Create(pairing.Bot1);
        using var bot2 = ConsoleBot.Create(pairing.Bot2);

        var name1 = DisplayName(bot1.Bot);
        var name2 = DisplayName(bot2.Bot);

        Render.Board(name1, name2, colors);
        Render.Rankings(competition.RankingBots);

        var bot = bot1;

        var start = new Move(Point.Parse("Hh"), Rnd.NextTile(), true);
        var turn = 0;

        var board = Board.Empty.Move(start);
        Render.Move(start, board, colors, bot1, bot2);

        var response = Move.Start;

        bot1.Write(((int)colors.One).ToString());
        bot1.Write(start.ToString());

        bot2.Write(((int)colors.Two).ToString());
        bot2.Write(start.ToString());

        while (MouveCount(board) is { } count && count != 0)
        {
            turn++;
            competition.Turns[turn]++;
            competition.Options[turn] += count;
            competition.Min[turn] = Math.Min(competition.Min[turn], count);
            competition.Max[turn] = Math.Max(competition.Max[turn], count);

            var tile = Rnd.NextTile();

            bot.Write(response.ToString());
            bot.Write(tile.ToString());

            var read = bot.Read(timeMax);

            // Invalid move / timeout.
            if (read is not { Length: 3 })
            {
                var isBot1 = bot == bot1;

                return new Match(pairing.Bot1, pairing.Bot2, isBot1 ? 0 : 1)
                {
                    Duration1 = bot1.Elapsed,
                    Duration2 = bot2.Elapsed,
                };
            }

            response = Move.Parse($"{read[..2]}{tile}{read[2..]}");

            board = board.Move(response);
            Render.Move(response, board, colors, bot1, bot2);

            bot = bot == bot1 ? bot2 : bot1;
        }

        competition.Turns[turn + 1]++;

        bot1.Write(Move.Quit.ToString());
        bot2.Write(Move.Quit.ToString());

        var scores = Scores.Get(board.ToArray());

        var sco1 = scores[colors.One];
        var sco2 = scores[colors.Two];

        var score = 0.5f;
        if (sco1 > sco2) { score = 1; }
        if (sco2 > sco1) { score = 0; }

        return new Match(pairing.Bot1, pairing.Bot2, score)
        {
            Duration1 = bot1.Elapsed,
            Duration2 = bot2.Elapsed,
        };
    }

    [Pure]
    private static int MouveCount(Board board)
    {
        var ver = new MoveGenerator.Vertical(board);
        var hor = new MoveGenerator.Horizontal(board);
        var count = 0;
        while (ver.MoveNext())
        {
            count++;
        }
        while (hor.MoveNext())
        {
            count++;
        }
        return count;
    }

    [Pure]
    private static string DisplayName(Bot bot)
        => bot.Version is { Length: > 0 }
            ? $"{bot.Name} v{bot.Version} ({bot.Elo:0000})"
            : $"{bot.Name}  ({bot.Elo:0000})";

    private static class Render
    {
        public static void Rankings(IEnumerable<Bot> bots)
        {
            var pos = 1;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            var ranking = bots.Where(b => b.IsActive).OrderByDescending(b => b.Elo).ToArray();
            var names = ranking.Select(b => DisplayName(b)[..^7]).ToArray();
            var max = names.Max(n => n.Length);

            foreach (var bot in ranking)
            {
                Console.CursorLeft = 45;
                Console.CursorTop = pos;

                Console.Write($"{pos,2}. ");
                Console.Write(names[pos - 1]);
                Console.CursorLeft = 42 + 10 + max;
                Console.Write($"{bot.Elo,4:0}");
                pos++;
            }
        }

        public static void Board(string bot1, string bot2, Colors colors)
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            Console.Write(' ');
            for (var col = 'a'; col <= 't'; col++) Console.Write($" {col}");
            Console.WriteLine();
            for (var row = 'A'; row <= 'P'; row++)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine();
            Display(bot1, colors.One);
            Console.Write(" - ");
            Display(bot2, colors.Two);
            Console.WriteLine();

            static void Display(string bot, Color color)
            {
                Console.BackgroundColor = Color(color);
                Console.Write("  ");
                Console.BackgroundColor = ConsoleColor.Black;

                Console.Write($" {bot}");
            }
        }

        public static void Move(Move move, Board board, Colors colors, ConsoleBot bot1, ConsoleBot bot2)
        {
            Console.CursorLeft = move.Point.Col * 2 + 1;
            Console.CursorTop = move.Point.Row + 1;

            Console.ForegroundColor = ConsoleColor.Black;

            if (move.Horizontal)
            {

                for (var i = 0; i < 6; i++)
                {
                    Pixel(move, i);
                }
                Console.CursorTop++;
                Console.CursorLeft -= 12;

                for (var i = 5; i >= 0; i--)
                {
                    Pixel(move, i);
                }
            }
            else
            {
                for (var i = 5; i >= 0; i--)
                {
                    Pixel(move, i);
                    Console.CursorTop++;
                    Console.CursorLeft -= 2;
                }
                Console.CursorTop -= 6;
                Console.CursorLeft += 2;

                for (var i = 0; i < 6; i++)
                {
                    Pixel(move, i);
                    Console.CursorTop++;
                    Console.CursorLeft -= 2;
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;


            Console.CursorLeft = 0;
            Console.CursorTop = 19;

            
            var score = Scores.Get(board.ToArray());

            Console.BackgroundColor = Color(colors.One);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{score[colors.One],2}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Write($" {bot1.Elapsed.TotalSeconds:00.00}s".Replace(',', '.'));

            Console.CursorLeft = DisplayName(bot1.Bot).Length + 6;
            Console.BackgroundColor = Color(colors.Two);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{score[colors.Two],2}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Write($" {bot2.Elapsed.TotalSeconds:00.00}s".Replace(',', '.'));

            static void Pixel(Move move, int i)
            {
                var color = move.Tile[i];
                Console.BackgroundColor = Color(color);
                Console.Write(' ');
                Console.Write((int)color);
            }
        }

        private static ConsoleColor Color(Color color) => color switch
        {
            Box.Color.Red => ConsoleColor.Red,
            Box.Color.Yellow => ConsoleColor.Yellow,
            Box.Color.Green => ConsoleColor.Green,
            Box.Color.Cyan => ConsoleColor.Cyan,
            Box.Color.Blue => ConsoleColor.Blue,
            Box.Color.Purple => ConsoleColor.Magenta,
            _ => ConsoleColor.Black,
        };
    }
}
