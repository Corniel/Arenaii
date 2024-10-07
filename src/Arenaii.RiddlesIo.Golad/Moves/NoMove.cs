using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves;

public struct NoMove : IMove
{
    public bool Apply(Cells cells) => false;

    public override string ToString() => string.Empty;
}
