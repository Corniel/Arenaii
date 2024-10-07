using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves;

public struct BirthMove : IMove
{
    public BirthMove(Cell child, Cell father, Cell mother)
    {
        Child = child;
        Father = father;
        Mother = mother;
    }
    public Cell Child { get; }
    public Cell Father { get; }
    public Cell Mother { get; }

    public bool Apply(Cells cells)
    {
        // We can't revoke a cell that is alive.
        if (Child.IsAlive)
        {
            return false;
        }
        // It should be two different cells that die.
        if (Father == Mother)
        {
            return false;
        }
        var player = cells.P0ToMove ? Player.Player0 : Player.Player1;

        // Wrong owner (of not even alive).
        if (Mother.Current != player || Father.Current != player)
        {
            return false;
        }
        cells.State[Father.Index] = Player.None;
        cells.State[Mother.Index] = Player.None;
        cells.State[Child.Index] = player;
        return true;
    }

    public override string ToString() => $"Birth {Child.X},{Child.Y} {Father.X},{Father.Y} {Mother.X},{Mother.Y}";
}
