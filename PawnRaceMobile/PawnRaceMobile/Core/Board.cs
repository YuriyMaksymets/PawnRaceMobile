#define debug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PawnRaceMobile.Core
{
    public class Board : ICloneable
    {
        public const byte c_MAX_COORDINATE = 8;
        public const byte c_MAX_INDEX = c_MAX_COORDINATE - 1;

        public byte BlackGapIndex
        {
            get; private set;
        }

        public List<Square> BlackPawns
        {
            get; private set;
        }

        public List<Square> Pawns => WhitePawns.Concat(BlackPawns).ToList();

        public byte WhiteGapIndex
        {
            get; private set;
        }

        public List<Square> WhitePawns
        {
            get; private set;
        }

        private Square[,] Squares
        {
            get; set;
        }

        public Board(char whiteGap, char blackGap)
        {
            //assert(whiteGap >= 'a' && whiteGap <= 'h'
            //    && blackGap >= 'a' && blackGap <= 'h')
            //  : "Wrong gap letter";
            Squares = new Square[c_MAX_COORDINATE, c_MAX_COORDINATE];
            for (int x = 0; x < c_MAX_COORDINATE; x++)
            {
                for (int y = 0; y < c_MAX_COORDINATE; y++)
                {
                    Squares[x, y] = new Square(x, y);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Squares[i, 1].Color = Color.White;
                Squares[i, c_MAX_COORDINATE - 2].Color = Color.Black;
            }

            WhiteGapIndex = (byte)(char.ToLower(whiteGap) - 'a');
            BlackGapIndex = (byte)(char.ToLower(blackGap) - 'a');
            try
            {
                Squares[WhiteGapIndex, 1].Color = Color.None;
                Squares[BlackGapIndex, c_MAX_INDEX - 1].Color = Color.None;
            }
            catch (Exception)
            {
                throw new Exception("Incorrect gaps: " + whiteGap + " " + blackGap);
            }

            BlackPawns = new List<Square>(7);
            WhitePawns = new List<Square>(7);
            for (int i = 0; i < c_MAX_COORDINATE; i++)
            {
                for (int j = 0; j < c_MAX_COORDINATE; j++)
                {
                    if (Squares[i, j].Color == Color.White)
                    {
                        WhitePawns.Add(Squares[i, j]);
                    }
                    else if (Squares[i, j].Color == Color.Black)
                    {
                        BlackPawns.Add(Squares[i, j]);
                    }
                }
            }
        }

        public void AddPawn(Square pawn)
        {
            if (pawn.Color == Color.White)
            {
                WhitePawns.Add(pawn);
            }
            else
            {
                BlackPawns.Add(pawn);
            }
        }

        public void applyMove(Move move)
        {
            Square from = move.From;
            Square to = move.To;
            Color pawnColor = from.Color;
#if debug
            if (pawnColor == Color.None
                || (to.Color == from.Color)
                || (move.IsCapture && !move.IsEpCapture && to.Color != pawnColor.Inverse()))
            {
                Console.WriteLine("Invalid move");
                return;
            }
#endif
            int toX = to.X;
            int toY = to.Y;
            if (move.IsEpCapture)
            {
                int moveShift = pawnColor == Color.White ? -1 : 1;
                Square passedPawn = Squares[toX, toY + moveShift];
                if (passedPawn.Color == pawnColor.Inverse())
                {
                    RemovePawn(passedPawn);
                    passedPawn.Color = Color.None;
                }
                else
                {
                    Console.WriteLine("Invalid move");
                    return;
                }
            }
            else if (move.IsCapture)
            {
                RemovePawn(to);
            }
            RemovePawn(from);
            //throw new Exception((to.Equals(Squares[fromtoX, to.Y])).ToString());
            from.Color = Color.None;
            to.Color = pawnColor;
            AddPawn(to);
        }

        public object Clone() => MemberwiseClone();

        public string Display()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("   A B C D E F G H   ");

            for (int col = c_MAX_COORDINATE - 1; col >= 0; col--)
            {
                string rowString = (col + 1).ToString() + "  ";
                for (int row = 0; row < c_MAX_COORDINATE; row++)
                {
                    rowString += Squares[row, col].Color.Char();
                    rowString += " ";
                }
                rowString += "  " + (col + 1).ToString();
                sb.Append(rowString);
            }

            sb.Append("   A B C D E F G H   ");
            return sb.ToString();
        }

        //assert x<Squares.length && y < Squares[0].length
        //  : "The square does not exist in current Squares";
        public Square GetSquare(int x, int y) => Squares[x, y];

        public string GetState()
        {
            StringBuilder WhitePawns = new StringBuilder(18);
            StringBuilder BlackPawns = new StringBuilder(18);
            for (int tempX = 0; tempX < Squares.Length; tempX++)
            {
                int[] wPawnsYOnThisX = new int[2];
                int wp = 0;
                int[] bPawnsYOnThisX = new int[2];
                int bp = 0;
                for (int tempY = 0; tempY < Squares.GetLength(1); tempY++)
                {
                    if (Squares[tempX, tempY].Color == Color.White)
                    {
                        wPawnsYOnThisX[wp] = tempY;
                        wp++;
                    }
                    else if (Squares[tempX, tempY].Color == Color.Black)
                    {
                        bPawnsYOnThisX[bp] = tempY;
                        bp++;
                    }
                }

                if (wp == 0)
                {
                    WhitePawns.Append('X');
                }
                else if (wp == 2)
                {
                    WhitePawns.Append('d');
                    WhitePawns.Append(wPawnsYOnThisX[0]);
                    WhitePawns.Append(wPawnsYOnThisX[1]);
                }
                else
                {
                    WhitePawns.Append(wPawnsYOnThisX[0]);
                }
                if (bp == 0)
                {
                    BlackPawns.Append('X');
                }
                else if (bp == 2)
                {
                    BlackPawns.Append('d');
                    BlackPawns.Append(bPawnsYOnThisX[0]);
                    BlackPawns.Append(bPawnsYOnThisX[1]);
                }
                else
                {
                    BlackPawns.Append(bPawnsYOnThisX[0]);
                }
            }
            return WhitePawns.ToString() + BlackPawns.ToString();
        }

        public bool IsInState(string state)
        {
            char[] stateC = state.ToCharArray();
            int readIndex = 0;
            for (int tempX = 0; tempX < Squares.Length; tempX++)
            {
                if (stateC[readIndex] == 'd')
                {
                    for (int i = 0; i < 2; i++)
                    {
                        readIndex++;
                        int checkY = stateC[readIndex];
                        if (Squares[tempX, checkY].Color != Color.White)
                        {
                            return false;
                        }
                    }

                    readIndex++;
                }
                else if (stateC[readIndex] == 'X')
                {
                    for (int i = 0; i < c_MAX_COORDINATE; i++)
                    {
                        if (Squares[tempX, i].Color == Color.White)
                        {
                            return false;
                        }
                    }

                    readIndex++;
                }
                else
                {
                    int checkY = stateC[readIndex];
                    if (Squares[tempX, checkY].Color != Color.White)
                    {
                        return false;
                    }

                    for (int i = 0; i < c_MAX_COORDINATE; i++)
                    {
                        if (i != checkY && Squares[tempX, i].Color == Color.White)
                        {
                            return false;
                        }
                    }

                    readIndex++;
                }
            }

            for (int tempX = 0; tempX < Squares.Length; tempX++)
            {
                if (stateC[readIndex] == 'd')
                {
                    for (int i = 0; i < 2; i++)
                    {
                        readIndex++;
                        int checkY = stateC[readIndex];
                        if (Squares[tempX, checkY].Color != Color.Black)
                        {
                            return false;
                        }
                    }

                    readIndex++;
                }
                else if (stateC[readIndex] == 'X')
                {
                    for (int i = 0; i < c_MAX_COORDINATE; i++)
                    {
                        if (Squares[tempX, i].Color == Color.Black)
                        {
                            return false;
                        }
                    }

                    readIndex++;
                }
                else
                {
                    int checkY = stateC[readIndex];
                    if (Squares[tempX, checkY].Color != Color.Black)
                    {
                        return false;
                    }

                    for (int i = 0; i < c_MAX_COORDINATE; i++)
                    {
                        if (i != checkY && Squares[tempX, i].Color == Color.Black)
                        {
                            return false;
                        }
                    }

                    readIndex++;
                }
            }

            return true;
        }

        public void unapplyMove(Move move)
        {
            Square pawn = Squares[move.To.X, move.To.Y];
            Color pawnColor = pawn.Color;
#if debug
            if (pawnColor == Color.None)
            {
                Console.WriteLine("Invalid move");
                return;
            }
#endif
            if (move.IsEpCapture)
            {
                Square to = move.To;
                int toX = to.X;
                int toY = to.Y;
                if (pawnColor == Color.Black)
                {
                    Square tPawn = Squares[toX, toY + 1];
                    tPawn.Color = Color.White;
                    WhitePawns.Add(tPawn);
                    BlackPawns.Remove(pawn);
                    pawn.Color = Color.None;
                }
                else
                {
                    Square tPawn = Squares[toX, toY - 1];
                    tPawn.Color = Color.Black;
                    BlackPawns.Add(tPawn);
                    WhitePawns.Remove(pawn);
                    pawn.Color = Color.None;
                }
            }
            else if (move.IsCapture)
            {
                if (pawnColor == Color.White)
                {
                    pawn.Color = Color.Black;
                    BlackPawns.Add(pawn);
                    WhitePawns.Remove(pawn);
                }
                else
                {
                    pawn.Color = Color.White;
                    WhitePawns.Add(pawn);
                    BlackPawns.Remove(pawn);
                }
            }
            else
            {
                RemovePawn(pawn);
                pawn.Color = Color.None;
            }
            Square from = move.From;
            from.Color = pawnColor;
            AddPawn(from);
        }

        private void RemovePawn(Square pawn)
        {
            if (pawn.Color == Color.White)
            {
                WhitePawns.Remove(pawn);
            }
            else
            {
                BlackPawns.Remove(pawn);
            }
        }
    }
}