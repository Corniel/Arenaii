using Arenaii.AIGames.FourInARow.Data;

namespace Arenaii.AIGames.FourInARow
{
	public class Program : Simulator<FourInARowCompetition, FourInARowSettings>
	{
		public Program()
		{
			Engine = new FourInARowEngine();
		}
		static void Main(string[] args)
		{
			var program = new Program();
			program.Run(args);
		}
	}
}
