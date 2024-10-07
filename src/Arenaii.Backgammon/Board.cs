namespace Arenaii.Backgammon;

public class Board
	{
		private readonly Field[] fields;
		public const int BarXIndex = 0;
		public const int BarOIndex = 25;

		public Board()
		{
			NotFinished = true;
			fields = new Field[26];
			for (var i = 0; i < 26; i++)
			{
				fields[i] = new Field(i);
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

		public Field this[int index] { get { return fields[index]; } }

		public Field BarX { get { return this[BarXIndex]; } }
		public Field BarO { get { return this[BarOIndex]; } }

		public Field Bar { get { return XToMove ? BarX : BarO; } }
		public Field BarOpponent { get { return XToMove ? BarO : BarX; } }

		public int BearOffIndex { get { return XToMove ? BarOIndex : BarXIndex; } }

		public int ScoreX { get { return 15 - fields.Where(field => field.OwnedByX).Sum(field => field.Stones); } }
		public int ScoreO { get { return 15 - fields.Where(field => field.OwnedByO).Sum(field => field.Stones); } }

		public bool CanBearOff { get { return XToMove ? XCanBearOff : OCanBearOff; } }
		public bool XCanBearOff
		{
			get
			{
				return fields.Where(field => field.Index <= 18 && field.OwnedByX).Sum(field => field.Stones) == 0;
			}
		}
		public bool OCanBearOff
		{
			get
			{
				return fields.Where(field => field.Index > 6 && field.OwnedByO).Sum(field => field.Stones) == 0;
			}
		}

		public bool NotFinished { get; private set; }
		public bool XToMove { get; internal set; }
		public bool XIsWinner { get; private set; }

		public int Turn { get; internal set; }

		public int PipX
		{
			get
			{
				var tip = 0;

				for (var i = 0; i <= 24; i++)
				{
					if (this[i].OwnedByX)
					{
						tip += this[i].Stones * (25 - i);
					}
				}
				return tip;
			}
		}
		public int PipO
		{
			get
			{
				var tip = 0;

				for (var i = 25; i >= 1; i--)
				{
					if (this[i].OwnedByO)
					{
						tip += this[i].Stones * i;
					}
				}
				return tip;
			}
		}


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
		public void ToConsole(int dice0, int dice1)
		{
			Ident(); Console.WriteLine(" 13 14 15 16 17 18      19 20 21 22 23 24");
			Ident(); Console.WriteLine("o------------------o  o------------------o");
			for (var row = 0; row <= 5; row++)
			{
				Ident();
				Console.Write("|");

				for (var field = 13; field <= 18; field++)
				{
					ValueToConsole(this[field], row);
				}
				Console.Write("|  |");
				for (var field = 19; field <= 24; field++)
				{
					ValueToConsole(this[field], row);
				}
				Console.Write("|");
				ValueToConsole(BarO, row, true);
				Console.WriteLine();
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
				Console.Write("|");
				ValueToConsole(BarX, row, true);
				Console.WriteLine();
			}
			Ident(); Console.WriteLine("o------------------o  o------------------o");
			Ident(); Console.WriteLine(" 12 11 10 09 08 07      06 05 04 03 02 01");
			Console.WriteLine();
			Ident(); Console.WriteLine("                 [{0}]  [{1}]", Math.Max(dice0, dice1), Math.Min(dice0, dice1));
			Console.WriteLine();
			RenderPip(PipX, ConsoleColor.Red, XToMove);
			RenderPip(PipO, ConsoleColor.Blue, !XToMove);
			Console.WriteLine();
			Ident(); RenderScore(ScoreX, ConsoleColor.Red);
			Ident(); RenderScore(ScoreO, ConsoleColor.Blue);
			Console.WriteLine();
		}

		private void ValueToConsole(Field field, int row, bool isBar = false)
		{
			var val = row == 5 || isBar ? " " : ".";
			if (field.Stones > row)
			{
				val = "O";
			}
			if (row == 5 && field.Stones > 6)
			{
				val = field.Stones.ToString();
			}
			if (val.Length == 1)
			{
				Console.Write(" ");
			}
			if (val != "." && val != " ")
			{
				if (field.OwnedByX)
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
			Console.Write("  ");
		}

		private void RenderPip(int pip, ConsoleColor color, bool toMove)
		{
			if (toMove)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("* ");
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			else
			{
				Console.Write("  ");
			}

			var value = pip.ToString();
			var length = Math.Max(0, pip / 3 - value.Length);
			Console.BackgroundColor = color;
			Console.Write(value);
			Console.Write(new string(' ', length));
			Console.BackgroundColor = ConsoleColor.Black;

			Console.WriteLine();
		}

		private void RenderScore(int score, ConsoleColor color)
		{
			if (score == 0)
			{
				Console.ForegroundColor = color;
				Console.WriteLine(score);
				Console.ForegroundColor = ConsoleColor.Gray;
				return;
			}

			var value = score.ToString();
			var length = Math.Max(0, score - value.Length);
			Console.BackgroundColor = color;
			Console.Write(value);
			Console.Write(new string(' ', length));
			Console.BackgroundColor = ConsoleColor.Black;
			Console.WriteLine();
		}

		public void CheckFinished()
		{
			if (ScoreX == 15)
			{
				XIsWinner = true;
				NotFinished = false;
			}
			else if (ScoreO == 15)
			{
				XIsWinner = false;
				NotFinished = false;
			}
		}
		public MoveResult Apply(string move, int dice0, int dice1)
		{
			// No move provided.
			if (string.IsNullOrEmpty(move)) { return MoveResult.EmptyMove; }

			if (move == Move.NoPlay.ToString())
			{
				return ApplyNoPlay(dice0, dice1);
			}

			var moves = new List<Move>();
			var result = ParseMoves(move, dice0, dice1, moves);
			if (result != MoveResult.Ok) { return result; }

			foreach (var m in moves)
			{
				result = Apply(m);
				if (result != MoveResult.Ok)
				{
					return result;
				}
			}
			return result;
		}

		private MoveResult Apply(Move move)
		{
			var source = this[move.Source];
			var target = this[move.Target];

			if (Bar.NotEmpty && move.Source != Bar.Index)
			{
				return MoveResult.MustMoveFromBarFirst;
			}
			if (source.IsEmpty)
			{
				return MoveResult.NoStoneOnSourceField;
			}
			if (target.Index == BearOffIndex && !CanBearOff)
			{
				return MoveResult.BearOffIsNotAllowed;
			}
			// Bear off is never blocked.
			if (target.Index != BearOffIndex && target.IsBlocked(XToMove))
			{
				return MoveResult.TargetFieldIsBlocked;
			}

			source.Stones--;
			if (target.Index != BearOffIndex)
			{
				// Add stone to bar of the opponent.
				if (target.NotEmpty && target.OwnedByO == XToMove)
				{
					BarOpponent.Stones++;
				}
				else
				{
					target.Stones++;
				}

				// Set new owner.
				target.OwnedByX = XToMove;
			}
			else { /* just remove the stone from the field. */ }
			return MoveResult.Ok;
		}

		private MoveResult ParseMoves(string str, int dice0, int dice1, List<Move> moves)
		{
			var dices = new List<int>() { dice0, dice1 };
			if (dice0 == dice1)
			{
				dices.Add(dice0);
				dices.Add(dice0);
			}
			foreach (var move in str.Split(','))
			{
				var mv = Move.Parse(move, XToMove);
				// invalid move.
				if (Move.NoPlay.Equals(mv))
				{
					return MoveResult.UnparsebleMove;
				}
				moves.Add(mv);
			}
			// To much moves supplied.
			if (moves.Count > dices.Count)
			{
				return MoveResult.TooManyMoves;
			}

			// Validate dice/move mapping.
			foreach (var move in moves)
			{
				var dice = dices.FirstOrDefault(d => move.IsValid(d, XToMove));
				if (dice == 0)
				{
					return MoveResult.MoveDoesNotMatchDice;
				}
				dices.Remove(dice);
			}

			return MoveResult.Ok;
		}

		private MoveResult ApplyNoPlay(int dic0, int dice1)
		{
			return MoveResult.Ok;
		}

		public string GetGameUpdate()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("update game board {0}", ToString()).AppendLine();
			sb.AppendFormat("update game turn {0}", Turn);
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
