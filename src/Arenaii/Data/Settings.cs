namespace Arenaii.Data;

[Serializable]
public abstract class Settings
{
    public bool IsSymetric { get; init; }
    public Elo AverageElo { get; init; } = 1600;
    public PairingType Pairing { get; init; }
}
