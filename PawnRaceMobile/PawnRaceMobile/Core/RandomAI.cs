namespace PawnRaceMobile.Core
{
    internal class RandomAI : AI
    {
        public override Move SelecteMove(Move[] moves) => SelectRandomMove(moves);
    }
}