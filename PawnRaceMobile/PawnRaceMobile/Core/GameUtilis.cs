using System;
using System.Collections.Generic;
using System.Text;

namespace PawnRaceMobile.Core
{
    public static class GameUtilis
    {
        public static bool ValidSquare(Square x)
        {
            return (x.X >= 0 && x.X < Board.c_MAX_COORDINATE &&
                x.Y >= 0 && x.Y < Board.c_MAX_COORDINATE);
        }

        public static bool CheckSimpleCapture(Square to, Color playerColor)
            => (to.Color == playerColor.Inverse());

        public static bool CheckEnPassantCapture(Square from, Square to, Board board,
            Color player, Move lastMove)
        {
            return lastMove != null
                && from.Y == lastMove.To.Y
                && to.X == lastMove.To.X
                && board.GetSquare(to.X, from.Y).Color == player.Inverse();
        }

        public static bool CheckForwardMove(Square to)
            => to.Color == Color.None;
    }
}