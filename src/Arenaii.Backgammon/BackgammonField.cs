using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arenaii.Backgammon
{
	public class BackgammonField
	{
		public int Stones { get; set;}
		public bool OwnedByX { get; set; }

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
	}
}
