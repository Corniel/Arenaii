using Arenaii.Data;

namespace Arenaii;

public abstract class Simulator<TCompetition, TSettings>
		where TCompetition : Competition<TSettings>
		where TSettings : Settings
	{
		protected Simulator()
		{
			Competition = Competition<TSettings>.Load<TCompetition>();
		}

		public IEngine<TCompetition, TSettings> Engine { get; protected set; }

		public TCompetition Competition { get; protected set; }

		public virtual void Run(string[] args)
		{
			while (true)
			{
				Competition.Bots.Activate();
				var queue = CreatePairings();
				if (queue.Count == 0) { return; }
				while (queue.Count > 0)
				{
					var pairing = queue.Dequeue();
					var outcome = Engine.Simulate(pairing, Competition);
					Competition.Matches.Add(outcome);

					Competition.RecalculateElo();
					Competition.Save();
					Competition.WriteResults();
				}
			}
		}

    private Queue<Pairing> CreatePairings() 
        => Competition.Settings.Pairing switch
        {
            PairingType.Frequency => CreatePairingsByFrequency(),
            _ => CreateRandomPairings(),
        };

    private Queue<Pairing> CreateRandomPairings()
		{
			var queue = new Queue<Pairing>();
			var sorted = Competition.Bots
				.Where(bot => bot.IsActive)
				.OrderBy(bot => Engine.Rnd.Next())
				.ToArray();

			for (var i = 1; i < sorted.Length; i += 2)
			{
				queue.Enqueue(new Pairing(sorted[i - 1], sorted[i]));
				if (!Competition.Settings.IsSymetric)
				{
					queue.Enqueue(new Pairing(sorted[i], sorted[i - 1]));
				}
			}

			return queue;
		}
		private Queue<Pairing> CreatePairingsByFrequency()
		{
			var queue = new Queue<Pairing>();

			var results = Competition
                .GetWeightedResults()
                .Where(r => r.Bot1.IsActive && r.Bot2.IsActive)
                .OrderBy(res => res.Count)
                .ToList();

			if (results.Count == 0) { return queue; }

			var freq = results[0].Count;

			foreach (var res in results)
			{
				if (res.Count != freq) { break; }
				queue.Enqueue(new Pairing(res.Bot1, res.Bot2));
			}
			return queue;
		}
	}
