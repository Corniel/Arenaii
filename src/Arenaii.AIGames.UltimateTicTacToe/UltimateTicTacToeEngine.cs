using Arenaii.AIGames.UltimateTicTacToe.Data;
using Arenaii.Data;
using Arenaii.Platform;
using System;
using System.Text;

namespace Arenaii.AIGames.UltimateTicTacToe
{
	public class UltimateTicTacToeEngine : AIGamesEngine<UltimateTicTacToeCompetition, UltimateTicTacToeSettings>
	{
		public override Match Simulate(Pairing pairing, UltimateTicTacToeCompetition competition)
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
						.AppendLine("settings player_names player1,player2");

					bot1.Write(gen.ToString());
					bot2.Write(gen.ToString());

					bot1.Write("settings your_bot player1\r\nsettings your_botid 1\r\n");
					bot2.Write("settings your_bot player2\r\nsettings your_botid 2\r\n");

					var board = new MetaBoard();

					var time1 = timeMax;
					var time2 = timeMax;
					var state = BoardState.None;

					while (state == BoardState.None)
					{
						var oToMove = board.OToMove;
						var bot = oToMove ? bot1 : bot2;
						var time = oToMove ? time1 : time2;
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
							state = oToMove ? BoardState.Player2 : BoardState.Player1;
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
							if (oToMove)
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
	}
}
