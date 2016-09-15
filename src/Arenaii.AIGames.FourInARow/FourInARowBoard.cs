using System;
using Arenaii.AIGames.Data;

namespace Arenaii.AIGames.FourInARow
{
	public class FourInARowBoard
	{
		public FourInARowBoard(int columns, int rows)
		{
			Columns = columns;
			Rows = rows;
			Fields = new PlayerName[columns, rows];
		}

		public PlayerName[,] Fields { get; private set; }

		public int Columns { get; private set; }
		public int Rows { get; private set; }
		public bool RedToMove { get; internal set; }
		public BoardState State { get; internal set; }

		internal string GetGameUpdate()
		{
			throw new NotImplementedException();
		}

		internal void ToConsole()
		{
			throw new NotImplementedException();
		}

		internal bool Apply(string move)
		{
			throw new NotImplementedException();
		}
	}
}