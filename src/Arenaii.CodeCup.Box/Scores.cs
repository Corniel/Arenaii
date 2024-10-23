namespace Arenaii.CodeCup.Box;

[StructLayout(LayoutKind.Auto)]
public readonly struct Scores
{
    private const ulong Mask = 0x3FF;

    public static readonly Scores None = new Scores(0);

    public Scores(ulong value)
    {
        Value = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ulong Value;

    public int this[Color color] => (int)(Mask & (Value >> Shift(color)));

    public Scores Add(ulong val, Color color) => new Scores((val << Shift(color)) + Value);

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var color = Color.Red; color <= Color.Purple; color++)
        {
            var score = this[color];
            if (score != 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append($"{color} = {score}");
            }
        }
        return sb.Length == 0
            ? "{no scores}"
            : sb.ToString();
    }

    public static Scores operator +(Scores l, Scores r) => new Scores(l.Value + r.Value);

    public static Scores Get(ushort[] columns)
    {
        var score = None;

        ushort row_mask = 1;

        for (var row = 0; row < Row.Count - 1; row++)
        {
            var max_row = 15 - row;

            for (var col = 0; col < Column.Count - 1; col++)
            {
                // No fill
                var color = GetColor(columns, row_mask, col);

                if (color != Color.None)
                {
                    var max_col = 19 - col;
                    var max = Math.Min(max_col, max_row);
                    var index = ((int)color) * Column.Count + col;

                    for (byte sc = 1; sc <= max; sc++)
                    {
                        var mask = row_mask | (row_mask << sc);

                        if ((mask & columns[index] & columns[index+sc]) == mask)
                        {
                            score = score.Add(sc, color);
                        }
                    }
                }
            }
            row_mask <<= 1;
        }

        return score;
    }

    [Pure]
    private static Color GetColor(ushort[] columns, ushort row_mask, int col)
    {
        if ((columns[col] & row_mask) == 0) return Color.None;

        var offset = col;

        for (var color = Color.Red; color < Color.Purple; color++)
        {
            offset += Column.Count;

            if ((row_mask & columns[offset]) != 0)
            {
                return color;
            }
        }

        return Color.Purple;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Shift(Color color) =>10 * (((int)color) - 1);

    public int Best(Color except)
    {
        var buffer = Value;
        var score = 0;

        for (var color = Color.Red; color <= Color.Purple; color++)
        {
            if (color != except)
            {
                int test = (int)(buffer & Mask);
                if (test > score)
                {
                    score = test;
                }
            } 
            buffer >>= 10;
        }
        return score;
    }
}
