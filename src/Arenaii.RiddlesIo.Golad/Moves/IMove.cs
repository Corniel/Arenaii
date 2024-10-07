using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves;

public interface IMove
{
    bool Apply(Cells cells);
}
