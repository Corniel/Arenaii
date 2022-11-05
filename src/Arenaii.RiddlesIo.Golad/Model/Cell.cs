namespace Arenaii.RiddlesIo.Golad.Model;

public class Cell
{
    internal Cell(Cells universe, int index, int x, int y, int neighbors)
    {
        Index = index;
        X = x;
        Y = y;
        Neighbors = new Cell[neighbors];
        Count = neighbors;
        Cells = universe;
    }
    public int Index { get; }
    public int X { get; }
    public int Y { get; }
    public Cell[] Neighbors { get; }
    public int Count { get; }
    private Cells Cells { get; }

    /// <summary>Gets the current owner of the cell.</summary>
    public byte Current => Cells.State[Index];
    public bool IsAlive => Current != Player.None;
    public bool IsDead => Current == Player.None;

    /// <summary>Kills the owner (if any) of the cell.</summary>
    public void Kill() => Cells.State[Index] = Player.None;

    public override string ToString() => $"[{X}, {Y}]" + (IsAlive ? $"Player{Current - 1}" : "");

    /// <summary>Returns true if the cell will be alive next state, otherwise false.</summary>
    /// <remarks>
    /// If a cell has 2 or 3 living neighbors, it continues to live, otherwise it dies.
    /// If a dead cell has exactly 3 living neighbors, it becomes alive.
    /// </remarks>
    public byte Next
    {
        get
        {
            if (IsAlive)
            {
                var alive = 0;
                for (var index = 0; index != Count; index++)
                {
                    if (Neighbors[index].IsAlive)
                    {
                        if (alive == 3)
                        {
                            // overpopulated.
                            return Player.None;
                        }
                        alive++;
                    }
                }
                // not underpopulated.
                return alive > 1 ? Current : Player.None;
            }
            else
            {
                var p1 = 0;
                var p2 = 0;
                for (var index = 0; index != Count; index++)
                {
                    var owner = Neighbors[index].Current;
                    if (owner == Player.Player0)
                    {
                        // too many neighbors to reproduce.
                        if (p1 + p2 == 3)
                        {
                            return Player.None;
                        }
                        p1++;

                    }
                    else if (owner == Player.Player1)
                    {
                        // too many neighbors to reproduce.
                        if (p1 + p2 == 3)
                        {
                            return Player.None;
                        }
                        p2++;
                    }
                }
                // The most influencing reproduces 
                if (p1 + p2 == 3)
                {
                    return p1 > p2 ? Player.Player0 : Player.Player1;
                }
                return Player.None;
            }
        }
    }
}
