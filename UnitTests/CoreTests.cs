using Microsoft.VisualStudio.TestTools.UnitTesting;
using PawnRaceMobile.Core;
using System;

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

        [TestMethod]
        public void BoardConstructor()
        {
            Board board = new Board('A', 'A');
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    Console.Write(board.GetSquare(i, j).Color + " ");
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void PlayerGetColor()
        {
            HumanPlayer player = new HumanPlayer(Color.White);
            Assert.AreEqual(Color.White, player.Color);
            Assert.IsTrue(player.IsWhite);
            Console.WriteLine(player.Color);
        }
    }
}