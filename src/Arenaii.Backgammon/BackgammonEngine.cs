using Arenaii.Backgammon.Data;
using Arenaii.Data;
using System;
using Troschuetz.Random;

namespace Arenaii.Backgammon
{
	public class BackgammonEngine : IEngine<BackgammonCompetition, BackgammonSettings>
	{
		public IGenerator Rnd { get; set; }

		public Match Simulate(Pairing pairing, BackgammonCompetition competition)
		{
			throw new NotImplementedException();
		}
	}
}
