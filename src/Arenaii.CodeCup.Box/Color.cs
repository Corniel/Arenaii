using System.Collections.Generic;

namespace Arenaii.CodeCup.Box;

public enum Color
{
    None = 0,
    Red = 1,
    Yellow = 2,
    Green = 3,
    Cyan = 4,
    Blue = 5,
    Purple = 6,
}

public readonly record struct Colors(Color One, Color Two)
{
    public static readonly IReadOnlyList<Color> All = [Color.Red, Color.Yellow, Color.Green, Color.Cyan, Color.Blue, Color.Purple];
}
