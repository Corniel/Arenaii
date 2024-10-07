using Arenaii.Data;
using System;

namespace Arenaii.Backgammon.Data;

	[Serializable]
	public class BackgammonSettings : Settings
	{
		public BackgammonSettings()
		{
			TimeBank = 1000;
			TimePerMove = 500;
		}
		public int TimeBank { get; set; }
		public int TimePerMove { get; set; }
	}
