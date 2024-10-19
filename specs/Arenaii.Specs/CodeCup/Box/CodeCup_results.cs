using Arenaii.CodeCup.Box.Data;
using System;
using System.IO;

namespace Specs.CodeCup_results;

internal class Standings
{
    [Test]
    public void Round2()
    {
        var competition = BoxCompetition.Load<BoxCompetition>(new FileInfo("../../../CodeCup/Box/Results/Round2.xml"));

        foreach(var bot in competition.Bots)
        {
            bot.IsActive = true;
        }

        for (var i = 0; i < 100; i++)
        {
            competition.RecalculateElo();
        }

        Console.WriteLine("[table border= 1 width = 350 cellpadding = 2 bordercolor =#000000]");
        Console.WriteLine("[tr][th]Pos[/th][th]Elo[/th][th]Bot[/th][/tr]");

        var pos = 1;

        foreach (var bot in competition.Bots)
        {
            Console.WriteLine(FormattableString.Invariant($"[tr][td align = right]{pos++}[/td][td align = right]{bot.Elo:0.0}[/td][td]{bot.Name}[/td][/tr]"));
        }
        Console.WriteLine("[/table]");

        competition.Bots.Should().HaveCount(38);
    }
}
