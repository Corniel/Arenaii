using Arenaii.Data;

namespace Arenaii;

public interface IEngine<TCompetition, TSettings>
    where TCompetition : Competition<TSettings>
    where TSettings : Settings
{
    Random Rnd { get; }

    Match Simulate(Pairing pairing, TCompetition competition);
}
