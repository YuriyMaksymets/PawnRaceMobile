using PawnRaceMobile.Core;
using static PawnRaceMobile.BoardPage;

namespace PawnRaceMobile
{
    internal class GameManager
    {
        private Move? m_SelectedMove;
        private Game m_Game;
        public Board Board => m_Game.Board;

        public bool IsValidMove(Move move) => m_Game.ParseMove(move.SAN) != null;

        public void SelectMove(Move move)
        {
            m_SelectedMove = move;
            //if(!m_Game.IsFinished && m_Game.Player = User)
            m_Game.ApplyMove(move);
        }

        public GameManager(char whiteGap, char blackGap)
        {
            m_Game = new Game('a', 'a');
        }

        public void Play()
        {
            m_Game = new Game('a', 'a');
            //while (!game.IsFinished)
            //{
            //    // Player currentPlayer = game.getCurrentPlayerColor() == Color.WHITE ? playerW : playerB;
            //    //  board.display();
            //    //   System.out.println("-------------------------");
            //    //      for (int i = 0; i < 25; i++) {
            //    //        System.out.println();
            //    //      }
            //    //while ()
            //    //    if (!currentPlayer.isComputerPlayer())
            //    //    {
            //    //        while(m_SelectedMove == null) { }
            //    //        Move move;
            //    //        do
            //    //        {
            //    //            System.out.println("Enter your move: ");
            //    //            String input = IOUtil.readString();
            //    //            move = game.parseMove(input);
            //    //        } while (move == null);
            //    //        game.applyMove(move);
            //    //    }
            //    //    else
            //    //    {
            //    //        currentPlayer.makeMove();
            //    //        System.out.println(currentPlayer.getColor().toString() + " : "
            //    //            + game.getLastMove().getSAN());
            //    //    }
            //}
        }
    }
}