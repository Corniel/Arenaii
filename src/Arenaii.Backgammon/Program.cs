using Arenaii.Backgammon.Data;

namespace Arenaii.Backgammon
{
	public class Program : Simulator<BackgammonCompetition, BackgammonSettings>
	{
		public Program()
		{
			Engine = new BackgammonEngine();
		}

		static void Main(string[] args)
		{
			var program = new Program();
			program.Run(args);
		}
	}
}
