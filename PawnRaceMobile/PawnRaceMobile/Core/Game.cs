﻿using System;
using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    public class Game
    {
        private const byte c_MinMovesToFinish = 8;
        private const byte c_MinMovesToFinishByKilling = 14;

        private Stack<Move> m_Moves = new Stack<Move>();

        public Player BlackPlayer
        {
            get; private set;
        }

        public Board Board
        {
            get; private set;
        }

        public Player CurrentPlayer
        {
            get; private set;
        }

        public Color GameResult
        {
            get
            {
                for (int i = 0; i < Board.c_MaxCoord; i++)
                {
                    if (Board.GetSquare(i, 0).IsOccupiedBy(Color.Black))
                    {
                        return Color.Black;
                    }
                    if (Board.GetSquare(i, Board.c_MaxIndex).IsOccupiedBy(Color.White))
                    {
                        return Color.White;
                    }
                }
                if (Board.BlackPawns.Count == 0)
                {
                    return Color.White;
                }
                if (Board.WhitePawns.Count == 0)
                {
                    return Color.Black;
                }
                return Color.None;
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
                for (int i = 0; i < Board.c_MaxCoord; i++)
                {
                    if (Board.GetSquare(i, 0).IsOccupied
                        || Board.GetSquare(i, Board.c_MaxIndex).IsOccupied)
                    {
                        return true;
                    }
                }
                if (CurrentPlayer.CalculatePossibleMovesOptimized().Count < 1)
                {
                    return true;
                }
                if (m_Moves.Count < c_MinMovesToFinishByKilling)
                {
                    return false;
                }
                return Board.BlackPawns.Count == 0 || Board.WhitePawns.Count == 0;
            }
        }

        public Move LastMove
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

        public Player WhitePlayer
        {
            get; private set;
        }

        public Game(char whiteGap, char blackGap, Player player1, Player player2)
        {
            Board = new Board(whiteGap, blackGap);
            if (player1.Color == player2.Color
                || player1.Color == Color.None || player2.Color == Color.None)
            {
                throw new ArgumentException
                    ("Invalid player colors: "
                    + player1.Color.ToString() + player2.Color.ToString());
            }
            WhitePlayer = player1.Color == Color.White ? player1 : player2;
            BlackPlayer = WhitePlayer == player1 ? player2 : player1;
            WhitePlayer.Set(this, BlackPlayer);
            BlackPlayer.Set(this, WhitePlayer);
            CurrentPlayer = WhitePlayer;
        }

        public void ApplyMove(Move move)
        {
            Board.ApplyMove(move);
            m_Moves.Push(move);
            SwitchPlayer();
        }

        public Move ParseMove(string san)
        {
            if (!(san.Length == 4 || san.Length == 2))
            {
                return null;
            }
            char[] sanChars = san.ToCharArray();

            int moveShift = CurrentPlayer.Color == Color.White ? -1 : 1;
            int xIndex = sanChars[1] == 'x' ? 2 : 0;
            int finishX = sanChars[xIndex] - 'a';
            int finishY = sanChars[xIndex + 1] - '1';
            int startY = finishY + moveShift;
            int startX = finishX;

            if (startX < 0 || startX > Board.c_MaxIndex
                || startY < 0 || startY > Board.c_MaxIndex
                || finishX < 0 || finishX > Board.c_MaxIndex
                || finishY < 0 || finishY > Board.c_MaxIndex)
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
                    else if (LastMove != null && LastMove.IsLong
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
                int possibleY = CurrentPlayer.Color == Color.White ? 1 : Board.c_MaxIndex - 1;
                int actualY = finishY + (moveShift << 1);
                if (actualY >= 0 && actualY < Board.c_MaxCoord)
                {
                    startSquare = Board.GetSquare(startX, actualY);
                    if (actualY == possibleY && startSquare.IsOccupiedBy(CurrentPlayer.Color))
                    {
                        return new Move(startSquare, finishSquare);
                    }
                }
            }
            return null;
        }

        public void UnapplyMove(Move move)
        {
            Board.UnapplyMove(move);
            m_Moves.Pop();
            SwitchPlayer();
        }

        private bool SquareOccupiedByCurrentPlayer(int x, int y)
                    => Board.GetSquare(x, y).IsOccupiedBy(CurrentPlayer.Color);

        private bool SquareOccupiedByOtherPlayer(int x, int y)
            => Board.GetSquare(x, y).IsOccupiedBy(CurrentPlayer.Color.Inverse());

        private void SwitchPlayer()
            => CurrentPlayer = CurrentPlayer == BlackPlayer ? WhitePlayer : BlackPlayer;
    }
}