using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves
{
    public struct KillMove : IMove
    {
        public KillMove(Cell cell) => Cell = cell;
        public Cell Cell { get; }

        public bool Apply(Cells cells)
        {
            if (Cell.IsDead)
            {
                return false;
            }
            cells.State[Cell.Index] = Player.None;
            return true;
        }

        public override string ToString() => $"Kill {Cell.Row}, {Cell.Col}";
    }
}
