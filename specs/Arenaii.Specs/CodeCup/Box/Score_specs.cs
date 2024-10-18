using Arenaii.CodeCup.Box;

namespace Specs.CodeCup.Box;

public class Scores
{
    [TestCase(100, 000, 1.0f)]
    [TestCase(054, 004, 0.8333333f)]
    [TestCase(006, 056, 0.16666667f)]
    [TestCase(020, 020, 0.5f)]
    [TestCase(003, 105, 0.0f)]
    [TestCase(003, 105, 0.0f)]
    public void Fraction(int p1, int p2, float score)
    {
        var player1 = BoxEngine.Score(p1, p2);
        player1.Should().Be(score);

        var player2 = BoxEngine.Score(p2, p1);

        (player1 + player2).Should().Be(1f);
    } 
}
