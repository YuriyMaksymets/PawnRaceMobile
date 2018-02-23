using System;

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

        void TakeTurn();
    }
}