using Arenaii.AIGames.Data;
using System;

namespace Arenaii.AIGames.FourInARow.Data
{
	[Serializable]
	public class FourInARowSettings : AIGamesSettings
	{
		public FourInARowSettings()
		{
			Columns = 7;
			Rows = 6;
		}

		public int Columns { get; set; }
		public int Rows { get; set; }
	}
}
