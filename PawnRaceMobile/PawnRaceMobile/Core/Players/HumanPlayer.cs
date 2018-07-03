using System;

namespace PawnRaceMobile.Core
{
    public class HumanPlayer : Player
    {
        public event Action TurnTaken;

        public HumanPlayer(Color color) : base(color)
        {
        }

        public void ParseMove(Move move) => OnMoveProduced(move);

        public override Move ProduceMove() => throw new NotImplementedException();

        public override void TakeTurn() => TurnTaken?.Invoke();
    }
}