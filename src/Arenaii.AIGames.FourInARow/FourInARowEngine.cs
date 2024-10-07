using Arenaii.AIGames.FourInARow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arenaii.Data;
using Arenaii.Platform;
using Arenaii.AIGames.Data;
using Arenaii.AIGames;

namespace Arenaii.AIGames.FourInARow
{
	public class FourInARowEngine: AIGamesEngine<FourInARowCompetition, FourInARowSettings>
	{
		public override Match Simulate(Pairing pairing, FourInARowCompetition competition)
		{
			var settings = competition.Settings;
			var timeMax = TimeSpan.FromMilliseconds(settings.TimeBank);

			using (var bot1 = ConsoleBot.Create(pairing.Bot1))
			{
				using (var bot2 = ConsoleBot.Create(pairing.Bot2))
				{
					WriteSettings(bot1, PlayerName.player1, settings);
					WriteSettings(bot2, PlayerName.player2, settings);

					var time1 = timeMax;
					var time2 = timeMax;
					var state = BoardState.None;

					var board = new FourInARowBoard(settings.Columns, settings.Rows);
					while (state == BoardState.None)
					{
						var redToMove = board.RedToMove;
						var bot = redToMove ? bot1 : bot2;
						var time = redToMove ? time1 : time2;
						var start = bot.Elapsed;

						bot.Write(board.GetGameUpdate());
						bot.Write("action move {0:0}", time.TotalMilliseconds);
						bot.Start();
						var move = bot.Read(time);
						bot.Stop();

						Console.Clear();
						board.ToConsole();
						Console.WriteLine(@"{0:ss\:ff} {1:0000} {2}", bot1.Elapsed, bot1.Bot.Elo, bot1.Bot.FullName);
						Console.WriteLine(@"{0:ss\:ff} {1:0000} {2}", bot2.Elapsed, bot2.Bot.Elo, bot2.Bot.FullName);

						if (bot.TimedOut || !board.Apply(move))
						{
							state = redToMove ? BoardState.Player2 : BoardState.Player1;
						}
						else
						{
							state = board.State;
							var elapsed = bot.Elapsed - start;
							var tm = time - elapsed + TimeSpan.FromMilliseconds(settings.TimePerMove);

							if (tm > timeMax)
							{
								tm = timeMax;
							}
							if (redToMove)
							{
								time1 = tm;
							}
							else
							{
								time2 = tm;
							}
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
						Duration1 = bot1.Elapsed,
						Duration2 = bot2.Elapsed,
					};
				}
			}
		}

		protected override void WriteSettingsCustom(ConsoleBot bot, PlayerName player, FourInARowSettings setting)
		{
			bot.Write("settings field_columns {0}", setting.Columns);
			bot.Write("settings field_rows {0}", setting.Rows);
		}
	}
}
