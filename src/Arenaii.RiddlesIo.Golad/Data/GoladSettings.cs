using Arenaii.AIGames.Data;

namespace Arenaii.RiddlesIo.Golad.Data
{
    public class GoladSettings: AIGamesSettings
    {
        public int Height { get; set; } = 16;
        public int Width { get; set; } = 18;
        public int InitialPlayerCount { get; set; } = 50;
        public int MaximumRounds { get; set; } = 100;
    }
}
