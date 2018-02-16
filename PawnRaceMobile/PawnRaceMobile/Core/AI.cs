using System;

namespace PawnRaceMobile.Core
{
    internal abstract class AI
    {
        private Random m_Random = new Random();

        protected Board m_Board;

        public abstract Move SelecteMove(Move[] moves);

        protected Move SelectRandomMove(Move[] possibleMoves)
            => possibleMoves[m_Random.Next(possibleMoves.Length)];
    }
}