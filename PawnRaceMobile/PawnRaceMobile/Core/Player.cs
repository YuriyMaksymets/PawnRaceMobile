using System;
using System.Collections.Generic;
using System.Text;

namespace PawnRaceMobile.Core
{
    internal abstract class Player
    {
        private readonly int r_MoveShift;
        private readonly int r_PassedY;
        private readonly Random r_Random = new Random();
        private readonly int r_YCannotAttack;
        private readonly int r_YForLongMove;
        private Game m_Game;
        private Player m_Opponent;

        public Board Board => m_Game.Board;

        public Color Color
        {
            get; private set;
        }

        public bool IsWhite => Color == Color.White;

        private List<Square> Pawns
            => Color == Color.White ? m_Game.Board.WhitePawns : m_Game.Board.BlackPawns;

        public event Action<Player, Move> MoveProduced;

        public Player(Color color)
        {
            Color = color;
            r_YForLongMove = IsWhite ? 1 : Board.c_MAX_INDEX - 1;
            r_YCannotAttack = Board.c_MAX_INDEX - r_YForLongMove;
            r_MoveShift = IsWhite ? 1 : -1;
            r_PassedY = IsWhite ? Board.c_MAX_INDEX : 0;
        }

        public abstract Move ProduceMove();

        public void Set(Game game, Player opponent)
        {
            m_Game = game ?? throw new ArgumentNullException(nameof(game));
            m_Opponent = opponent ?? throw new ArgumentNullException(nameof(opponent));
        }

        public abstract void TakeTurn();

        //If there is an obvious move, only it is returned
        protected Move[] CalculatePossibleMovesOptimized()
        {
            if (m_Game.NumberOfMoves > 0)
            {
                Move[] maxMovesArray = new Move[18];
                Move lastMove = m_Game.LastMove.Value;
                bool lastMoveWasLong = lastMove.IsLong && lastMove.To.IsOccupiedBy(Color.Inverse());
                int numOfMoves = 0;
                for (int i = 0; i < Pawns.Capacity; i++)
                {
                    Square pawn = Pawns[i];
                    int currentY = pawn.Y;
                    int currentX = pawn.X;
                    if (currentY == r_YCannotAttack)
                    {
                        return new Move[1]
                        {
                            new Move(pawn, Board.GetSquare(currentX, currentY + r_MoveShift))
                        };
                    }
                    if (currentY == r_PassedY)
                    {
                        return null;
                    }

                    int newY = currentY + r_MoveShift;
                    Square movePosition = Board.GetSquare(currentX, newY);
                    if (!movePosition.IsOccupied)
                    {
                        maxMovesArray[numOfMoves++] = new Move(pawn, movePosition);
                        if (currentY == r_YForLongMove)
                        {
                            int longMoveY = newY + r_MoveShift;
                            Square longMovePosition = Board.GetSquare(currentX, longMoveY);
                            if (!longMovePosition.IsOccupied)
                            {
                                maxMovesArray[numOfMoves++] = new Move(pawn, longMovePosition);
                            }
                        }
                    }

                    bool EpCapturePossible = lastMoveWasLong && lastMove.To.Y == currentY;
                    for (int attackShift = -1; attackShift <= 1; attackShift += 2)
                    {
                        int newX = currentX + attackShift;
                        if (newX < 0 || newX > Board.c_MAX_INDEX)
                        {
                            continue;
                        }
                        Square newSquare = Board.GetSquare(newX, newY);
                        if (newSquare.IsOccupiedBy(Color.Inverse()))
                        {
                            maxMovesArray[numOfMoves++] = new Move(pawn, newSquare, true, false);
                        }
                        else if (EpCapturePossible && lastMove.To.X == newX)
                        {
                            maxMovesArray[numOfMoves++] = new Move(pawn, newSquare, true, true);
                        }
                    }
                }
                Move[] moves = new Move[numOfMoves];
                Array.Copy(maxMovesArray, moves, numOfMoves);
                return moves;
            }
            else
            {
                Move[] moves = new Move[14];
                int numOfMoves = 0;
                Pawns.ForEach(pawn =>
                {
                    moves[numOfMoves++]
                    = new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + r_MoveShift));
                    moves[numOfMoves++]
                        = new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + (r_MoveShift >> 1)));
                });
                return moves;
            }
        }

        protected void OnMoveProduced(Move move)
                                                    => MoveProduced?.Invoke(this, move);

        protected Move SelectRandomMove(Move[] possibleMoves)
            => possibleMoves[r_Random.Next(possibleMoves.Length)];
    }
}