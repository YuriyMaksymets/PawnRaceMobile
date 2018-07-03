using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    internal static class PlayerUtilis
    {
        public static bool SimpleCapture(Square to, Color player)
            => (to.Color == BoardUtilis.EnemyColor(player));

        public static bool EnPassantCapture(Square from, Square to, Board board, Color player,
            Move move)
        {
            if (move == null)
            {
                return false;
            }

            if (from.Y == move.To.Y && to.X == move.To.X)
            {
                if (board.GetSquare(from.X, to.Y).Color == BoardUtilis
                    .EnemyColor(player))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ForwardMove(Square to) => to.Color == Color.None;

        public static int DistanceToFinal(Square x, Color player, Board board)
        {
            if (player == Color.White)
            {
                return board.Size - 1 - x.Y;
            }

            return x.Y;
        }

        private static bool CheckRank(int rank, Square pawn, Board board, Color color)
        {
            //its file
            if (board.GetSquare(pawn.X, rank).Color != Color.None)
            {
                return false;
            }

            //left and right
            if (pawn.X > 0)
            {
                if (board.GetSquare(pawn.X - 1, rank).Color == BoardUtilis.EnemyColor(color))
                {
                    return false;
                }
            }

            if (pawn.X < board.Size - 1)
            {
                if (board.GetSquare(pawn.X + 1, rank).Color == BoardUtilis.EnemyColor(color))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsPassedPawn(Square pawn, Board board, Color color)
        {
            if (color == Color.White)
            {
                for (int rank = pawn.Y + 1; rank < board.Size; ++rank)
                {
                    if (!CheckRank(rank, pawn, board, color))
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int rank = pawn.Y - 1; rank >= 0; --rank)
                {
                    if (!CheckRank(rank, pawn, board, color))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void InitPawnChains(int[][] a)
        {
            for (int i = 0; i < a.Length; ++i)
            {
                for (int j = 0; j < a[0].Length; ++j)
                {
                    a[i][j] = 0;
                }
            }
        }

        public static void ComputePawnChainsForRank(int[][] a, Board board, int rank, Color color)
        {
            int colorType = -1;
            if (color == Color.Black)
            {
                colorType = 1;
            }

            for (int file = 1; file < board.Size - 1; ++file)
            {
                if (board.GetSquare(file - 1, rank + colorType).Color == color)
                {
                    if (a[rank + colorType][file - 1] == 0)
                    {
                        a[rank + colorType][file - 1] = 1;
                    }
                    a[rank][file] = a[rank + colorType][file - 1] + 1;
                }

                if (board.GetSquare(file + 1, rank + colorType).Color == color)
                {
                    if (a[rank + colorType][file + 1] == 0)
                    {
                        a[rank + colorType][file + 1] = 1;
                    }
                    a[rank][file] = a[rank - 1][file + 1] + 1;
                }
            }
        }

        public static int ComputeScorePwnChains(int[][] a, int size)
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

        public static List<Move> ValidMovesForPawn(Color player, Square pawn, Board board, Game game)
        {
            List<Move> moves = new List<Move>(4);
            Move lastMove = game.LastMove;
            bool lastMoveWasLong
                = lastMove != null
                && lastMove.IsLong
                && lastMove.To.IsOccupiedBy(player.Inverse());

            int moveShift = player == Color.White ? 1 : -1;

            int yForLongMove = player == Color.White ? 1 : Board.c_MAX_INDEX - 1;

            int newY = pawn.Y + moveShift;
            Square movePosition = board.GetSquare(pawn.X, newY);
            if (!movePosition.IsOccupied)
            {
                moves.Add(new Move(pawn, movePosition));
                if (pawn.Y == yForLongMove)
                {
                    int longMoveY = newY + moveShift;
                    Square longMovePosition = board.GetSquare(pawn.X, longMoveY);
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
                Square newSquare = board.GetSquare(newX, newY);
                if (newSquare.IsOccupiedBy(player.Inverse()))
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

        private static int CapturedFileStructure(Square pawn, Board board)
        {
            int structureScore = 0;
            int pawnY = pawn.Y;
            int pawnX = pawn.X;
            Color pColor = pawn.Color;

            if (pawnX > 0)
            {
                if (board.GetSquare(pawnX - 1, pawnY).Color == pColor)
                {
                    structureScore += 3;
                }
                if (pawnY > 0)
                {
                    if (board.GetSquare(pawnX - 1, pawnY - 1).Color == pColor)
                    {
                        structureScore += 3;
                    }
                }
                if (pawnY < board.Size - 1)
                {
                    if (board.GetSquare(pawnX - 1, pawnY + 1).Color == pColor)
                    {
                        structureScore += 3;
                    }
                }
            }

            if (pawnX < board.Size - 1)
            {
                if (board.GetSquare(pawnX + 1, pawnY).Color == pColor)
                {
                    structureScore += 3;
                }
                if (pawnY > 0)
                {
                    if (board.GetSquare(pawnX + 1, pawnY - 1).Color == pColor)
                    {
                        structureScore += 3;
                    }
                }
                if (pawnY < board.Size - 1)
                {
                    if (board.GetSquare(pawnX + 1, pawnY + 1).Color == pColor)
                    {
                        structureScore += 4;
                    }
                }
            }

            return structureScore;
        }

        public static int CapturedFileScore(Board board, int file, Color player, Color myPlayer)
        {
            bool capturedFile = true;
            bool hasPawn = false;
            int capturedFileScore = 0;
            for (int rank = 0; rank < board.Size; ++rank)
            {
                if (board.GetSquare(file, rank).Color == BoardUtilis.EnemyColor(player))
                {
                    capturedFile = false;
                }
                if (board.GetSquare(file, rank).Color == player)
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
                    if (board.GetSquare(file, rank).Color == player)
                    {
                        capturedFileScore = CapturedFileStructure(board.GetSquare(file, rank), board) * 4;
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

        public static Square LeadingPawn(Color pawnColor, Board board)
        {
            if (pawnColor == Color.Black)
            {
                for (int rank = 1; rank < board.Size - 2; ++rank)
                {
                    for (int file = 0; file < board.Size; ++file)
                    {
                        if (board.GetSquare(file, rank).Color == pawnColor)
                        {
                            return board.GetSquare(file, rank);
                        }
                    }
                }
            }

            for (int rank = board.Size - 2; rank >= 0; --rank)
            {
                for (int file = 0; file < board.Size; ++file)
                {
                    if (board.GetSquare(file, rank).Color == pawnColor)
                    {
                        return board.GetSquare(file, rank);
                    }
                }
            }

            return null;
        }

        public static int BlockingPawns(Color color, Color playerToMove, IList<Square> playerPawns,
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

                validMovesForPawn = ValidMovesForPawn(color, playerPawns[i], board, game);
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