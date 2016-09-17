using System;

namespace Arenaii.Backgammon
{
	public class Field
	{
		public Field(int index)
		{
			Index = index;
		}
		public int Index { get; }

		public int Stones { get; set;}
		public bool OwnedByX { get; set; }
		public bool OwnedByO { get { return !OwnedByX; } }

		public bool NotEmpty { get { return !IsEmpty; } }
		public bool IsEmpty { get { return Stones== 0; } }
				

		public void SetX(int stones)
		{
			Stones = stones;
			OwnedByX = true;
		}
		public void SetO(int stones)
		{
			Stones = stones;
			OwnedByX = false;
		}

		public bool IsBlocked(bool xToMove)
		{
			return OwnedByX != xToMove && Stones > 1;
		}
	}
}
