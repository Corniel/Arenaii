namespace Arenaii.RiddlesIo.Golad.Model;

public static class Player
{
    public const byte None = 0;
    public const byte Player0 = 1;
    public const byte Player1 = 2;
    public const byte Draw = 3;

    public static byte Other(byte playerToMove) => playerToMove == Player0 ? Player1 : Player0;
}
