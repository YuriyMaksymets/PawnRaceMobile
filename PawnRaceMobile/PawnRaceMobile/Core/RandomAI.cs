using System;

namespace PawnRaceMobile.Core
{
    internal class RandomAI : Player
    {
        public RandomAI(Color color) : base(color)
        {
        }

        public override Move ProduceMove()
        {
            Move selectedMove = SelectRandomMove(CalculatePossibleMovesOptimized());
            OnMoveProduced(selectedMove);
            return selectedMove;
        }

        public override void TakeTurn() => ProduceMove();
    }
}