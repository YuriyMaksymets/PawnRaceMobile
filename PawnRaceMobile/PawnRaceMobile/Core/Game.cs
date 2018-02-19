using System;
using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    internal class Game
    {
        private const byte c_MinMovesToFinish = 8;
        private const byte c_MinMovesToFinishByKilling = 14;

        private Color m_CurrentPlayerColor;
        private Stack<Move> m_Moves = new Stack<Move>();

        public Player WhitePlayer
        {
            get; private set;
        }

        public Player BlackPlayer
        {
            get; private set;
        }

        public Player CurrentPlayer
        {
            get; private set;
        }

        public Game(char whiteGap, char blackGap)
        {
            Board = new Board(whiteGap, blackGap);
        }

        public Board Board
        {
            get; private set;
        }

        public Color GameResult
        {
            get
            {
                for (int i = 0; i < Board.c_MAX_COORDINATE; i++)
                {
                    if (Board.GetSquare(i, 0).IsOccupiedBy(Color.BLACK))
                    {
                        return Color.BLACK;
                    }
                    if (Board.GetSquare(i, Board.c_MAX_INDEX).IsOccupiedBy(Color.WHITE))
                    {
                        return Color.WHITE;
                    }
                }
                if (Board.BlackPawns.Count == 0)
                {
                    return Color.WHITE;
                }
                if (Board.WhitePawns.Count == 0)
                {
                    return Color.BLACK;
                }
                return Color.NONE;
            }
        }

        public bool IsFinished
        {
            get
            {
                if (m_Moves.Count < c_MinMovesToFinish)
                {
                    return false;
                }
                for (int i = 0; i < Board.c_MAX_COORDINATE; i++)
                {
                    if (Board.GetSquare(i, 0).IsOccupied
                        || Board.GetSquare(i, Board.c_MAX_INDEX).IsOccupied)
                    {
                        return true;
                    }
                }
                if (m_Moves.Count < c_MinMovesToFinishByKilling)
                {
                    return false;
                }
                //if (currentPlayer.getPossibleMoves().length < 1)
                //{
                //    return true;
                //}
                return Board.BlackPawns.Count == 0 || Board.WhitePawns.Count == 0;
            }
        }

        public Move? LastMove
        {
            get
            {
                if (m_Moves.Count > 0)
                {
                    return m_Moves.Peek();
                }
                else
                {
                    return null;
                }
            }
        }

        public int NumberOfMoves => m_Moves.Count;

        public void ApplyMove(Move move) => throw new NotImplementedException();

        public Move? ParseMove(string san)
        {
            if (!(san.Length == 4 || san.Length == 2))
            {
                return null;
            }
            char[] sanChars = san.ToCharArray();

            int moveShift = m_CurrentPlayerColor == Color.WHITE ? -1 : 1;
            int xIndex = sanChars[1] == 'x' ? 2 : 0;
            int finishX = sanChars[xIndex] - 'a';
            int finishY = sanChars[xIndex + 1] - '1';
            int startY = finishY + moveShift;
            int startX = finishX;

            if (startX < 0 || startX > Board.c_MAX_INDEX
                || startY < 0 || startY > Board.c_MAX_INDEX
                || finishX < 0 || finishX > Board.c_MAX_INDEX
                || finishY < 0 || finishY > Board.c_MAX_INDEX)
            {
                return null;
            }

            Square finishSquare = Board.GetSquare(finishX, finishY);
            Square startSquare = Board.GetSquare(startX, startY);

            if (sanChars.Length == 4 && sanChars[1] == 'x')
            {
                startX = sanChars[0] - 'a';
                startSquare = Board.GetSquare(startX, startY);
                if (SquareOccupiedByCurrentPlayer(startX, startY))
                {
                    if (SquareOccupiedByOtherPlayer(finishX, finishY))
                    {
                        return new Move(startSquare, finishSquare, true, false);
                    }
                    else if (m_Moves.Peek().IsLong
                      && SquareOccupiedByOtherPlayer(finishX, startY))
                    {
                        return new Move(startSquare, finishSquare, true, true);
                    }
                }
                return null;
            }

            if (SquareOccupiedByCurrentPlayer(startX, startY))
            {
                return new Move(startSquare, finishSquare);
            }
            else
            {
                int possibleY = m_CurrentPlayerColor == Color.WHITE ? 1 : Board.c_MAX_INDEX - 1;
                int actualY = finishY + 2 * moveShift;
                if (actualY >= 0 && actualY < Board.c_MAX_COORDINATE)
                {
                    startSquare = Board.GetSquare(startX, actualY);
                    if (actualY == possibleY && startSquare.IsOccupiedBy(m_CurrentPlayerColor))
                    {
                        return new Move(startSquare, finishSquare);
                    }
                }
            }
            return null;
        }

        public void UnapplyMove() => throw new NotImplementedException();

        private bool SquareOccupiedByCurrentPlayer(int x, int y)
                    => Board.GetSquare(x, y).IsOccupiedBy(m_CurrentPlayerColor);

        private bool SquareOccupiedByOtherPlayer(int x, int y)
            => Board.GetSquare(x, y).IsOccupiedBy(m_CurrentPlayerColor.Inverse());

        private void SwitchPlayer()
            => CurrentPlayer = CurrentPlayer == BlackPlayer ? WhitePlayer : BlackPlayer;
    }
}