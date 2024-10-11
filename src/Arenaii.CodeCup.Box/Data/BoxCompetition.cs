namespace Arenaii.CodeCup.Box.Data;

public sealed class BoxCompetition : Competition<BoxSettings>
{
    public long[] Options { get; set; } = new long[40];

    public long[] Turns { get; set; } = new long[40];

    public long[] Min { get; set; } = [1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000];

    public long[] Max { get; set; } = new long[40];
}
