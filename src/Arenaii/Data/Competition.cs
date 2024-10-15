using Arenaii.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace Arenaii.Data;

[Serializable]
public abstract class Competition<T> where T : Settings
{
    protected Competition()
    {
        Bots = new Bots();
        Matches = new List<Match>();
        Settings = Activator.CreateInstance<T>();
    }

    public T Settings { get; set; }

    public Bots Bots { get; protected set; }

    public IEnumerable<Bot> RankingBots { get { return Bots.Where(b => b.IsActive || b.IsReference); } }

    public List<Match> Matches { get; protected set; }

    public void Remove(Bot bot)
    {
        Guard.NotNull(bot, "bot");

        Bots.Remove(bot);
        RemoveUnlinkedMatches();
    }

    public void RemoveUnlinkedMatches()
    {
        var ids = new HashSet<string>(Bots.Select(bot => bot.Id));
        for (var index = Matches.Count - 1; index >= 0; index--)
        {
            var match = Matches[index];
            if (ids.Contains(match.Id1) && ids.Contains(match.Id2))
            {
                continue;
            }
            Matches.Remove(match);
        }
    }

    public IEnumerable<WeightedResult> GetWeightedResults()
    {
        return Settings.IsSymetric ? GetSymetricWeightedResults() : GetASymetricWeightedResults();
    }

    private IEnumerable<WeightedResult> GetSymetricWeightedResults()
    {
        foreach (var bot1 in RankingBots)
        {
            foreach (var bot2 in RankingBots.Where(bot => bot != bot1))
            {
                var matches = Matches.Where
                (m =>
                    (m.Id1 == bot1.Id && m.Id2 == bot2.Id) ||
                    (m.Id1 == bot2.Id && m.Id2 == bot1.Id)
                );

                var result = new WeightedResult()
                {
                    Bot1 = bot1,
                    Bot2 = bot2,
                };

                foreach (var match in matches)
                {
                    var sc = (int)Math.Round(match.Score * 2);

                    // mirrored.
                    if (match.Id1 != bot1.Id)
                    {
                        sc = 2 - sc;
                    }
                    if (sc == 2)
                    {
                        result.Wins++;
                    }
                    else if (sc == 1)
                    {
                        result.Draws++;
                    }
                    else
                    {
                        result.Loses++;
                    }
                }
                yield return result;
            }
        }
    }
    private IEnumerable<WeightedResult> GetASymetricWeightedResults()
    {
        foreach (var bot1 in RankingBots)
        {
            foreach (var bot2 in RankingBots.Where(bot => bot != bot1))
            {
                var matches = Matches.Where(m => m.Id1 == bot1.Id && m.Id2 == bot2.Id);
                var result = new WeightedResult()
                {
                    Bot1 = bot1,
                    Bot2 = bot2,
                };

                foreach (var match in matches)
                {
                    var sc = (int)Math.Round(match.Score * 2);
                    if (sc == 2)
                    {
                        result.Wins++;
                    }
                    else if (sc == 1)
                    {
                        result.Draws++;
                    }
                    else
                    {
                        result.Loses++;
                    }
                }
                yield return result;
            }
        }
    }

    public void RecalculateElo()
    {
        var results = GetWeightedResults().ToList();

        for (var k = 128.0; k >= 0.48; k /= 2)
        {
            foreach (var result in results.Where(r => r.Count > 0))
            {
                Bot bot1 = result.Bot1;
                Bot bot2 = result.Bot2;
                var z = Elo.GetZScore(bot1.Rating, bot2.Rating);

                var delta = (double)result.Score - z;
                // Over 10 games does not give any extra weight.
                var f = Math.Min(result.Count, 10) / 10d;

                bot1.Rating += delta * k * f;
                bot2.Rating -= delta * k * f;
            }
        }

        var dt = Bots.Rating - Settings.AverageElo;

        foreach (var bot in Bots)
        {
            bot.Rating -= dt;
        }
        Bots.Sort();
    }

    public void WriteResults()
    {
        var pos = 1;

        using var writer = new StreamWriter(new FileStream(AppConfig.ResultsFile.FullName, FileMode.Create, FileAccess.Write));
        foreach (var bot in Bots)
        {
            writer.WriteLine("{0,4}  {1,4}  {2} ({3})", pos++, bot.Rating.ToString("0"), bot.FullName, Matches.Count(m => m.Id1 == bot.Id || m.Id2 == bot.Id));
        }
        writer.WriteLine();

        var results = GetWeightedResults().ToList();

        var combined = WeightedResult.Merge(results);

        writer.WriteLine($"Total: {combined}");
        writer.WriteLine();

        foreach (var bot in Bots)
        {
            writer.WriteLine("Opponents: {0} ({1:0})", bot.FullName, bot.Rating);

            var botResults = results
                .Where(res => res.Bot1 == bot || (res.Bot2 == bot && !Settings.IsSymetric))
                .Where(res => res.Count > 0)
                .OrderByDescending(res => res.Bot1 == bot)
                .ThenBy(res => res.Bot1 == bot ? +res.Score : -res.Score)
                .ToList();

            foreach (var oppo in botResults)
            {
                writer.WriteLine("  {0}", oppo);
            }
            writer.WriteLine();
        }
    }

    public void Save() { Save(AppConfig.CompetitionDirectory); }

    public void Save(DirectoryInfo directory)
    {
        Guard.NotNull(directory, "directory");
        if (!directory.Exists) { directory.Create(); }

        var file = new FileInfo(Path.Combine(directory.FullName, GetType().Name + ".xml"));

        using var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write);
        var serializer = new XmlSerializer(GetType());
        serializer.Serialize(stream, this);
    }

    public static TCompetition Load<TCompetition>() where TCompetition : Competition<T>
    {
        return Load<TCompetition>(AppConfig.CompetitionDirectory);
    }
    public static TCompetition Load<TCompetition>(DirectoryInfo directory) where TCompetition : Competition<T>
    {
        Guard.NotNull(directory, "directory");
        if (!directory.Exists) { directory.Create(); }

        var file = new FileInfo(Path.Combine(directory.FullName, typeof(TCompetition).Name + ".xml"));

        if (!file.Exists)
        {
            return Activator.CreateInstance<TCompetition>();
        }
        using var stream = file.OpenRead();
        var serializer = new XmlSerializer(typeof(TCompetition));
        var data = (TCompetition)serializer.Deserialize(stream);
        data.RemoveUnlinkedMatches();
        return data;
    }
}
