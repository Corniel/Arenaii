using Arenaii.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arenaii.AIGames.Data
{
	[Serializable]
	public abstract class AIGamesSettings : Settings
	{
		protected AIGamesSettings()
		{
			TimeBank = 10000;
			TimePerMove = 500;
			PlayerNames = new PlayerName[] { PlayerName.player1, PlayerName.player2 };
		}
		public int TimeBank { get; set; }
		public int TimePerMove { get; set; }
		public PlayerName[] PlayerNames { get; set; }
	}
}
