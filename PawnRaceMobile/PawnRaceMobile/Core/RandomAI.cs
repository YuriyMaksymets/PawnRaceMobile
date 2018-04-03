using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    public class RandomAI : Player
    {
        public RandomAI(Color color) : base(color)
        {
        }

        public override Move ProduceMove()
        {
            IList<Move> possibleMoves = CalculatePossibleMovesOptimized();
            if (possibleMoves.Count > 0)
            {
                Move selectedMove = SelectRandomMove(CalculatePossibleMovesOptimized());
                OnMoveProduced(selectedMove);
                return selectedMove;
            }
            else
            {
                return null;
            }
        }

        public override void TakeTurn() => ProduceMove();
    }
}