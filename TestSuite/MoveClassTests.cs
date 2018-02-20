using Microsoft.VisualStudio.TestTools.UnitTesting;
using PawnRaceMobile.Core;

namespace TestSuite
{
    [TestClass]
    public class MoveClassTests
    {
        [TestMethod]
        public void TestGetSAN()
        {
            Square from = new Square(3, 5);
            Square to = new Square(3, 6);

            Move move = new Move(from, to, false, false);
            Assert.AreEqual(move.SAN, "d7");

            move = new Move(from, to, true, false);
            Assert.AreEqual(move.SAN, "dxd7");
        }
    }
}
