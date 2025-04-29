namespace Arenaii.CodeCup.Box;

public readonly struct Tile : IEquatable<Tile>
{
    public Tile(uint value) => Value = value;

    public readonly uint Value;

    public Color this[int index] => (Color)((Value >> ((5 - index) * 3)) & 0x7);

    public override string ToString() => Convert.ToString(Value, 8);

    public static Tile Parse(string value) => new Tile(Convert.ToUInt32(value, 8));

    public static Tile New(int value) => Parse(value.ToString());

    public override bool Equals(object obj) => obj is Tile tile && Equals(tile);

    public bool Equals(Tile other) => Value == other.Value;

    public override int GetHashCode() => (int)Value;

    public static bool operator ==(Tile left, Tile right) => left.Equals(right);

    public static bool operator !=(Tile left, Tile right) => !(left == right);
}
