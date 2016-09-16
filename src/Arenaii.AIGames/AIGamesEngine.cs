using Arenaii.AIGames.Data;
using Arenaii.Data;
using System;
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

		public virtual Match Simulate(Pairing pairing, TCompetition competition)
		{
			throw new NotImplementedException();
		}

		public IGenerator Rnd { get; set; }
	}
}
