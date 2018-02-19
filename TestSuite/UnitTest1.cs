using Microsoft.VisualStudio.TestTools.UnitTesting;
using PawnRaceMobile.Core;

namespace TestSuite
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Square from = new Square(3, 5);
            Square to = new Square(3, 6);

            Move move = new Move(from, to, false, false);
            Assert.Equals(move.SAN, "d7");

            move = new Move(from, to, true, false);
            Assert.Equals(move.SAN, "dxd7");
        }
    }
}
