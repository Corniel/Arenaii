using Arenaii.AIGames.UltimateTicTacToe.Data;

namespace Arenaii.AIGames.UltimateTicTacToe
{
	public class Program : Simulator<UltimateTicTacToeCompetition, UltimateTicTacToeSettings>
	{
		public Program()
		{
			Engine = new UltimateTicTacToeEngine();
		}

		static void Main(string[] args)
		{
			var program = new Program();
			program.Run(args);
		}
	}
}
