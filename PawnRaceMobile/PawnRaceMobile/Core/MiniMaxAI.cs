using System;
using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    public class MiniMaxAI : Player
    {
        private long timeLimit = 3000;
        private long moveStartTime;
        private int minimaxMoveIndex, minimaxMoveScore;
        private const int c_MinimaxDepth = 5;
        private int[][] whitePawnChains;
        private int[][] blackPawnChains;
        private int Inf => int.MaxValue;

        public MiniMaxAI(Color color) : base(color)
        {
        }

        public override Move ProduceMove()
        {
            IList<Move> possibleMoves = CalculatePossibleMovesOptimized();
            if (possibleMoves.Count == 1)
            {
                Move selectedMove = possibleMoves[0];
                OnMoveProduced(selectedMove);
                return selectedMove;
            }
            else if (possibleMoves.Count > 1)
            {
                whitePawnChains = new int[Board.Size][];
                blackPawnChains = new int[Board.Size][];
                for (int i = 0; i < Board.Size; i++)
                {
                    whitePawnChains[i] = new int[Board.Size];
                    blackPawnChains[i] = new int[Board.Size];
                }
                moveStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                Move selectedMove = possibleMoves[minimax()];
                OnMoveProduced(selectedMove);
                return selectedMove;
            }
            else
            {
                OnMoveProduced(null);
                return null;
            }
        }

        private IList<Square> PawnsByColor(Color color)
            => color == Color.White ? Board.WhitePawns : Board.BlackPawns;

        private IList<Move> CalculateAvailableMovesByColor(Color color)
            => color == Color
            ? CalculatePossibleMovesOptimized()
            : Opponent.CalculatePossibleMovesOptimized();

        private Square bestPassedPawn(Color player)
        {
            IList<Square> pawnsList = PawnsByColor(player);

            Square bestPawn = null;
            int distMin = int.MaxValue;

            foreach (Square pawn in pawnsList)
            {
                if (PlayerUtilis.isPassedPawn(pawn, Board, player))
                {
                    if (bestPawn == null || distMin > PlayerUtilis.getDistToFinal(pawn, player, Board))
                    {
                        distMin = PlayerUtilis.getDistToFinal(pawn, player, Board);
                        bestPawn = pawn;
                    }
                }
            }
            return bestPawn;
        }

        private int minimax()
        {
            minimaxMoveIndex = 0;
            minimaxMoveScore = -Inf;

            int remainingDepth = c_MinimaxDepth;
            //iterative deepening
            while (minimaxMoveScore != Inf &&
                !getSearchCutoff())
            {
                alphaBetaMax(-Inf, Inf, 0, Color, remainingDepth);
                remainingDepth++;
            }
            return minimaxMoveIndex;
        }

        private bool getSearchCutoff()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long elapsedTime = (currentTime - moveStartTime);

            return elapsedTime >= timeLimit;
        }

        private bool searchOver(IList<Move> validMoves, Color playerToMove
            , int remainingDepth
            , bool searchCutoff)
        {
            return (searchCutoff || m_Game.IsFinished || validMoves.Count == 0
                || remainingDepth == 0
                || ContainsWinningMove(Color.Black
                    , CalculateAvailableMovesByColor(Color.Black), playerToMove) != -1
                || ContainsWinningMove(Color.White
                    , CalculateAvailableMovesByColor(Color.White), playerToMove) != -1);
        }

        private int computeScore(Color playerToMove)
        {
            Color enemyColor = Opponent.Color;

            if (m_Game.IsFinished)
            {
                Color winner = m_Game.GameResult;
                if (winner == Color.None)
                {
                    return 0;
                }
                else if (winner == Color)
                {
                    return Inf;
                }
                else
                {
                    return -Inf;
                }
            }

            if (ContainsWinningMove(Color, CalculateAvailableMovesByColor(Color), playerToMove) != -1)
            {
                return Inf;
            }

            if (ContainsWinningMove(enemyColor, CalculateAvailableMovesByColor(enemyColor), playerToMove) != -1)
            {
                return -Inf;
            }

            int score = (Board.getNoOfWhites() - Board.getNoOfBlackes()) * 60;
            if (Color == Color.Black)
            {
                score = -score;
            }

            PlayerUtilis.initPawnChains(whitePawnChains);
            PlayerUtilis.initPawnChains(blackPawnChains);

            for (int rank = 1; rank < Board.Size; ++rank)
            {
                PlayerUtilis.computePawnChainsForRank(whitePawnChains, Board, rank, Color.White);
            }

            for (int rank = Board.Size - 2; rank >= 1; --rank)
            {
                PlayerUtilis.computePawnChainsForRank(blackPawnChains, Board, rank, Color.Black);
            }

            if (Color == Color.White)
            {
                score += 2 * PlayerUtilis.computeScorePwnChains(whitePawnChains, Board.Size);
                score -= 2 * PlayerUtilis.computeScorePwnChains(whitePawnChains, Board.Size);
            }
            else
            {
                score -= 2 * PlayerUtilis.computeScorePwnChains(whitePawnChains, Board.Size);
                score += 2 * PlayerUtilis.computeScorePwnChains(whitePawnChains, Board.Size);
            }

            IList<Square> playerPawns = PawnsByColor(Color);
            score += PlayerUtilis
                .blockingPawns(Color, playerToMove, playerPawns, whitePawnChains, blackPawnChains,
                    Board, m_Game);
            playerPawns = PawnsByColor(enemyColor);
            score -= PlayerUtilis
                .blockingPawns(enemyColor, playerToMove, playerPawns, whitePawnChains, blackPawnChains,
                    Board, m_Game);

            //search for Board presence on empty file
            //and adjacent files
            int capturedFileScore = 0;
            for (int file = 0; file < Board.Size; ++file)
            {
                capturedFileScore += PlayerUtilis.capturedFileScore(Board, file, Color, Color);
                capturedFileScore -= PlayerUtilis.capturedFileScore(Board, file, enemyColor, Color);
            }

            score += capturedFileScore;

            return score;
        }

        private int alphaBetaMin(int alpha, int beta, int depth, Color player, int remainingDepth)
        {
            IList<Move> validMoves = CalculateAvailableMovesByColor(player);
            int nodeScore;
            bool searchCutoff = getSearchCutoff();

            if (searchOver(validMoves, player, remainingDepth, searchCutoff))
            {
                return computeScore(player);
            }

            foreach (Move currMove in validMoves)
            {
                m_Game.ApplyMove(currMove);
                nodeScore = alphaBetaMax(alpha, beta, depth + 1, BoardUtilis.enemyColor(player),
                    remainingDepth - 1);
                m_Game.UnapplyMove(currMove);

                if (nodeScore <= alpha)
                {
                    return nodeScore;
                }

                if (nodeScore < beta)
                {
                    beta = nodeScore;
                }
            }

            return beta;
        }

        private int alphaBetaMax(int alpha, int beta, int depth, Color player, int remainingDepth)
        {
            IList<Move> validMoves = CalculateAvailableMovesByColor(player);
            int nodeScore;
            int currBestMove = 0;
            int currBestScore = -Inf;
            bool toChange = false;
            Move currMove;
            bool searchCutoff = getSearchCutoff();

            if (searchOver(validMoves, player, remainingDepth, searchCutoff))
            {
                return computeScore(player);
            }

            for (int i = 0; i < validMoves.Count; ++i)
            {
                currMove = validMoves[i];

                m_Game.ApplyMove(currMove);
                nodeScore = alphaBetaMin(alpha, beta, depth + 1, BoardUtilis.enemyColor(player),
                    remainingDepth - 1);
                m_Game.UnapplyMove(currMove);

                if (depth == 0 && i == minimaxMoveIndex && nodeScore == -Inf)
                {
                    toChange = true;
                }

                if (depth > 0 && nodeScore >= beta)
                {
                    return nodeScore;
                }

                if (nodeScore > alpha)
                {
                    if (depth == 0)
                    {
                        if (nodeScore == Inf)
                        {
                            minimaxMoveIndex = i;
                            minimaxMoveScore = nodeScore;
                        }
                        currBestMove = i;
                        currBestScore = nodeScore;
                    }

                    alpha = nodeScore;
                }
            }

            searchCutoff = getSearchCutoff();
            if (depth == 0 && (!searchCutoff || toChange))
            {
                minimaxMoveIndex = currBestMove;
                minimaxMoveScore = currBestScore;
            }

            return alpha;
        }

        private int ContainsWinningMove(Color player, IList<Move> validMoves, Color playerToMove)
        {
            Square passedPawn = bestPassedPawn(player);
            int finalMove = -1;
            int d1;
            int d2;

            if (passedPawn != null)
            {
                bool advancePassedPawn = true;
                d2 = PlayerUtilis.getDistToFinal(passedPawn, player, Board);

                //Check if enemy can win
                IList<Square> enemyPawns = Opponent.Pawns;
                foreach (Square pawn in enemyPawns)
                {
                    d1 = PlayerUtilis.getDistToFinal(pawn, BoardUtilis.enemyColor(player), Board);

                    if (d1 < d2)
                    {
                        advancePassedPawn = false;
                    }
                    else if (d1 == d2 && playerToMove != player)
                    {
                        advancePassedPawn = false;
                    }
                }

                Move currMove;
                if (advancePassedPawn)
                {
                    for (int i = 0; i < validMoves.Count; ++i)
                    {
                        currMove = validMoves[i];
                        //For a pawn the one step forward move is always before the two steps one
                        //in the list of valid moves
                        if (currMove.From == passedPawn)
                        {
                            finalMove = i;
                        }
                    }

                    return finalMove;
                }
            }

            return -1;
        }

        public override void TakeTurn() => ProduceMove();
    }
}