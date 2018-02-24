using System;
using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    public interface IPlayer
    {
        event Action<IPlayer, Move> MoveProduced;

        Color Color
        {
            get;
        }

        Move ProduceMove();

        IList<Move> GetAvailableMovesForPawn(Square pawn);

        void TakeTurn();
    }
}