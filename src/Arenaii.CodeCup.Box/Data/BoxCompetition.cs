namespace Arenaii.CodeCup.Box.Data;

public sealed class BoxCompetition : Competition<BoxSettings>
{
    public long[] Options { get; set; } = new long[40];

    public long[] Turns { get; set; } = new long[40];
}
