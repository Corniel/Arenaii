using Arenaii.AIGames.LightRiders.Data;
using Arenaii.Data;
using Arenaii.Platform;
using System;
using System.Text;

namespace Arenaii.AIGames.LightRiders
{
    public class LightRidersEngine: AIGamesEngine<LightRidersCompetition, LightRidersSettings>
    {
        public override Match Simulate(Pairing pairing, LightRidersCompetition competition)
        {
            var settings = competition.Settings;
            var timeMax = TimeSpan.FromMilliseconds(settings.TimeBank);
			var random = new Random();

            using (var bot0 = ConsoleBot.Create(pairing.Bot1))
            {
                using (var bot1 = ConsoleBot.Create(pairing.Bot2))
                {
                    var gen = new StringBuilder()
                        .AppendFormat("settings timebank {0}", settings.TimeBank).AppendLine()
                        .AppendFormat("settings time_per_move {0}", settings.TimePerMove).AppendLine()
                        .AppendLine("settings player_names player0,player1");

                    bot0.Write(gen.ToString());
                    bot1.Write(gen.ToString());

                    bot0.Write("settings your_bot player0\r\nsettings your_botid 0\r\n");
                    bot1.Write("settings your_bot player1\r\nsettings your_botid 1\r\n");

                    var board = new Board(random);

                    var time0 = timeMax;
                    var time1 = timeMax;
                    var state = BoardState.None;

                    while (state == BoardState.None)
                    {
                        var start0 = bot0.Elapsed;
                        var start1 = bot1.Elapsed;

                        var move0 = GetMove(board, bot0, time0);
                        var move1 = GetMove(board, bot1, time1);
                        
                        Console.Clear();
                        board.ToConsole();
                        Console.WriteLine(@"{0:ss\:ff} {1:0000} {2}", bot0.Elapsed, bot0.Bot.Elo, bot0.Bot.FullName);
                        Console.WriteLine(@"{0:ss\:ff} {1:0000} {2}", bot1.Elapsed, bot1.Bot.Elo, bot1.Bot.FullName);

                        if(bot0.TimedOut || bot1.TimedOut)
                        {
                            if(bot0.TimedOut)
                            {
                                state = bot1.TimedOut ? BoardState.Draw : BoardState.Player2;
                            }
                            else
                            {
                                state = BoardState.Player1;
                            }
                        }
                        else
                        {
                            state = board.Apply(move0, move1);
                            time0 = ApplyTime(settings, bot0, time0, start0);
                            time1 = ApplyTime(settings, bot1, time1, start1);
                        }
                    }

                    var score = 0.5F;

                    switch (state)
                    {
                        case BoardState.Player1: score = 1; break;
                        case BoardState.Player2: score = 0; break;
                    }
                    return new Match(pairing.Bot1, pairing.Bot2, score)
                    {
                        Duration1 = bot0.Elapsed,
                        Duration2 = bot1.Elapsed,
                    };
                }
            }
        }
        private static string GetMove(Board board, ConsoleBot bot,  TimeSpan time)
        {
            bot.Write(board.GetGameUpdate());
            bot.Write("action move {0:0}", time.TotalMilliseconds);
            bot.Start();
            var move = bot.Read(time);
            bot.Stop();
            return move;
        }
        private static TimeSpan ApplyTime(LightRidersSettings settings, ConsoleBot bot, TimeSpan time, TimeSpan start)
        {
            var timeMax = TimeSpan.FromMilliseconds(settings.TimeBank);
            var elapsed = bot.Elapsed - start;
            var tm = time - elapsed + TimeSpan.FromMilliseconds(settings.TimePerMove);
            if (tm > timeMax)
            {
                tm = timeMax;
            }
            return tm;
        }
    }
}
