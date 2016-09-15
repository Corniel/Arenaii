using Arenaii.Data;

namespace Arenaii
{
	public class Pairing
	{
		public Pairing(Bot bot1, Bot bot2)
		{
			Bot1 = bot1;
			Bot2 = bot2;
		}

		public Bot Bot1 { get; set; }
		public Bot Bot2 { get; set; }
	}
}
