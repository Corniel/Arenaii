using System;
using System.Linq;
using System.Text;

namespace Arenaii.Backgammon
{
	public class BackgammonBoard
	{
		private readonly BackgammonField[] fields;

		public BackgammonBoard()
		{
			fields = new BackgammonField[26];
			for (var i = 0; i < 26; i++)
			{
				fields[i] = new BackgammonField();
			}
			fields[01].SetX(2);
			fields[12].SetX(5);
			fields[17].SetX(3);
			fields[19].SetX(5);

			fields[24].SetO(2);
			fields[13].SetO(5);
			fields[08].SetO(3);
			fields[06].SetO(5);
		}

		public BackgammonField this[int index] { get { return fields[index]; } }

		public BackgammonField BarX { get { return this[0]; } }
		public BackgammonField BarO { get { return this[25]; } }

		public int ScoreX { get { return 15 - fields.Where(field => field.OwnedByX).Sum(field => field.Stones); } }
		public int ScoreO { get { return 15 - fields.Where(field => !field.OwnedByX).Sum(field => field.Stones); } }

		public bool NotFinished { get; private set; }
		public bool XToMove { get; private set; }
		public bool XIsWinner { get; private set; }

		public int Turn { get; private set; }

		public void ToConsole()
		{
			throw new NotImplementedException();
		}

		public bool Apply(string move)
		{
			throw new NotImplementedException();
		}

		public string GetGameUpdate()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("update game board {0}", ToString()).AppendLine();
			sb.AppendFormat("update game turn {0}", Turn).AppendLine();
			return sb.ToString();
		}

		public void Loses(bool xLoses)
		{
			NotFinished = false;
			XIsWinner = !xLoses;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			for (var col = 0; col < 26; col++)
			{
				if (col == 1)
				{
					sb.Append('}');
				}
				if (col == 25)
				{
					sb.Append('|');
				}
				if (col == 0 || col == 25)
				{
					sb.Append('{');
				}
				else
				{
					sb.Append('|');
				}

				var value = this[col].Stones;

				var isX = value > 0 && this[col].OwnedByX;
				var isO = value > 0 && !this[col].OwnedByX;

				var owner = '_';
				if (isX)
				{
					owner = isO ? '?' : 'X';
				}
				else if (isO)
				{
					owner = 'O';
				}

				if (value == 0 && owner == '_')
				{
					sb.Append("..");
				}
				else
				{
					sb.AppendFormat("{1}{0}", value, owner);
				}

				if (col == 6 || col == 12 || col == 18)
				{
					sb.Append("|=");
				}
			}
			sb.Append('}');

			sb.AppendFormat(" {0}-{1}", ScoreX, ScoreO);

			return sb.ToString();
		}
	}
}
