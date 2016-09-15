using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arenaii.AIGames.UltimateTicTacToe
{
	public class Field
	{
		public Field(int row, int col)
		{
			Row = row;
			Col = col;
		}
		public int Row { get; private set; }
		public int Col { get; private set; }
		public int Value { get; set; }

		public override string ToString() { return Value.ToString(); }
	}
}
