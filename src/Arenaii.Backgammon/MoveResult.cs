namespace Arenaii.Backgammon;

public enum MoveResult
{
    TimeOut = -1,
    Invalid = 0,
    EmptyMove,
    TooManyMoves,
    UnparsebleMove,
    MustMoveFromBarFirst,
    NoStoneOnSourceField,
    TargetFieldIsBlocked,
    BearOffIsNotAllowed,
    MoveDoesNotMatchDice,
    Ok = 255,
}
