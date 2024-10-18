using Arenaii.Data;
using Qowaiv;

namespace Arenaii;

public class WeightedResult
{
    public Bot Bot1 { get; set; }
    public Bot Bot2 { get; set; }


    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Loses { get; set; }
    public int Count => Wins + Draws + Loses;
    public double Scores { get; set; }

    public Percentage Score => Count == 0 ? 50.Percent() : Percentage.Create(Scores / Count);

    public override string ToString()
    {
        if (Bot1 != null && Bot2 != null)
        {
            return FormattableString.Invariant($"{Wins,4}+ {Draws,4}= {Loses,4}- {Count,4}# {Score.ToString("0.00%"),7} ({Bot1.FullName}-{Bot2.FullName})");
        }
        else
        {
            return FormattableString.Invariant($"{Wins,4}+ {Draws,4}= {Loses,4}- {Count,4}# {Score.ToString("0.00%"),7}");
        }
    }

    public static WeightedResult Merge(IEnumerable<WeightedResult> results)
    {
        var result = new WeightedResult();
        foreach (var res in results)
        {
            result.Wins += res.Wins;
            result.Draws += res.Draws;
            result.Loses += res.Loses;
            result.Scores += res.Scores;
        }
        return result;
    }
}
