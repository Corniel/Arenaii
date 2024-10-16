using Arenaii.Data;

namespace Arenaii;

public sealed class Pairing(Bot bot1, Bot bot2)
{
    public Bot Bot1 { get; } = bot1;

    public Bot Bot2 { get; } = bot2;

    public override string ToString() => $"{Bot1.FullName} - {Bot2.FullName}";
}
