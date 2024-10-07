namespace Arenaii.CodeCup.Box;

[DebuggerDisplay("{ToString()}, Row = {Row}, Col = {Col}")]
[StructLayout(LayoutKind.Auto)]
public readonly struct Point : IEquatable<Point>
{
    public Point(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public readonly int Row;
    public readonly int Col;

    public override string ToString() => "" + (char)(Row + 'A') + (char)(Col + 'a');

    public override bool Equals(object obj)=> obj is Point p && Equals(p);

    public bool Equals(Point other)=> Row == other.Row && Col == other.Col;

    public override int GetHashCode() => Row | (Col << 4);

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !(left == right);

    public static Point Parse(string str)
        => new Point(str[0] - 'A', str[1] - 'a');
}
