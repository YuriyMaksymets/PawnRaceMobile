using System;
using System.Collections.Generic;
using System.Text;

namespace PawnRaceMobile.Core
{
    public abstract class Player : IPlayer
    {
        private const int c_MaxTotalMoves = 18;
        private const int c_MaxMovesForPawn = 4;

        private readonly int r_MoveShift;
        private readonly int r_PassedY;
        private readonly Random r_Random = new Random();
        private readonly int r_YCannotPass;
        private readonly int r_YForLongMove;
        private Game m_Game;

        internal Player Opponent
        {
            get; private set;
        }

        public Player(Color color)
        {
            Color = color;
            r_YForLongMove = IsWhite ? 1 : Board.c_MAX_INDEX - 1;
            r_YCannotPass = Board.c_MAX_INDEX - r_YForLongMove;
            r_MoveShift = IsWhite ? 1 : -1;
            r_PassedY = IsWhite ? Board.c_MAX_INDEX : 0;
        }

        public event Action<IPlayer, Move> MoveProduced;

        public Board Board => m_Game.Board;

        public Color Color
        {
            get; private set;
        }

        public bool IsWhite => Color == Color.White;

        private IList<Square> Pawns
            => IsWhite ? Board.WhitePawns : Board.BlackPawns;

        public abstract Move ProduceMove();

        public abstract void TakeTurn();

        internal void Set(Game game, Player opponent)
        {
            m_Game = game ?? throw new ArgumentNullException(nameof(game));
            Opponent = opponent ?? throw new ArgumentNullException(nameof(opponent));
        }

        //If there is an obvious move, only it is returned
        internal IList<Move> CalculatePossibleMovesOptimized()
        {
            if (m_Game.NumberOfMoves > 0)
            {
                List<Move> allMoves = new List<Move>(c_MaxTotalMoves);
                for (int i = 0; i < Pawns.Count; i++)
                {
                    Square pawn = Pawns[i];
                    if (pawn.Y == r_YCannotPass)
                    {
                        return new Move[1]
                        {
                            new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + r_MoveShift))
                        };
                    }
                    if (pawn.Y == r_PassedY)
                    {
                        return null;
                    }
                    allMoves.AddRange(GetAvailableMovesForPawn(pawn));
                }
                return allMoves;
            }
            else
            {
                IList<Move> moves = new List<Move>(Board.c_MAX_INDEX << 1);
                Pawns.ForEach(pawn =>
                {
                    moves.Add(new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + r_MoveShift)));
                    moves.Add(new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + (r_MoveShift << 1))));
                });
                return moves;
            }
        }

        protected void OnMoveProduced(Move move) => MoveProduced?.Invoke(this, move);

        protected Move SelectRandomMove(IList<Move> possibleMoves)
            => possibleMoves[r_Random.Next(possibleMoves.Count)];

        public IList<Move> GetAvailableMovesForPawn(Square pawn)
        {
            List<Move> moves = new List<Move>(c_MaxMovesForPawn);
            Move lastMove = m_Game.LastMove;
            bool lastMoveWasLong
                = lastMove != null
                && lastMove.IsLong
                && lastMove.To.IsOccupiedBy(Opponent.Color);

            int newY = pawn.Y + r_MoveShift;
            Square movePosition = Board.GetSquare(pawn.X, newY);
            if (!movePosition.IsOccupied)
            {
                moves.Add(new Move(pawn, movePosition));
                if (pawn.Y == r_YForLongMove)
                {
                    int longMoveY = newY + r_MoveShift;
                    Square longMovePosition = Board.GetSquare(pawn.X, longMoveY);
                    if (!longMovePosition.IsOccupied)
                    {
                        moves.Add(new Move(pawn, longMovePosition));
                    }
                }
            }

            bool epCapturePossible = lastMoveWasLong && lastMove.To.Y == pawn.Y;
            for (int attackShift = -1; attackShift <= 1; attackShift += 2)
            {
                int newX = pawn.X + attackShift;
                if (newX < 0 || newX > Board.c_MAX_INDEX)
                {
                    continue;
                }
                Square newSquare = Board.GetSquare(newX, newY);
                if (newSquare.IsOccupiedBy(Opponent.Color))
                {
                    moves.Add(new Move(pawn, newSquare, true, false));
                }
                else if (epCapturePossible && lastMove.To.X == newX)
                {
                    moves.Add(new Move(pawn, newSquare, true, true));
                }
            }
            return moves;
        }
    }
}