using Arenaii.AIGames.Data;

namespace Arenaii.RiddlesIo.Golad.Data
{
    public class GoladSettings: AIGamesSettings
    {
        public GoladSettings()
        {
            PlayerNames = new PlayerName[] { PlayerName.player0, PlayerName.player1 };
            TimePerMove = 100;
        }
        public int Height { get; set; } = 16;
        public int Width { get; set; } = 18;
        public int InitialPlayerCount { get; set; } = 40;
        public int MaximumRounds { get; set; } = 100;
        public bool VisualizeNext { get; set; } = true;
    }
}
