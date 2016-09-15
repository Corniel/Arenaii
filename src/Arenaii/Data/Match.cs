using Qowaiv;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Arenaii.Data
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	[Serializable]
	public class Match
	{
		public Match() { }

		public Match(Bot bot1, Bot bot2, float score)
		{
			Id1 = bot1.Id;
			Id2 = bot2.Id;
			Score = score;
		}
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("id1")]
		public string Id1 { get; set; }
		[XmlAttribute("id2")]
		public string Id2 { get; set; }
		[XmlAttribute("s")]
		public float Score { get; set; }

		[XmlAttribute("d1")]
		public int MilliSeconds1 { get { return (int)Duration1.TotalMilliseconds; } set { Duration1 = TimeSpan.FromMilliseconds(value); } }
		[XmlIgnore]
		public TimeSpan Duration1 { get; set; }

		[XmlAttribute("d2")]
		public int MilliSeconds2 { get { return (int)Duration2.TotalMilliseconds; } set { Duration2 = TimeSpan.FromMilliseconds(value); } }
		[XmlIgnore]
		public TimeSpan Duration2 { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay { get { return string.Format("{0}-{1} : {2}-{3}", Id1, Id2, Score, 1 - Score); } }
	}
}
