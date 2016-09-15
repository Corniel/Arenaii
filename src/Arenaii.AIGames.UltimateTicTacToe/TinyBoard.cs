using System.Linq;

namespace Arenaii.AIGames.UltimateTicTacToe
{
	public class TinyBoard
	{
		private readonly Field[] fields = new Field[9];

		public Field this[int index]
		{
			get
			{
				return fields[index];
			}
			set
			{
				fields[index] = value;
			}
		}

		public override string ToString()
		{
			return string.Join(",", fields.Select(f => f.Value));
		}

		public BoardState State
		{
			get
			{
				foreach (var score in Score.All)
				{
					if (score.All(index => fields[index].Value == 1)) { return BoardState.Player1; }
					if (score.All(index => fields[index].Value == 2)) { return BoardState.Player2; }
				}
				return fields.All(field => field.Value > 0) ? BoardState.Draw : BoardState.None;
			}
		}
		
	}
}
