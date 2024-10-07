using Arenaii.Data;
using Qowaiv.Statistics;
using System.IO;

namespace Arenaii.UnitTests.Domain;

public class AIClientNameTest
{
    [Test]
    public void Create_ArenaiiAssembly_ArenaiiAIcompetionrunner2()
    {
        var act = Bot.Create(new FileInfo(typeof(Pairing).Assembly.Location));

        act.Should().BeEquivalentTo(new
        {
            Name = "Arenaii",
            Version = "1",
            Rating = (Elo)1600,
        });
    }
}
