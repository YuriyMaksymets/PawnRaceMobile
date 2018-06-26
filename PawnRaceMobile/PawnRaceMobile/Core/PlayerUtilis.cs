using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    internal static class PlayerUtilis
    {
        public static bool simpleCapture(Square to, Color player)
        {
            return (to.Color == BoardUtilis.enemyColor(player));
        }

        public static bool enPassantCapture(Square from, Square to, Board board, Color player,
            Move move)
        {
            if (move == null)
            {
                return false;
            }

            if (from.Y == move.To.Y && to.X == move.To.X)
            {
                if (board.GetSquare(from.Y, to.X).Color == BoardUtilis
                    .enemyColor(player))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool forwardMove(Square to)
        {
            return to.Color == Color.None;
        }

        public static int getDistToFinal(Square x, Color player, Board board)
        {
            if (player == Color.White)
            {
                return board.Size - 1 - x.Y;
            }

            return x.Y;
        }

        private static bool checkRank(int rank, Square pawn, Board board, Color color)
        {
            //its file
            if (board.GetSquare(rank, pawn.X).Color != Color.None)
            {
                return false;
            }

            //left and right
            if (pawn.X > 0)
            {
                if (board.GetSquare(rank, pawn.X - 1).Color == BoardUtilis.enemyColor(color))
                {
                    return false;
                }
            }

            if (pawn.X < board.Size - 1)
            {
                if (board.GetSquare(rank, pawn.X + 1).Color == BoardUtilis.enemyColor(color))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool isPassedPawn(Square pawn, Board board, Color color)
        {
            if (color == Color.White)
            {
                for (int rank = pawn.Y + 1; rank < board.Size; ++rank)
                {
                    if (!checkRank(rank, pawn, board, color))
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int rank = pawn.Y - 1; rank >= 0; --rank)
                {
                    if (!checkRank(rank, pawn, board, color))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void initPawnChains(int[][] a)
        {
            for (int i = 0; i < a.Length; ++i)
            {
                for (int j = 0; j < a[0].Length; ++j)
                {
                    a[i][j] = 0;
                }
            }
        }

        public static void computePawnChainsForRank(int[][] a, Board board, int rank, Color color)
        {
            int colorType = -1;
            if (color == Color.Black)
            {
                colorType = 1;
            }

            for (int file = 1; file < board.Size - 1; ++file)
            {
                if (board.GetSquare(rank + colorType, file - 1).Color == color)
                {
                    if (a[rank + colorType][file - 1] == 0)
                    {
                        a[rank + colorType][file - 1] = 1;
                    }
                    a[rank][file] = a[rank + colorType][file - 1] + 1;
                }

                if (board.GetSquare(rank + colorType, file + 1).Color == color)
                {
                    if (a[rank + colorType][file + 1] == 0)
                    {
                        a[rank + colorType][file + 1] = 1;
                    }
                    a[rank][file] = a[rank - 1][file + 1] + 1;
                }
            }
        }

        public static int computeScorePwnChains(int[][] a, int size)
        {
            int sum = 0;

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    sum += a[i][j];
                }
            }

            return sum;
        }

        public static List<Move> getValidMovesForPawn(Color player, Square pawn, Board board, Game game)
        {
            List<Move> listOfMoves = new List<Move>();
            Move move;
            int colorType = 1;
            if (player == Color.Black)
            {
                colorType = -1;
            }
            Square nextSquare;

            nextSquare = new Square(pawn.Y + colorType, pawn.X - 1);
            if (GameUtilis.ValidSquare(nextSquare))
            {
                nextSquare = board.GetSquare(nextSquare.Y, nextSquare.X);

                //Check if simple capture is possible
                if (simpleCapture(nextSquare, player))
                {
                    listOfMoves.Add(new Move(pawn, nextSquare, true, false));
                }
                else
                {
                    //Check if enPassant capture
                    move = game.LastMove;
                    if (move != null && move.IsLong)
                    {
                        if (enPassantCapture(pawn, nextSquare, board, player, move))
                        {
                            listOfMoves.Add(new Move(pawn, nextSquare, true, true));
                        }
                    }
                }
            }

            nextSquare = new Square(pawn.Y + colorType, pawn.X + 1);
            if (GameUtilis.ValidSquare(nextSquare))
            {
                nextSquare = board.GetSquare(nextSquare.Y, nextSquare.X);

                //Check if simple capture is possible
                if (simpleCapture(nextSquare, player))
                {
                    listOfMoves.Add(new Move(pawn, nextSquare, true, false));
                }
                else
                {
                    //Check if enPassant capture
                    move = game.LastMove;
                    if (move != null && move.IsLong)
                    {
                        if (enPassantCapture(pawn, nextSquare, board, player, move))
                        {
                            listOfMoves.Add(new Move(pawn, nextSquare, true, true));
                        }
                    }
                }
            }

            //Check for one step forward move
            nextSquare = new Square(pawn.Y + colorType, pawn.X);
            if (GameUtilis.ValidSquare(nextSquare))
            {
                nextSquare = board.GetSquare(nextSquare.Y, nextSquare.X);

                if (forwardMove(nextSquare))
                {
                    //Check for two steps forward move
                    Square nextSquare2 = new Square(pawn.Y + 2 * colorType, pawn.X);
                    if ((player == Color.White && pawn.Y == 1) || (player == Color.Black
                        && pawn.Y == board.Size - 2))
                    {
                        if (GameUtilis.ValidSquare(nextSquare2))
                        {
                            nextSquare2 = board.GetSquare(nextSquare2.Y, nextSquare2.X);

                            if (forwardMove(nextSquare2))
                            {
                                listOfMoves.Add(new Move(pawn, nextSquare2, false, false));
                            }
                        }
                    }
                    listOfMoves.Add(new Move(pawn, nextSquare, false, false));
                }
            }

            return listOfMoves;
        }

        public static bool emptyFile(int rank, int file, Color player, Board board)
        {
            if (player == Color.White)
            {
                for (int i = rank + 1; i < board.Size; ++i)
                {
                    if (board.GetSquare(i, file).Color != Color.None)
                    {
                        return false;
                    }
                }
                return true;
            }

            for (int i = rank - 1; i >= 0; --i)
            {
                if (board.GetSquare(i, file).Color != Color.None)
                {
                    return false;
                }
            }
            return true;
        }

        private static int capturedFileStructure(Square pawn, Board board)
        {
            int structureScore = 0;
            int pawnY = pawn.Y;
            int pawnX = pawn.X;
            Color pColor = pawn.Color;

            if (pawnX > 0)
            {
                if (board.GetSquare(pawnY, pawnX - 1).Color == pColor)
                {
                    structureScore += 3;
                }
                if (pawnY > 0)
                {
                    if (board.GetSquare(pawnY - 1, pawnX - 1).Color == pColor)
                    {
                        structureScore += 3;
                    }
                }
                if (pawnY < board.Size - 1)
                {
                    if (board.GetSquare(pawnY + 1, pawnX - 1).Color == pColor)
                    {
                        structureScore += 3;
                    }
                }
            }

            if (pawnX < board.Size - 1)
            {
                if (board.GetSquare(pawnY, pawnX + 1).Color == pColor)
                {
                    structureScore += 3;
                }
                if (pawnY > 0)
                {
                    if (board.GetSquare(pawnY - 1, pawnX + 1).Color == pColor)
                    {
                        structureScore += 3;
                    }
                }
                if (pawnY < board.Size - 1)
                {
                    if (board.GetSquare(pawnY + 1, pawnX + 1).Color == pColor)
                    {
                        structureScore += 4;
                    }
                }
            }

            return structureScore;
        }

        public static int capturedFileScore(Board board, int file, Color player, Color myPlayer)
        {
            bool capturedFile = true;
            bool hasPawn = false;
            int capturedFileScore = 0;
            for (int rank = 0; rank < board.Size; ++rank)
            {
                if (board.GetSquare(rank, file).Color == BoardUtilis.enemyColor(player))
                {
                    capturedFile = false;
                }
                if (board.GetSquare(rank, file).Color == player)
                {
                    if (player == Color.White && rank != 1)
                    {
                        hasPawn = true;
                    }
                    else if (player == Color.Black && rank != 6)
                    {
                        hasPawn = true;
                    }
                }
            }

            if (capturedFile && hasPawn)
            {
                capturedFileScore += 2;
                for (int rank = 0; rank < board.Size; ++rank)
                {
                    if (board.GetSquare(rank, file).Color == player)
                    {
                        capturedFileScore = capturedFileStructure(board.GetSquare(rank, file), board) * 4;
                        if (capturedFileScore > 0)
                        {
                            if (player == Color.White)
                            {
                                capturedFileScore += rank * 3 + 5;
                            }
                            else
                            {
                                capturedFileScore += (board.Size - rank - 1) * 3;
                            }
                        }
                    }
                }
            }

            return capturedFileScore * 2;
        }

        public static Square leadingPawn(Color pawnColor, Board board)
        {
            if (pawnColor == Color.Black)
            {
                for (int rank = 1; rank < board.Size - 2; ++rank)
                {
                    for (int file = 0; file < board.Size; ++file)
                    {
                        if (board.GetSquare(rank, file).Color == pawnColor)
                        {
                            return board.GetSquare(rank, file);
                        }
                    }
                }
            }

            for (int rank = board.Size - 2; rank >= 0; --rank)
            {
                for (int file = 0; file < board.Size; ++file)
                {
                    if (board.GetSquare(rank, file).Color == pawnColor)
                    {
                        return board.GetSquare(rank, file);
                    }
                }
            }

            return null;
        }

        public static int blockingPawns(Color color, Color playerToMove, IList<Square> playerPawns,
            int[][] whitePawnChains, int[][] blackPawnChains, Board board, Game game)
        {
            int pawnValue;
            List<Move> validMovesForPawn;
            int score = 0;
            for (int i = 0; i < playerPawns.Count; ++i)
            {
                pawnValue = 0;
                if (color == Color.Black)
                {
                    if (playerPawns[i].Y - 1 >= 0)
                    {
                        pawnValue = whitePawnChains[playerPawns[i].Y - 1][playerPawns[i]
                            .X];
                    }
                    if (pawnValue > 0)
                    {
                        score += (pawnValue);
                    }
                }
                else
                {
                    if (playerPawns[i].Y + 1 < board.Size)
                    {
                        pawnValue = blackPawnChains[playerPawns[i].Y - 1][playerPawns[i]
                            .X];
                    }
                    if (pawnValue > 0)
                    {
                        score += (pawnValue - 1);
                    }
                }

                validMovesForPawn = getValidMovesForPawn(color, playerPawns[i], board, game);
                foreach (Move move in validMovesForPawn)
                {
                    if (move.IsCapture)
                    {
                        if (color == Color.White)
                        {
                            pawnValue = blackPawnChains[move.To.Y][move.To.X];
                        }
                        else
                        {
                            pawnValue = whitePawnChains[move.To.Y][move.To.X];
                        }

                        if (playerToMove == color && pawnValue > 0)
                        {
                            score += (10 - pawnValue);
                        }
                        else
                        {
                            score -= (10 - pawnValue);
                        }
                    }
                }
            }

            return score;
        }
    }
}