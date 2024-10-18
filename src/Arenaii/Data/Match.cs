using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Arenaii.Data;

[DebuggerDisplay("{DebuggerDisplay}")]
[Serializable]
public sealed class Match
{
    public Match() { }

    public Match(Bot bot1, Bot bot2, float score)
    {
        Id1 = bot1.Id;
        Id2 = bot2.Id;
        Score = score;
    }
    [XmlAttribute("id1")]
    public string Id1 { get; init; }

    [XmlAttribute("id2")]
    public string Id2 { get; init; }

    [XmlAttribute("s")]
    public float Score { get; init; }

    [XmlAttribute("d1")]
    public int MilliSeconds1
    {
        get => (int)Duration1.TotalMilliseconds;
        init => Duration1 = TimeSpan.FromMilliseconds(value);
    }

    [XmlIgnore]
    public TimeSpan Duration1 { get; init; }

    [XmlAttribute("d2")]
    public int MilliSeconds2
    {
        get => (int)Duration2.TotalMilliseconds;
        init => Duration2 = TimeSpan.FromMilliseconds(value);
    }

    [XmlIgnore]
    public TimeSpan Duration2 { get; init; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
    private string DebuggerDisplay => string.Format("{0}-{1} : {2}-{3}", Id1, Id2, Score, 1 - Score);
} 
