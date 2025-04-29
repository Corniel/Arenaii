using Arenaii.CodeCup.Box.Data;

namespace Arenaii.CodeCup.Box;

public sealed class Program : Simulator<BoxCompetition, BoxSettings>
{
    public Program() => Engine = new BoxEngine();

    static void Main(string[] args)
    {
        var program = new Program();
        program.Run(args);
    }
}
