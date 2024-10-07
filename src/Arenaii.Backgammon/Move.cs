namespace Arenaii.Backgammon;

public struct Move
{
    public static readonly Move NoPlay;

    private readonly byte src;
    private readonly byte tar;
    private readonly bool cap;

    public Move(int source, int target) : this(source, target, false) { }
    public Move(int source, int target, bool isCapture)
    {
        src = (byte)source;
        tar = (byte)target;
        cap = isCapture;
    }

    public int Source { get { return src; } }
    public int Target { get { return tar; } }
    public bool IsCapture { get { return cap; } }

    public int Movement { get { return Math.Abs(Target - Source); } }

    public bool IsL2R { get { return Target > Source; } }

    public bool IsValid(int dice, bool xToMove)
    {
        if (IsL2R != xToMove) { return false; }

        if (Movement == dice) { return true; }

        // Bear-off?
        if (Movement < dice)
        {
            return (xToMove ? Board.BarOIndex : Board.BarXIndex) == Target;
        }
        return false;
    }

    public override string ToString()
    {
        if (NoPlay.Equals(this)) { return "(no play)"; }

        return string.Format(
            "{0}/{1}{2}",
            Source == 0 || Source == 25 ? "bar" : Source.ToString(),
            Target == 0 || Target == 25 ? "off" : Target.ToString(),
            IsCapture ? "*" : "");
    }

    public override int GetHashCode()
    {
        var hash = cap.GetHashCode();
        hash |= src << 1;
        hash |= tar << 9;
        return hash;
    }

    public static Move Parse(string str, bool xToMove)
    {
        if (string.IsNullOrEmpty(str) || !str.Contains("/")) { return NoPlay; }

        var s = str.Trim().ToUpperInvariant();
        var source = 0;
        var target = 0;
        var capture = s.EndsWith("*");

        if (capture)
        {
            s = s.Substring(0, s.Length - 1);
        }
        if (s.StartsWith("BAR/"))
        {
            source = xToMove ? Board.BarXIndex : Board.BarOIndex;
        }
        else if (!int.TryParse(s.Substring(0, s.IndexOf('/')), out source))
        {
            return NoPlay;
        }

        if (s.EndsWith("/OFF"))
        {
            target = xToMove ? Board.BarOIndex : Board.BarXIndex;
        }
        else if (!int.TryParse(s.Substring(s.IndexOf('/') + 1), out target))
        {
            return NoPlay;
        }

        // source or target is outside the board.
        if (source < Board.BarXIndex || source > Board.BarOIndex ||
            target < Board.BarXIndex || target > Board.BarOIndex)
        {
            return NoPlay;
        }

        return new Move(source, target, capture);
    }
}
