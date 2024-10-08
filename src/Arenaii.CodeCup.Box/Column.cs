using System.Numerics;

namespace Arenaii.CodeCup.Box;

public readonly struct Column : IEquatable<Column>
{
    public const int Count = 20;

    public Column(ushort value) => Value = value;

    private readonly ushort Value;

    public bool this[int row] => (Value & (1 << row)) != 0;

    public int Filled => BitOperations.PopCount(Value);

    [Pure]
    public int OverlayCount(uint mask)
    {
        var overlay = Value & mask;
        return BitOperations.PopCount(overlay);
    }

    [Pure]
    public bool Overlay(uint mask) => (Value & mask) != 0;

    [Pure]
    public override string ToString()
    {
        var str = Convert.ToString(Value, 2);
        return str.Length == Row.Count
            ? str
            : new string('0', Row.Count - str.Length) + str;
    }

    [Pure]
    public override bool Equals(object obj) => obj is Column other && Equals(other);

    [Pure]
    public bool Equals(Column other) => Value == other.Value;

    [Pure]
    public override int GetHashCode() => Value;

    public static bool operator ==(Column left, Column right) => left.Equals(right);

    public static bool operator !=(Column left, Column right) => !(left == right);

    public static explicit operator ushort(Column column) => column.Value;
}
