using Arenaii.Data;
using NUnit.Framework;
using Qowaiv.Statistics;
using System.IO;

namespace Arenaii.UnitTests.Domain;

public class AIClientNameTest
	{
		[Test]
		public void Create_ArenaiiAssembly_ArenaiiAIcompetionrunner2()
		{
			var act = Bot.Create(new FileInfo( typeof(Pairing).Assembly.Location));

			var name = "Arenaii";
			var version = "2";
			var elo = (Elo)1600;

			Assert.AreEqual(elo, act.Rating, "Rating");
			Assert.AreEqual(name, act.Name, "Name");
			Assert.AreEqual(version, act.Version, "Version");
		}
	}
