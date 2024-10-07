using Arenaii.AIGames.UltimateTicTacToe;
using NUnit.Framework;

namespace Arenaii.UnitTests.AIGames.UltimateTicTacToe
{
    public class UltimateTicTacToeEngineTest
	{
		[Test]
		public void Apply_20_()
		{
			var board = new MetaBoard();
			board.Apply("place_move 2 0");

			Assert.AreEqual(
				"0,0,1,0,0,0,0,0,0," +
				"0,0,0,0,0,0,0,0,0," +
				"0,0,0,0,0,0,0,0,0," +

				"0,0,0,0,0,0,0,0,0," +
				"0,0,0,0,0,0,0,0,0," +
				"0,0,0,0,0,0,0,0,0," +

				"0,0,0,0,0,0,0,0,0," +
				"0,0,0,0,0,0,0,0,0," +
				"0,0,0,0,0,0,0,0,0", board.ToString());
			Assert.AreEqual("0,0,-1,0,0,0,0,0,0", board.ToMacro());
		}

		[Test]
		public void ToIndex_All_AreEqual()
		{
			var exp = new int[,]
			{
				{ 00, 01, 02, 03, 04, 05, 06, 07, 08 },
				{ 09, 10, 11, 12, 13, 14, 15, 16, 17 },
				{ 18, 19, 20, 21, 22, 23, 24, 25, 26 },

				{ 27, 28, 29, 30, 31, 32, 33, 34, 35 },
				{ 36, 37, 38, 39, 40, 41, 42, 43, 44 },
				{ 45, 46, 47, 48, 49, 50, 51, 52, 53 },

				{ 54, 55, 56, 57, 58, 59, 60, 61, 62 },
				{ 63, 64, 65, 66, 67, 68, 69, 70, 71 },
				{ 72, 73, 74, 75, 76, 77, 78, 79, 80 },
			};

			for (var row = 0; row < 9; row++)
			{
				for (var col = 0; col < 9; col++)
				{
					Assert.AreEqual(exp[row, col], MetaBoard.ToIndex(row, col), "[{0},{1}]", row, col);
				}
			}
		}

		[Test]
		public void ToTiny_All_AreEqual()
		{
			var exp = new int[,]
			{
				{  0, 0, 0, 1, 1, 1, 2, 2, 2, },
				{  0, 0, 0, 1, 1, 1, 2, 2, 2, },
				{  0, 0, 0, 1, 1, 1, 2, 2, 2, },

				{  3, 3, 3, 4, 4, 4, 5, 5, 5, },
				{  3, 3, 3, 4, 4, 4, 5, 5, 5, },
				{  3, 3, 3, 4, 4, 4, 5, 5, 5, },

				{  6, 6, 6, 7, 7, 7, 8, 8, 8, },
				{  6, 6, 6, 7, 7, 7, 8, 8, 8, },
				{  6, 6, 6, 7, 7, 7, 8, 8, 8, },
			};

			for (var row = 0; row < 9; row++)
			{
				for (var col = 0; col < 9; col++)
				{
					Assert.AreEqual(exp[row, col], MetaBoard.ToTiny(row, col), "[{0},{1}]", row, col);
				}
			}
		}

		[Test]
		public void ToTinyIndex_All_AreEqual()
		{
			var exp = new int[]
			{
				0, 1, 2, 0, 1, 2, 0, 1, 2,
				3, 4, 5, 3, 4, 5, 3, 4, 5,
				6, 7, 8, 6, 7, 8, 6, 7, 8,

				0, 1, 2, 0, 1, 2, 0, 1, 2,
				3, 4, 5, 3, 4, 5, 3, 4, 5,
				6, 7, 8, 6, 7, 8, 6, 7, 8,

				0, 1, 2, 0, 1, 2, 0, 1, 2,
				3, 4, 5, 3, 4, 5, 3, 4, 5,
				6, 7, 8, 6, 7, 8, 6, 7, 8,
			};

			for (var index = 0; index < 81; index++)
			{
				Assert.AreEqual(exp[index], MetaBoard.ToTinyIndex(index), "[{0}]", index);
			}
		}
	}
}
