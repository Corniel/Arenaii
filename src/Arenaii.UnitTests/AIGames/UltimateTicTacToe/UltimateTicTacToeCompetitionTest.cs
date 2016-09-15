using Arenaii.AIGames.UltimateTicTacToe.Data;
using Arenaii.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arenaii.UnitTests.AIGames.UltimateTicTacToe
{
	[TestFixture]
	public class UltimateTicTacToeCompetitionTest
	{
		[Test]
		public void Remove_()
		{
			var competion = UltimateTicTacToeCompetition.Load<UltimateTicTacToeCompetition>();
			var bots = competion.Bots.Where(bot => bot.Version.Contains("fhf")).ToList();
			foreach (var bot in bots)
			{
				competion.Remove(bot);
			}
				for (var i = 0; i < 1000; i++)
			{
				competion.RecalculateElo();
			}
			competion.WriteResults();
			competion.Save();
		}
		[Test]
		public void Save_()
		{
			var competition = new UltimateTicTacToeCompetition();
			competition.Bots.Add(Bot.Create(new FileInfo(typeof(Pairing).Assembly.Location)));
			competition.Save(new DirectoryInfo(@"c:\temp"));
		}
	}
}
