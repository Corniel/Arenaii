using System.Numerics;

namespace Arenaii.CodeCup.Box;

[StructLayout(LayoutKind.Auto)]
public readonly struct Board : IEquatable<Board>
{
    public static readonly Board Empty = new Board(new ushort[Layer.Count * Column.Count]);

    public Board(ushort[] columns) => Columns = columns;

    private readonly ushort[] Columns;

    public Column this[Color color, int col] => new Column(Columns[Idx(color, col)]);

    public Column this[int index] => new Column(Columns[index]);

    public int AllDots => Dots(Color.None);

    [Pure]
    public int Dots(Color color)
    {
        var count = 0;
        var offset = Column.Count * (int)color;
        var max = offset + Column.Count;

        for (var i = offset; i < max; i++)
        {
            count += BitOperations.PopCount(Columns[i]);
        }
        return count;
    }

    [Pure]
    public Board Move(Move move) => Move(move, new ushort[Layer.Count * Column.Count]);

    [Pure]
    public Board Move(Move move, ushort[] columns)
    {
        Array.Copy(Columns, columns, Layer.Count * Column.Count);

        if (move.Horizontal) MoveHorizontal(move, move.Tile.Value, columns);
        else /*...........*/ MoveVertical(move, move.Tile.Value, columns);

        return new Board(columns);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MoveHorizontal(Move move, uint colors, ushort[] columns)
    {
        var row0 = (ushort)(1 << move.Point.Row + 0);
        var row1 = (ushort)(row0 << 1);

        var col0 = move.Point.Col + 5;
        var col1 = move.Point.Col + 0;

        for (var i = 0; i < 6; i++)
        {
            var color = (Color)(colors & 7);

            MovePrepare(columns, row0, col0);
            MovePrepare(columns, row1, col1);

            // Set color
            columns[Idx(color, col0)] |= row0;
            columns[Idx(color, col1)] |= row1;

            // Next
            colors >>= 3;
            col0--;
            col1++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MoveVertical(Move move, uint colors, ushort[] columns)
    {
        var row0 = (ushort)(1 << move.Point.Row + 0);
        var row1 = (ushort)(row0 << 5);

        var col1 = move.Point.Col + 1;
        var col0 = move.Point.Col + 0;

        for (var i = 0; i < 6; i++)
        {
            var color = (Color)(colors & 7);

            MovePrepare(columns, row0, col0);
            MovePrepare(columns, row1, col1);

            // Set color
            columns[Idx(color, col0)] |= row0;
            columns[Idx(color, col1)] |= row1;

            // Next
            colors >>= 3;
            row0 <<= 1;
            row1 >>= 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MovePrepare(ushort[] columns, ushort row, int col)
    {
        if ((columns[col] & row) == 0)
        {
            columns[col] |= row;
        }
        else
        {
            var clr1 = (ushort)(ushort.MaxValue ^ row);
            for (Color c = Color.Red; c <= Color.Purple; c++)
            {
                columns[Idx(c, col)] &= clr1;
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Idx(Color color, int col) => (int)color * Column.Count + col;

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var row = 0; row < Row.Count; row++)
        {
            for (var col = 0; col < Column.Count; col++)
            {
                var filled = false;
                for (var color = Color.Red; color <= Color.Purple; color++)
                {
                    if (this[color, col][row])
                    {
                        sb.Append((int)color);
                        filled = true;
                    }
                }
                if (!filled)
                {
                    sb.Append('.');
                }
                if (col % 4 == 3 && col < 19)
                {
                    sb.Append('|');
                }
            }
            sb.AppendLine();
            if (row % 4 == 3 && row < 15)
            {
                sb.AppendLine("----+----+----+----+----");
            }
        }
        return sb.ToString();
    }

    public override bool Equals(object obj) => obj is Board other && Equals(other);

    public bool Equals(Board other)
        => Enumerable.SequenceEqual(Columns, other.Columns);

    public override int GetHashCode()
    {
        var hash = 0;
        for(var i = 0; i < Columns.Length; i++)
        {
            hash *= 17;
            hash ^= Columns[i].GetHashCode();
        }
        return hash;
    }

    public ushort[] ToArray() => Columns;
}
