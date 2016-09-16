using Arenaii.Backgammon.Data;
using Arenaii.Data;
using Arenaii.Platform;
using System;
using System.Text;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace Arenaii.Backgammon
{
	public class BackgammonEngine : IEngine<BackgammonCompetition, BackgammonSettings>
	{
		public BackgammonEngine()
		{
			Rnd = new MT19937Generator();
		}
		public IGenerator Rnd { get; set; }

		public Match Simulate(Pairing pairing, BackgammonCompetition competition)
		{
			var settings = competition.Settings;
			var timeMax = TimeSpan.FromMilliseconds(settings.TimeBank);

			using (var bot1 = ConsoleBot.Create(pairing.Bot1))
			{
				using (var bot2 = ConsoleBot.Create(pairing.Bot2))
				{
					var gen = new StringBuilder()
						.AppendFormat("settings timebank {0}", settings.TimeBank).AppendLine()
						.AppendFormat("settings time_per_move {0}", settings.TimePerMove).AppendLine()
						.AppendFormat("settings seed {0}", Rnd.Next());

					bot1.Write(gen.ToString());
					bot2.Write(gen.ToString());

					bot1.Write("settings bod_id 1");
					bot2.Write("settings bot_id 2");

					var board = new BackgammonBoard();

					var time1 = timeMax;
					var time2 = timeMax;

					int dice0 = Rnd.Next(1, 7);
					int dice1 = Rnd.Next(1, 7);

					// No doubles for the first move.
					while(dice0 == dice1)
					{
						dice0 = Rnd.Next(1, 7);
						dice1 = Rnd.Next(1, 7);
					}

					var xToMove = true;
					var turn = 0;
					while (board.NotFinished)
					{
						if (xToMove) { turn++; }
						board.Turn = turn;
						board.XToMove = xToMove;
						var bot = xToMove ? bot1 : bot2;
						var time = xToMove ? time1 : time2;
						var start = bot.Elapsed;

						bot.Write(board.GetGameUpdate());
						bot.Write("action move {0},{1} {2:0}", dice0, dice1, time.TotalMilliseconds);
						bot.Start();
						var move = bot.Read(time);
						bot.Stop();

						Console.Clear();
						board.ToConsole();
						Console.WriteLine(@"{0:ss\:ff} {1:0000} {2}", bot1.Elapsed, bot1.Bot.Elo, bot1.Bot.FullName);
						Console.WriteLine(@"{0:ss\:ff} {1:0000} {2}", bot2.Elapsed, bot2.Bot.Elo, bot2.Bot.FullName);
						
						if (bot.TimedOut || !board.Apply(move, dice0, dice1))
						{
							board.Loses(xToMove);
						}
						else
						{
							var elapsed = bot.Elapsed - start;
							var tm = time - elapsed + TimeSpan.FromMilliseconds(settings.TimePerMove);
							if (xToMove)
							{
								time1 = tm;
							}
							else
							{
								time2 = tm;
							}
						}
						dice0 = Rnd.Next(1, 7);
						dice1 = Rnd.Next(1, 7);
						xToMove = !xToMove;
					}
					
					var score = board.XIsWinner ? 1 : 0;
					return new Match(pairing.Bot1, pairing.Bot2, score)
					{
						Duration1 = bot1.Elapsed,
						Duration2 = bot2.Elapsed,
					};
				}
			}
		}
	}
}
