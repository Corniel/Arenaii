namespace Arenaii.CodeCup.Box.Data;

[Serializable]
public sealed class BoxSettings : Settings
{
    public int Time { get; init; } = 30_000;

    public TimeSpan TimeLimit => TimeSpan.FromMilliseconds(Time);
}
