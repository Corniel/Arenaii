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

    public static Scores Get(ushort[] columns, Point pt, Color color)
    {
        var scores = None;

        var col_idx = Board.Idx(color, pt.Col);
        var column = columns[col_idx];

        var w = pt.Col;
        var n = pt.Row;
        var e = Column.Count - pt.Col - 1;
        var s = Row.Count - pt.Row - 1;

        var nw = Math.Min(n, w);
        var ne = Math.Min(n, e);
        var se = Math.Min(s, e);
        var sw = Math.Min(s, w);

        // NW
        for (byte score = 1; score <= nw; score++)
        {
            var row = pt.Row - score;
            var mask = (ushort)Masks[score][row];
            var other = columns[col_idx - score];

            if ((mask & column & other) == mask)
            {
                scores = scores.Add(score, color);
            }
        }

        // NE
        for (byte score = 1; score <= ne; score++)
        {
            var row = pt.Row - score;
            var mask = (ushort)Masks[score][row];
            var other = columns[col_idx + score];

            if ((mask & column & other) == mask)
            {
                scores = scores.Add(score, color);
            }
        }

        // SE
        for (byte score = 1; score <= se; score++)
        {
            var mask = (ushort)Masks[score][pt.Row];
            var other = columns[col_idx + score];

            if ((mask & column & other) == mask)
            {
                scores = scores.Add(score, color);
            }
        }

        // SW
        for (byte sc = 1; sc <= sw; sc++)
        {
            var mask = (ushort)Masks[sc][pt.Row];
            var other = columns[col_idx - sc];

            if ((mask & column & other) == mask)
            {
                scores = scores.Add(sc, color);
            }
        }

        return scores;
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

    public static readonly Column[][] Masks = new Column[][]
    {
        Array.Empty<Column>(),
        Init(01),
        Init(02),
        Init(03),
        Init(04),
        Init(05),
        Init(06),
        Init(07),
        Init(08),
        Init(09),
        Init(10),
        Init(11),
        Init(12),
        Init(13),
        Init(14),
        Init(15),
    };

    private static Column[] Init(int points)
    {
        var column = 1 | (1 << points);

        var columns = new Column[Row.Count - points];

        for (var i = 0; i < columns.Length; i++)
        {
            columns[i] = new Column((ushort)column);
            column <<= 1;
        }
        return columns;
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
