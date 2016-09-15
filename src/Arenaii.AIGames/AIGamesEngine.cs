using Arenaii.AIGames.Data;
using Arenaii.Data;
using Arenaii.Platform;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace Arenaii.AIGames
{
	public abstract class AIGamesEngine<TCompetition, TSettings>: IEngine<TCompetition, TSettings>
		where TCompetition : Competition<TSettings>
		where TSettings : AIGamesSettings
	{
		public AIGamesEngine()
		{
			Rnd = new MT19937Generator();
		}

		protected void WriteSettings(ConsoleBot bot, PlayerName player, TSettings settings)
		{
			bot.Write("settings timebank {0}", settings.TimeBank);
			bot.Write("settings time_per_move {0}", settings.TimePerMove);
			bot.Write("settings player_names player1,player2");
			bot.Write("settings your_bot {0}", player);
			bot.Write("settings your_botid {0}", (int)player);

			WriteSettingsCustom(bot, player, settings);
		}
		protected abstract void WriteSettingsCustom(ConsoleBot bot, PlayerName player, TSettings setting);

		public abstract Match Simulate(Pairing pairing, TCompetition competition);

		public IGenerator Rnd { get; set; }
	}
}
