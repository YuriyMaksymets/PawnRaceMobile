using PawnRaceMobile.Core;

namespace PawnRaceMobile
{
    internal class GameManager
    {
        private Move? m_SelectedMove;
        private Game m_Game;
        public Board Board => m_Game.Board;

        public delegate void BoardUpdateEventHandler();

        public event BoardUpdateEventHandler OnBoardUpdate;

        public bool IsValidMove(Move move) => m_Game.ParseMove(move.SAN) != null;

        public void SelectMove(Player player, Move move)
        {
            m_SelectedMove = move;
            if (m_Game.CurrentPlayer == player && !m_Game.IsFinished)
            {
                m_Game.ApplyMove(move);
                OnBoardUpdate?.Invoke();
            }
        }

        public GameManager(char whiteGap, char blackGap
            , Player whitePlayer, Player blackPlayer)
        {
            m_Game = new Game(whiteGap, blackGap);
        }

        public void Play()
        {
            //m_Game = new Game('a', 'a');
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