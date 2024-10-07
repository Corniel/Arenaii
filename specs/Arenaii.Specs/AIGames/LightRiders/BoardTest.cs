using Arenaii.AIGames.LightRiders;
using NUnit.Framework;
using System;

namespace Arenaii.UnitTests.AIGames.LightRiders
{
    public class BoardTest
    {
        [Test]
        public void GetFieldUpdate_Initial()
        {
            var board = new Board();
            var act = board.GetFieldUpdate();
            var exp = 
                "update game field " +
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+

                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,0,.,.,.,.,.,.,.,.,1,.,.,.,"+

                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+

                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,"+
                ".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.";

            Console.WriteLine(act);
            Assert.AreEqual(exp, act);
        }
    }
}
