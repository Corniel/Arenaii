using Arenaii.AIGames.UltimateTicTacToe.Data;
using Arenaii.Data;
using NUnit.Framework;
using System.IO;

namespace Arenaii.UnitTests.AIGames.UltimateTicTacToe
{
	[Ignore("Manual only")]
	public class UltimateTicTacToeCompetitionTest
	{
		[Test]
		public void Remove()
		{
			var competion = UltimateTicTacToeCompetition.Load<UltimateTicTacToeCompetition>();
			for (var i = 0; i < 100; i++)
			{
				competion.RecalculateElo();
			}
			competion.Save();
		}
		[Test]
		public void Save()
		{
			var competition = new UltimateTicTacToeCompetition();
			competition.Bots.Add(Bot.Create(new FileInfo(typeof(Pairing).Assembly.Location)));
			competition.Save(new DirectoryInfo(@"c:\temp"));
		}
	}
}
