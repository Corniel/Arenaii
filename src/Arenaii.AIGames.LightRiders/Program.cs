using Arenaii.AIGames.LightRiders.Data;

namespace Arenaii.AIGames.LightRiders;

public class Program : Simulator<LightRidersCompetition, LightRidersSettings>
{
    public Program()
    {
        Engine = new LightRidersEngine();
    }

    static void Main(string[] args)
    {
        var program = new Program();
        program.Run(args);
    }
}
