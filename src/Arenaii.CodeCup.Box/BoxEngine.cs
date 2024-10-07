using Arenaii.CodeCup.Box.Data;
using Arenaii.Platform;

namespace Arenaii.CodeCup.Box;

public sealed class BoxEngine : IEngine<BoxCompetition, BoxSettings>
{
    public Random Rnd { get; } = new MathNet.Numerics.Random.MersenneTwister();

    public Match Simulate(Pairing pairing, BoxCompetition competition)
    {
        var settings = competition.Settings;
        var timeMax = settings.TimeLimit;

        using var bot1 = ConsoleBot.Create(pairing.Bot1);
        using var bot2 = ConsoleBot.Create(pairing.Bot2);

        Render.Board(bot1, bot2);

        var bot = bot1;

        var colors = Rnd.NextColors();
        var start = new Move(Point.Parse("Hh"), Rnd.NextTile(), true);

        var board = Board.Empty.Move(start);
        Render.Move(start, board, colors);

        var response = Move.Start;

        bot1.Write(((int)colors.One).ToString());
        bot1.Write(start.ToString());

        bot2.Write(((int)colors.Two).ToString());
        bot2.Write(start.ToString());

        while (HasMoves(board))
        {
            var tile = Rnd.NextTile();

            bot.Write(response.ToString());
            bot.Write(tile.ToString());

            var read = bot.Read(timeMax);

            response = Move.Parse($"{read[..2]}{tile}{read[2..]}");

            board = board.Move(response);
            Render.Move(response, board, colors);

            bot = bot == bot1 ? bot2 : bot1;
        }

        bot1.Write(Move.Quit.ToString());
        bot2.Write(Move.Quit.ToString());

        var scores = Scores.Get(board.ToArray());

        var sco1 = scores[colors.One];
        var sco2 = scores[colors.Two];

        var score = 0.5f;
        if (sco1 > sco2) { score = 1; }
        if (sco2 > sco1) { score = '0'; }

        return new Match(pairing.Bot1, pairing.Bot2, score)
        {
            Duration1 = bot1.Elapsed,
            Duration2 = bot2.Elapsed,
        };
    }

    [Pure]
    private static bool HasMoves(Board board)
    {
        var ver = new MoveGenerator.Vertical(board);
        var hor = new MoveGenerator.Horizontal(board);
        return ver.MoveNext() || hor.MoveNext();
    }

    

    private static class Render
    {
        public static void Board(ConsoleBot bot1, ConsoleBot bot2)
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            Console.Write(' ');
            for (var col = 'a'; col <= 't'; col++) Console.Write(col);
            Console.WriteLine();
            for (var row = 'A'; row <= 'P'; row++)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine();
            Console.WriteLine($"{bot1.Bot.Name} ({bot1.Bot.Elo:0000}) - {bot2.Bot.Name} ({bot2.Bot.Elo:0000})");
        }

        public static void Move(Move move, Board board, Colors colors)
        {
            Console.CursorLeft = move.Point.Col + 1;
            Console.CursorTop = move.Point.Row + 1;

            Console.ForegroundColor = ConsoleColor.Black;

            if (move.Horizontal)
            {

                for (var i = 0; i < 6; i++)
                {
                    Pixel(move, i);
                }
                Console.CursorTop++;
                Console.CursorLeft -= 6;

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
                    Console.CursorLeft--;
                }
                Console.CursorTop -= 6;
                Console.CursorLeft++;

                for (var i = 0; i < 6; i++)
                {
                    Pixel(move, i);
                    Console.CursorTop++;
                    Console.CursorLeft--;
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;


            Console.CursorLeft = 0;
            Console.CursorTop = 20;

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            var score = Scores.Get(board.ToArray());
            Console.WriteLine($"{score[colors.One]} - {score[colors.Two]}");

            static void Pixel(Move move, int i)
            {
                var color = move.Tile[i];
                Console.BackgroundColor = Color(color);
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
