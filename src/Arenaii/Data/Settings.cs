using Qowaiv.Statistics;
using System;

namespace Arenaii.Data
{
	[Serializable]
	public abstract class Settings
	{
		public Settings()
		{
			AverageElo = 1600;
		}
		public bool IsSymetric { get; set; }
		public Elo AverageElo { get; set; }

		public PairingType Pairing { get; set; }
	}
}
