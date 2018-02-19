using System;
using System.Collections.Generic;
using System.Text;

namespace PawnRaceMobile.Core
{
    public class Player
    {
        private readonly byte r_PassedY;
        private readonly int r_MoveShift;
        private readonly byte r_YForLongMove;
        private readonly byte r_YCannotAttack;

        private Game m_Game;
        private Player m_Opponent;

        public Color Color
        {
            get; private set;
        }

        public Board Board => m_Game.Board;

        private List<Square> Pawns
            => Color == Color.WHITE ? m_Game.Board.WhitePawns : m_Game.Board.BlackPawns;

        //If there is an obvious move, only it is returned
        private Move[] CalculatePossibleMovesOptimized()
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
                        = new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + (r_MoveShift >> 2)));
                });
                return moves;
            }
        }
    }
}