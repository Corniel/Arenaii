using Arenaii.AIGames.Data;
using System;

namespace Arenaii.AIGames.LightRiders.Data
{
	[Serializable]
	public class LightRidersSettings : AIGamesSettings
	{
		public LightRidersSettings()
		{
			IsSymetric = true;
			PlayerNames = new PlayerName[] { PlayerName.player0, PlayerName.player1 };
		}
	}
}
