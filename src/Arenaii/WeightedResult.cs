using Arenaii.Data;
using Qowaiv;
using System.Globalization;

namespace Arenaii
{
	public class WeightedResult
	{
		public Bot Bot1 { get; set; }
		public Bot Bot2 { get; set; }


		public int Wins { get; set; }
		public int Draws { get; set; }
		public int Loses { get; set; }
		public int Count { get { return Wins + Draws + Loses; } }

		public Percentage Score { get { return Count == 0 ? 0.5 : (Wins + 0.5 * Draws) / Count; } }


		public override string ToString()
		{
			return string.Format
			(
				CultureInfo.InvariantCulture,
				"{0,4}+ {1,4}= {2,4}- {3,4}# {4,7} ({5}-{6})",
				Wins, Draws, Loses, Count,

				Score.ToString("0.00%"),

				Bot1.FullName,
				Bot2.FullName
			);
		}
	}
}
