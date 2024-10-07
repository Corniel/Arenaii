using Arenaii.AIGames;
using Arenaii.Data;
using Arenaii.Platform;
using Arenaii.RiddlesIo.Golad.Data;
using Arenaii.RiddlesIo.Golad.Model;
using Arenaii.RiddlesIo.Golad.Moves;
using System;
using System.Text;

namespace Arenaii.RiddlesIo.Golad;

public class GoladEngine : AIGamesEngine<GoladCompetition, GoladSettings>
{
    public override Match Simulate(Pairing pairing, GoladCompetition competition)
    {
        var settings = competition.Settings;
        var timeMax = TimeSpan.FromMilliseconds(settings.TimeBank);

        using var bot0 = ConsoleBot.Create(pairing.Bot1);
        using var bot1 = ConsoleBot.Create(pairing.Bot2);
        var gen = new StringBuilder()
            .AppendLine($"settings timebank {settings.TimeBank}")
            .AppendLine($"settings time_per_move {settings.TimePerMove}")
            .AppendLine($"settings player_names player0,player1")
            .AppendLine($"settings field_width {settings.Width}")
            .AppendLine($"settings field_height {settings.Height}")
            .AppendLine($"settings max_rounds {settings.MaximumRounds}");

        bot0.Write(gen.ToString());
        bot1.Write(gen.ToString());

        bot0.Write("settings your_bot player0\r\nsettings your_botid 0\r\n");
        bot1.Write("settings your_bot player1\r\nsettings your_botid 1\r\n");

        var cells = Cells.Generate(settings, Rnd);

        var time0 = timeMax;
        var time1 = timeMax;
        var state = Player.None;
        var last = "";

        while (state == Player.None)
        {
            var p0ToMove = cells.P0ToMove;
            var bot = p0ToMove ? bot0 : bot1;
            var time = p0ToMove ? time0 : time1;
            var start = bot.Elapsed;
            var move = "";

            if (!bot.TimedOut)
            {
                bot.Write(cells.GetGameUpdate());
                bot.Write("action move {0:0}", time.TotalMilliseconds);
                bot.Start();
                move = bot.Read(time);
                bot.Stop();
            }

            UpdateConsole(settings, bot0, bot1, cells, last, p0ToMove);

            if (bot.TimedOut || !cells.Apply(Move.Parse(move, cells)))
            {
                cells.Apply(Move.Pass);
                state = cells.Outcome(settings);
                last = move + "*";
            }
            else
            {
                last = move;
                state = cells.Outcome(settings);
                var elapsed = bot.Elapsed - start;
                var tm = time - elapsed + TimeSpan.FromMilliseconds(settings.TimePerMove);

                if (tm > timeMax)
                {
                    tm = timeMax;
                }
                if (p0ToMove)
                {
                    time0 = tm;
                }
                else
                {
                    time1 = tm;
                }
            }
        }

        var score = 0.5F;

        switch (state)
        {
            case Player.Player0: score = 1; break;
            case Player.Player1: score = 0; break;
        }
        return new Match(pairing.Bot1, pairing.Bot2, score)
        {
            Duration1 = bot0.Elapsed,
            Duration2 = bot1.Elapsed,
        };
    }

    private static void UpdateConsole(GoladSettings settings, ConsoleBot bot0, ConsoleBot bot1, Cells cells, string last, bool p0ToMove)
    {
        Console.Clear();
        cells.ToConsole(settings.VisualizeNext);
        Console.BackgroundColor = p0ToMove ? ConsoleColor.Blue : ConsoleColor.Red;
        Console.Write(" ");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(" ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(last);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine();
        Console.WriteLine(@"{0:ss\:ff}{3} {1:0000} {2}", bot0.Elapsed, bot0.Bot.Elo, bot0.Bot.FullName, bot0.TimedOut ? "*" : " ");
        Console.WriteLine(@"{0:ss\:ff}{3} {1:0000} {2}", bot1.Elapsed, bot1.Bot.Elo, bot1.Bot.FullName, bot1.TimedOut ? "*" : " ");
    }
}
