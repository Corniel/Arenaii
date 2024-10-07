using Arenaii.Data;

namespace Arenaii;

public interface IEngine<TCompetition, TSettings>
		where TCompetition : Competition<TSettings>
		where TSettings : Settings
	{
		Random Rnd { get; set; }

		Match Simulate(Pairing pairing, TCompetition competition);
	}
