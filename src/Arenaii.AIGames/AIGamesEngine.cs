using Arenaii.AIGames.Data;
using Arenaii.Data;

namespace Arenaii.AIGames
{
    public abstract class AIGamesEngine<TCompetition, TSettings> : IEngine<TCompetition, TSettings>
        where TCompetition : Competition<TSettings>
        where TSettings : AIGamesSettings
    {
        protected AIGamesEngine()
        {
            Rnd = new MathNet.Numerics.Random.MersenneTwister();
        }

        public abstract Match Simulate(Pairing pairing, TCompetition competition);

        public Random Rnd { get; set; }
    }
}
