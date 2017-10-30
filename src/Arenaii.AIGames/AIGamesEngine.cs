using Arenaii.AIGames.Data;
using Arenaii.Data;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace Arenaii.AIGames
{
    public abstract class AIGamesEngine<TCompetition, TSettings> : IEngine<TCompetition, TSettings>
        where TCompetition : Competition<TSettings>
        where TSettings : AIGamesSettings
    {
        protected AIGamesEngine()
        {
            Rnd = new MT19937Generator();
        }

        public abstract Match Simulate(Pairing pairing, TCompetition competition);

        public IGenerator Rnd { get; set; }
    }
}
