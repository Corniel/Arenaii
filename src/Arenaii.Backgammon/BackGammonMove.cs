namespace Arenaii.Backgammon
{
	public struct BackGammonMove
	{
		public static readonly BackGammonMove NoPlay;

		private readonly byte src;
		private readonly byte tar;
		private readonly bool cap;

		public BackGammonMove(int source, int target) : this(source, target, false) { }
		public BackGammonMove(int source, int target, bool isCapture)
		{
			src = Guard(source);
			tar = Guard(target);
			cap = isCapture;
		}
		private static byte Guard(int value)
		{
			if (value < BackgammonBoard.BarXIndex) { return BackgammonBoard.BarXIndex; }
			if (value > BackgammonBoard.BarOIndex) { return BackgammonBoard.BarOIndex; }
			return (byte)value;
		}

		public int Source { get { return src; } }
		public int Target { get { return tar; } }
		public bool IsCapture { get { return cap; } }

		public bool IsL2R { get { return Target > Source; } }

		public override string ToString()
		{
			if (NoPlay.Equals(this)) { return "(no play)"; }

			return string.Format(
				"{0}/{1}{2}",
				Source == 0 || Source == 25 ? "bar" : Source.ToString(),
				Target == 0 || Target == 25 ? "off" : Target.ToString(),
				IsCapture ? "*" : "");
		}

		public override int GetHashCode()
		{
			var hash = cap.GetHashCode();
			hash |= src << 1;
			hash |= tar << 9;
			return hash;
		}

		public static BackGammonMove Parse(string str, bool xToMove)
		{
			if (string.IsNullOrEmpty(str) || !str.Contains("/")) { return NoPlay; }

			var s = str.Trim().ToUpperInvariant();
			var source = 0;
			var target = 0;
			var capture = s.EndsWith("*");

			if (capture)
			{
				s = s.Substring(0, s.Length - 1);
			}
			if (s.StartsWith("BAR/"))
			{
				source = xToMove ? BackgammonBoard.BarXIndex : BackgammonBoard.BarOIndex;
			}
			else if (!int.TryParse(s.Substring(0, s.IndexOf('/')), out source))
			{
				return NoPlay;
			}

			if (s.EndsWith("/OFF"))
			{
				source = xToMove ? BackgammonBoard.BarOIndex : BackgammonBoard.BarXIndex;
			}
			else if (!int.TryParse(s.Substring(s.IndexOf('/') + 1), out target))
			{
				return NoPlay;
			}
			return new BackGammonMove(source, target, capture);
		}
	}
}
