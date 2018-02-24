using System;
using System.Collections.Generic;
using System.Text;

namespace PawnRaceMobile.Core
{
    public static class GameUtilis
    {
        public static bool ValidSquare(Square x, Board board)
        {
            return (x.X >= 0 && x.X < board.Size &&
                x.Y >= 0 && x.Y < board.Size);
        }

        public static bool CheckSimpleCapture(Square to, Color playerColor)
        {
            return (to.Color == playerColor.Inverse());
        }

        public static bool CheckEnPassantCapture(Square from, Square to, Board board, 
            Color player, Move lastMove)
        {
            if (lastMove == null)
            {
                return false;
            }

            if (from.Y == lastMove.To.Y && to.X == lastMove.To.X)
            {
                if (board.GetSquare(to.X, from.Y).Color == player.Inverse())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckForwardMove(Square to)
        {
            return to.Color == Color.None;
        }

    }
}
