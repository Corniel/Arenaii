using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves;

public struct PassMove : IMove
{
    public bool Apply(Cells cells) => true;
    public override string ToString() => "Pass";
}
