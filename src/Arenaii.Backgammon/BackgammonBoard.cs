using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arenaii.Backgammon
{
	public class BackgammonBoard
	{
		private readonly BackgammonField[] fields;
		public const int BarXIndex = 0;
		public const int BarOIndex = 25;

		public BackgammonBoard()
		{
			fields = new BackgammonField[26];
			for (var i = 0; i < 26; i++)
			{
				fields[i] = new BackgammonField();
			}
			fields[BarXIndex].SetX(0);
			fields[BarOIndex].SetO(0);

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

		public BackgammonField BarX { get { return this[BarXIndex]; } }
		public BackgammonField BarO { get { return this[BarOIndex]; } }

		public int ScoreX { get { return 15 - fields.Where(field => field.OwnedByX).Sum(field => field.Stones); } }
		public int ScoreO { get { return 15 - fields.Where(field => !field.OwnedByX).Sum(field => field.Stones); } }

		public bool NotFinished { get; private set; }
		public bool XToMove { get; private set; }
		public bool XIsWinner { get; private set; }

		public int Turn { get; private set; }

		/// <summary>Renders the board to the console.</summary>
		/// <remarks>
		///  13 14 15 16 17 18      19 20 21 22 23 24
		/// o------------------o  o------------------o
		/// | O  .  .  .  X  . |  | X  .  .  .  .  O |
		/// | O  .  .  .  X  . |  | X  .  .  .  .  O |
		/// | O  .  .  .  X  . |  | X  .  .  .  .  . |
		/// | O  .  .  .  .  . |  | X  .  .  .  .  . |
		/// | O  .  .  .  .  . |  | X  .  .  .  .  . |
		/// |                  |  |                  |
		/// | X  .  .  .  .  . |  | 2  .  .  .  .  . |
		/// | X  .  .  .  .  . |  | O  .  .  .  .  . |
		/// | X  .  .  .  .  . |  | O  .  .  .  .  . |
		/// | X  .  .  .  O  . |  | O  .  .  .  .  X |
		/// | X  .  .  .  O  . |  | O  .  .  .  .  X |
		/// o------------------o  o------------------o
		///  12 11 10 09 08 07      06 05 04 03 02 01
		/// </remarks>
		public void ToConsole()
		{
			Ident(); Console.WriteLine(" 13 14 15 16 17 18      19 20 21 22 23 24");
			Ident(); Console.WriteLine("o------------------o  o------------------o");
			for(var row = 0; row <= 5; row++)
			{
				Ident();
				Console.Write("|");

				for(var field = 13; field <= 18; field++)
				{
					ValueToConsole(this[field], row);
				}
				Console.Write("|  |");
				for (var field = 19; field <= 24; field++)
				{
					ValueToConsole(this[field], row);
				}
				Console.WriteLine("|");
			}
			for (var row = 5; row >= 0; row--)
			{
				Ident();
				Console.Write("|");

				for (var field = 12; field >= 7; field--)
				{
					ValueToConsole(this[field], row);
				}
				Console.Write("|  |");
				for (var field = 6; field >= 1; field--)
				{
					ValueToConsole(this[field], row);
				}
				Console.WriteLine("|");
			}
			Ident(); Console.WriteLine("o------------------o  o------------------o");
			Ident(); Console.WriteLine(" 12 11 10 09 08 07      06 05 04 03 02 01");
		}

		private void ValueToConsole(BackgammonField field, int row)
		{
			var val = row == 5 ? " " : ".";
			if(field.Stones > row)
			{
				val = "O";
			}
			if(row == 5 && field.Stones > 6)
			{
				val = field.Stones.ToString();
			}
			if(val.Length == 1)
			{
				Console.Write(" ");
			}
			if(val != "." && val != " ")
			{
				if(field.OwnedByX)
				{
					Console.BackgroundColor = ConsoleColor.Red;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Blue;
				}
			}
			Console.Write(val);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(" ");
		}
		private void Ident()
		{
			Console.Write("   ");
		}

		public bool Apply(string move, int dice0, int dice1, bool xToMove)
		{
			XToMove = xToMove;

			// No move provided.
			if (string.IsNullOrEmpty(move)){ return false; }
			
			if (move == BackGammonMove.NoPlay.ToString())
			{
				return ApplyNoPlay(dice0, dice1);
			}

			var moves = new List<BackGammonMove>();
			if(!ParseMoves(move, dice0, dice1, moves))
			{
				return false;
			}

			// Capture.
			foreach(var m in moves)
			{
				this[m.Source].Stones--;
				if (this[m.Target].OwnedByX != xToMove)
				{
					this[m.Target].Stones = 0;
					(xToMove ? BarO : BarX).Stones++;
					this[m.Target].OwnedByX = xToMove;
				}
				// Bear-off
				else if (m.Target == BarXIndex || m.Target == BarOIndex) { /* No adding. */ }
				// Add a stone.
				else
				{
					this[m.Target].Stones++;
					this[m.Target].OwnedByX = xToMove;
				}
			}
			return true;
		}

		private bool ParseMoves(string move, int dice0, int dice1, List<BackGammonMove> moves)
		{
			foreach (var m in move.Split(','))
			{
				var mv = BackGammonMove.Parse(m, XToMove);
				// invalid move.
				if (BackGammonMove.NoPlay.Equals(mv))
				{
					return false;
				}
				moves.Add(mv);
			}
			// To much moves supplied.
			if (moves.Count > (dice0 == dice1 ? 4 : 2))
			{
				return false;
			}
			return true;
		}

		private bool ApplyNoPlay(int dic0, int dice1)
		{
			return true;
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
