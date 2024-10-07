namespace Arenaii.AIGames.UltimateTicTacToe;

public class Field
{
    public Field(int row, int col)
    {
        Row = row;
        Col = col;
    }
    public int Row { get; }
    public int Col { get; }
    public int Value { get; set; }

    public override string ToString() { return Value.ToString(); }
}
