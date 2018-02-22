using System;

namespace PawnRaceMobile.Core
{
    internal interface IPlayer
    {
        event Action<Player, Move> MoveProduced;

        Move ProduceMove();

        void TakeTurn();

        Color GetColor();
    }
}