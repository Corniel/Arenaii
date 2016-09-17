using Arenaii.Backgammon;
using NUnit.Framework;
using System;

namespace Arenaii.UnitTests.Backgammon
{
	[TestFixture]
	public class BackgammonBoardTest
	{
		[Test]
		public void ToString_Initial()
		{
			var board = new Board();
			var act = board.ToString();
			var exp = "{..}|X2|..|..|..|..|O5|=|..|O3|..|..|..|X5|=|O5|..|..|..|X3|..|=|X5|..|..|..|..|O2|{..} 0-0";

			Console.WriteLine(act);
			Assert.AreEqual(exp, act);
		}
	}
}
