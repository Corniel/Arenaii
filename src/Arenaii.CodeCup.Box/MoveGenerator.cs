namespace Arenaii.CodeCup.Box;

public static class MoveGenerator
{
    [DebuggerDisplay("Row = {row}, Col = {col}")]
    public struct Horizontal
    {
        private readonly Board Board;
        private int row;
        private int col;

        public Horizontal(Board board)
        {
            Board = board;
            row = 0;
            col = -1;
        }

        public Point Current => new Point(row, col);

        public bool MoveNext()
        {
            while (true)
            {
                // end of columns.
                if (++col > 'o' - 'a')
                {
                    col = 0;
                    // we're done.
                    if (++row > 'O' - 'A') return false;
                }

                var shared = Mask.Horizontal.Shared << row;

                // First two can not exceed 4.
                var overlay = 0
                    + Board[col + 0].OverlayCount(shared)
                    + Board[col + 1].OverlayCount(shared);

                // Too much overlay
                if ((overlay += Board[col + 2].OverlayCount(shared)) > 4 ||
                    (overlay += Board[col + 3].OverlayCount(shared)) > 4 ||
                    (overlay += Board[col + 4].OverlayCount(shared)) > 4 ||
                    (overlay += Board[col + 5].OverlayCount(shared)) > 4)
                {
                    continue;
                }

                // we're done.
                if (overlay != 0) return true;

                // No overlay, check alignment.
                var extended = row == 0
                    ? (Mask.Horizontal.Extended >> 1)
                    : (Mask.Horizontal.Extended << (row - 1));

                // vertical alignment.
                if (Board[col + 0].Overlay(extended) ||
                    Board[col + 1].Overlay(extended) ||
                    Board[col + 2].Overlay(extended) ||
                    Board[col + 3].Overlay(extended) ||
                    Board[col + 4].Overlay(extended) ||
                    Board[col + 5].Overlay(extended))
                {
                    return true;
                }
                if (col != 0 && Board[col - 1].Overlay(shared))
                {
                    return true;
                }
                else if (col != Column.Count - 6 && Board[col + 6].Overlay(shared))
                {
                    return true;
                }
            }
        }

        public override string ToString() => Format(row, col);
    }

    [DebuggerDisplay("Row = {row}, Col = {col}")]
    public struct Vertical
    {
        private readonly Board Board;
        private int row;
        private int col;

        public Vertical(Board board)
        {
            Board = board;
            row = 0;
            col = -1;
        }

        public Point Current => new Point(row, col);

        public bool MoveNext()
        {
            while (true)
            {
                // end of columns.
                if (++col > 's' - 'a')
                {
                    col = 0;
                    // we're done.
                    if (++row > 'K' - 'A') return false;
                }

                var shared = Mask.Vertical.Shared << row;

                var overlay = 0;

                // Too much overlay
                if ((overlay += Board[col + 0].OverlayCount(shared)) > 4 ||
                    (overlay += Board[col + 1].OverlayCount(shared)) > 4)
                {
                    continue;
                }

                // we're done.
                if (overlay != 0) return true;

                // No overlay, check alignment.
                var extended = row == 0
                    ? (Mask.Vertical.Extended >> 1)
                    : (Mask.Vertical.Extended << (row - 1));

                // vertical alignment.
                if (Board[col + 0].Overlay(extended) ||
                    Board[col + 1].Overlay(extended))
                {
                    return true;
                }
                if (col != 0 && Board[col - 1].Overlay(shared))
                {
                    return true;
                }
                else if (col != Column.Count - 2 && Board[col + 2].Overlay(shared))
                {
                    return true;
                }
            }
        }

        public override string ToString() => Format(row, col);
    }

    [Pure]
    private static bool Break(string pt, int row, int col)
    {
        var point = Point.Parse(pt);
        return point.Row == row && point.Col == col;
    }

    private static string Format(int row, int col) => $"Row = {row}, Col = {col}, {new Point(row, col)}";
}
