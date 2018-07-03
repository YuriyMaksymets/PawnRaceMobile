using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xamarin.Forms;

namespace PawnRaceMobile.Core
{
    public class MiniMaxAI : Player
    {
        private const long c_TimeLimit = 4000;
        private const int c_MinimaxDepth = 5;
        private long m_MoveStartTime;
        private int m_MinimaxMoveIndex, m_MinimaxMoveScore;
        private int[][] m_WhitePawnChains;
        private int[][] m_BlackPawnChains;

        public event Action TurnTaken;

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
                Device.BeginInvokeOnMainThread(() => OnMoveProduced(selectedMove));
                return selectedMove;
            }
            else if (possibleMoves.Count > 1)
            {
                m_WhitePawnChains = new int[Board.Size][];
                m_BlackPawnChains = new int[Board.Size][];
                for (int i = 0; i < Board.Size; i++)
                {
                    m_WhitePawnChains[i] = new int[Board.Size];
                    m_BlackPawnChains[i] = new int[Board.Size];
                }
                m_MoveStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                int optimalMoveIndex = Minimax();

                // Capture possible incorrect minimax behaviour
                if (optimalMoveIndex < 0 || optimalMoveIndex > possibleMoves.Count)
                {
                    optimalMoveIndex = 0;
                }

                Move selectedMove = possibleMoves[optimalMoveIndex];
                Device.BeginInvokeOnMainThread(() => OnMoveProduced(selectedMove));
                return selectedMove;
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => OnMoveProduced(null));
                return null;
            }
        }

        private IList<Square> PawnsByColor(Color color)
            => color == Color.White ? Board.WhitePawns : Board.BlackPawns;

        private IList<Move> CalculateAvailableMovesByColor(Color color)
            => color == Color
            ? CalculatePossibleMovesOptimized()
            : Opponent.CalculatePossibleMovesOptimized();

        private Square BestPassedPawn(Color player)
        {
            IList<Square> pawnsList = PawnsByColor(player);

            Square bestPawn = null;
            int distMin = int.MaxValue;

            foreach (Square pawn in pawnsList)
            {
                if (PlayerUtilis.IsPassedPawn(pawn, Board, player))
                {
                    if (bestPawn == null || distMin > PlayerUtilis.DistanceToFinal(pawn, player, Board))
                    {
                        distMin = PlayerUtilis.DistanceToFinal(pawn, player, Board);
                        bestPawn = pawn;
                    }
                }
            }
            return bestPawn;
        }

        private int Minimax()
        {
            m_MinimaxMoveIndex = 0;
            m_MinimaxMoveScore = -Inf;

            int remainingDepth = c_MinimaxDepth;
            //iterative deepening
            while (m_MinimaxMoveScore != Inf &&
                !SearchCutoff())
            {
                AlphaBetaMax(-Inf, Inf, 0, Color, remainingDepth);
                remainingDepth++;
            }
            return m_MinimaxMoveIndex;
        }

        private bool SearchCutoff()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long elapsedTime = (currentTime - m_MoveStartTime);

            return elapsedTime >= c_TimeLimit;
        }

        private bool IsSearchOver(IList<Move> validMoves, Color playerToMove
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

        private int ComputeScore(Color playerToMove, bool saveWinMove)
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

            int winMoveIndex
                = ContainsWinningMove(Color, CalculateAvailableMovesByColor(Color), playerToMove);
            if (winMoveIndex != -1)
            {
                if (saveWinMove)
                {
                    m_MinimaxMoveScore = Inf;
                    m_MinimaxMoveIndex = winMoveIndex;
                }

                return Inf;
            }

            if (ContainsWinningMove(enemyColor, CalculateAvailableMovesByColor(enemyColor), playerToMove) != -1)
            {
                return -Inf;
            }

            int score = (Board.NumOfWhites() - Board.NumOfBlacks()) * 60;
            if (Color == Color.Black)
            {
                score = -score;
            }

            PlayerUtilis.InitPawnChains(m_WhitePawnChains);
            PlayerUtilis.InitPawnChains(m_BlackPawnChains);

            for (int rank = 1; rank < Board.Size; ++rank)
            {
                PlayerUtilis.ComputePawnChainsForRank(m_WhitePawnChains, Board, rank, Color.White);
            }

            for (int rank = Board.Size - 2; rank >= 1; --rank)
            {
                PlayerUtilis.ComputePawnChainsForRank(m_BlackPawnChains, Board, rank, Color.Black);
            }

            if (Color == Color.White)
            {
                score += 2 * PlayerUtilis.ComputeScorePwnChains(m_WhitePawnChains, Board.Size);
                score -= 2 * PlayerUtilis.ComputeScorePwnChains(m_WhitePawnChains, Board.Size);
            }
            else
            {
                score -= 2 * PlayerUtilis.ComputeScorePwnChains(m_WhitePawnChains, Board.Size);
                score += 2 * PlayerUtilis.ComputeScorePwnChains(m_WhitePawnChains, Board.Size);
            }

            IList<Square> playerPawns = PawnsByColor(Color);
            score += PlayerUtilis
                .BlockingPawns(Color, playerToMove, playerPawns, m_WhitePawnChains, m_BlackPawnChains,
                    Board, m_Game);
            playerPawns = PawnsByColor(enemyColor);
            score -= PlayerUtilis
                .BlockingPawns(enemyColor, playerToMove, playerPawns, m_WhitePawnChains, m_BlackPawnChains,
                    Board, m_Game);

            //search for Board presence on empty file
            //and adjacent files
            int capturedFileScore = 0;
            for (int file = 0; file < Board.Size; ++file)
            {
                capturedFileScore += PlayerUtilis.CapturedFileScore(Board, file, Color, Color);
                capturedFileScore -= PlayerUtilis.CapturedFileScore(Board, file, enemyColor, Color);
            }

            score += capturedFileScore;

            return score;
        }

        private int AlphaBetaMin(int alpha, int beta, int depth, Color player, int remainingDepth)
        {
            IList<Move> validMoves = CalculateAvailableMovesByColor(player);
            int nodeScore;
            bool searchCutoff = SearchCutoff();

            if (IsSearchOver(validMoves, player, remainingDepth, searchCutoff))
            {
                return ComputeScore(player, false);
            }

            foreach (Move currMove in validMoves)
            {
                m_Game.ApplyMove(currMove);
                nodeScore = AlphaBetaMax(alpha, beta, depth + 1, BoardUtilis.EnemyColor(player),
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

        private int AlphaBetaMax(int alpha, int beta, int depth, Color player, int remainingDepth)
        {
            IList<Move> validMoves = CalculateAvailableMovesByColor(player);
            int nodeScore;
            int currBestMove = 0;
            int currBestScore = -Inf;
            bool toChange = false;
            Move currMove;
            bool searchCutoff = SearchCutoff();

            if (IsSearchOver(validMoves, player, remainingDepth, searchCutoff))
            {
                return ComputeScore(player, depth == 0);
            }

            for (int i = 0; i < validMoves.Count; ++i)
            {
                currMove = validMoves[i];

                m_Game.ApplyMove(currMove);
                nodeScore = AlphaBetaMin(alpha, beta, depth + 1, BoardUtilis.EnemyColor(player),
                    remainingDepth - 1);
                m_Game.UnapplyMove(currMove);

                if (depth == 0 && i == m_MinimaxMoveIndex && nodeScore == -Inf)
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
                            m_MinimaxMoveIndex = i;
                            m_MinimaxMoveScore = nodeScore;
                        }
                        currBestMove = i;
                        currBestScore = nodeScore;
                    }

                    alpha = nodeScore;
                }
            }

            searchCutoff = SearchCutoff();
            if (depth == 0 && (!searchCutoff || toChange))
            {
                m_MinimaxMoveIndex = currBestMove;
                m_MinimaxMoveScore = currBestScore;
            }

            return alpha;
        }

        private int ContainsWinningMove(Color player, IList<Move> validMoves, Color playerToMove)
        {
            Square passedPawn = BestPassedPawn(player);
            int finalMove = -1;
            int d1;
            int d2;

            if (passedPawn != null)
            {
                bool advancePassedPawn = true;
                d2 = PlayerUtilis.DistanceToFinal(passedPawn, player, Board);

                //Check if enemy can win
                IList<Square> enemyPawns = Opponent.Pawns;
                foreach (Square pawn in enemyPawns)
                {
                    d1 = PlayerUtilis.DistanceToFinal(pawn, BoardUtilis.EnemyColor(player), Board);

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

        public override void TakeTurn()
        {
            Device.BeginInvokeOnMainThread(() => TurnTaken?.Invoke());
            ThreadPool.QueueUserWorkItem(x => ProduceMove());
        }
    }
}