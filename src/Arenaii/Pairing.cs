using Arenaii.Data;

namespace Arenaii;

public sealed class Pairing(Bot bot1, Bot bot2)
{
    public Bot Bot1 { get; } = bot1;

    public Bot Bot2 { get; } = bot2;

    /// <summary>
    /// A pairing is considered forbidden if both bots have the same name
    /// and the same major and minor version number.
    /// </summary>
    public bool IsForbidden
    {
        get
        {
            if (Bot1.Name != Bot2.Name)
            {
                return false;
            }
            else if (Bot1.Version is { Length: > 0 } v1 &&
                Bot2.Version is { Length: > 0 } v2)
            {
                var v1s = v1.Split('.');
                var v2s = v2.Split('.');

                return v1s.Length > 2 
                    && v2s.Length > 2
                    && v1s[0] == v2s[0]
                    && v2s[1] == v1s[1];
            }
            else return false;
        }
    }

    public Pairing Mirrored => new(bot1: Bot2, bot2: Bot1);

    public override string ToString() => $"{Bot1.FullName} - {Bot2.FullName}";
}
