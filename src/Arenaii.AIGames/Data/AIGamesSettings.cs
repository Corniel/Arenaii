using Arenaii.Data;

namespace Arenaii.AIGames.Data;

[Serializable]
public abstract class AIGamesSettings : Settings
{
    protected AIGamesSettings()
    {
        TimeBank = 10000;
        TimePerMove = 500;
        PlayerNames = new PlayerName[] { PlayerName.player1, PlayerName.player2 };
    }
    public int TimeBank { get; set; }
    public int TimePerMove { get; set; }
    public PlayerName[] PlayerNames { get; set; }
}
