using Arenaii.Backgammon;

namespace Arenaii.UnitTests.Backgammon;

public class BackgammonBoardTest
	{
		[Test]
		public void ToString_Initial()
		{
			var board = new Board();
			var str = board.ToString();
            str.Should().Be("{..}|X2|..|..|..|..|O5|=|..|O3|..|..|..|X5|=|O5|..|..|..|X3|..|=|X5|..|..|..|..|O2|{..} 0-0");
		}
	}
