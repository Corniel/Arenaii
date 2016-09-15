using Arenaii.Data;
using Troschuetz.Random;

namespace Arenaii
{
	public interface IEngine<TCompetition, TSettings>
		where TCompetition : Competition<TSettings>
		where TSettings : Settings
	{
		IGenerator Rnd { get; set; }

		Match Simulate(Pairing pairing, TCompetition competition);
	}
}
