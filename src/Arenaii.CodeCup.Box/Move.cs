namespace Arenaii.CodeCup.Box;

[StructLayout(LayoutKind.Auto)]
public readonly struct Move : IEquatable<Move>
{
    public static readonly Move Start = new Move(Point.Parse("St"), default, default);
    public static readonly Move Quit = new Move(Point.Parse("Qt"), default, default);

    public Move(Point point, Tile tile, bool horizontal)
    {
        Point = point;
        Tile = tile;
        Horizontal = horizontal;
    }

    public readonly Point Point;
    public readonly Tile Tile;
    public readonly bool Horizontal;

    public string Response => Point.ToString() + (Horizontal ? 'h' : 'v');

    public override string ToString()
    {
        if (Point == Start.Point) return nameof(Start);
        if (Point == Quit.Point) return nameof(Quit);

        return Point.ToString()
            + Tile.ToString()
            + (Horizontal ? 'h' : 'v');
    }

    public override bool Equals(object obj) => obj is Move other && Equals(other);

    public bool Equals(Move other)
        => Point == other.Point
        && Tile == other.Tile
        && Horizontal == other.Horizontal;

    public override int GetHashCode()
        => Point.GetHashCode()
        ^ Tile.GetHashCode()
        ^ Horizontal.GetHashCode();

    public static bool operator ==(Move left, Move right) => left.Equals(right);

    public static bool operator !=(Move left, Move right) => !(left == right);

    public static Move Parse(string str)
    {
        if (str == nameof(Quit)) return Quit;
        if (str == nameof(Start)) return Start;

        var point = Point.Parse(str.Substring(0, 2));
        var tile = Tile.Parse(str.Substring(2, 6));
        var horizontal = str.EndsWith("h");

        return new Move(point, tile, horizontal);
    }
}
