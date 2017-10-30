namespace Arenaii.RiddlesIo.Golad.Model
{
    public class Cell
    {
        internal Cell(Cells universe, int index, int row, int col, int neighbors)
        {
            Index = index;
            Row = row;
            Col = col;
            Neighbors = new Cell[neighbors];
            Count = neighbors;
            Universe = universe;
        }
        public int Index { get; }
        public int Row { get; }
        public int Col { get; }
        public Cell[] Neighbors { get; }
        public int Count { get; }
        private Cells Universe { get; }

        /// <summary>Gets the current owner of the cell.</summary>
        public byte Owner => Universe.State[Index];
        public bool IsAlive => Owner != Player.None;
        public bool IsDead => Owner == Player.None;

        /// <summary>Kills the owner (if any) of the cell.</summary>
        public void Kill() => Universe.State[Index] = Player.None;

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
                    return alive > 1 ? Owner : Player.None;
                }
                else
                {
                    var p1 = 0;
                    var p2 = 0;
                    for (var index = 0; index != Count; index++)
                    {
                        var owner = Neighbors[index].Owner;
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
}
