using Microsoft.VisualStudio.TestTools.UnitTesting;
using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class EfficiencyTests
    {
        [TestMethod]
        public void MillionRandomGames()
        {
            const int numberOfGames = 200000;
            Player p1 = new RandomAI(Color.White);
            Player p2 = new RandomAI(Color.Black);
            Game game = new Game('c', 'g', p1, p2);
            //watch.Start();
            for (int i = 0; i < numberOfGames; i++)
            {
                while (!game.IsFinished)
                {
                    game.ApplyMove(game.CurrentPlayer.ProduceMove());
                }
                game = new Game('c', 'g', p1, p2);
            }
        }
    }
}