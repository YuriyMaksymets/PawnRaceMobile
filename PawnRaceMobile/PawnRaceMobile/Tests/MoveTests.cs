using NUnit.Framework;
using PawnRaceMobile.Core;

namespace PawnRaceMobile.Tests
{
    //   [Te]
    public class MoveTests
    {
        [Test]
        public void GetSAN()
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