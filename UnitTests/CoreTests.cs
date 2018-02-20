using Microsoft.VisualStudio.TestTools.UnitTesting;
using PawnRaceMobile.Core;

namespace UnitTests
{
    [TestClass]
    public class CoreTests
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

        [TestMethod]
        public void SquareNotation()
        {
            Square square = new Square(1, 1);
            Assert.AreEqual(square.Notation, "b2");

            square = new Square(7, 5);
            Assert.AreEqual(square.Notation, "h6");

            square = new Square(3, 7);
            Assert.AreEqual(square.Notation, "d8");
        }
    }
}