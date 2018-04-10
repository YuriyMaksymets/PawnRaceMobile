using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;

namespace PawnRaceMobile
{
    internal class GameManager
    {
        private Game m_Game;
        public Stack<Move> m_moves => m_Game.m_Moves;
        public Board Board => m_Game.Board;
        public IPlayer CurrentPlayer => m_Game.CurrentPlayer;
        public bool IsFinished => m_Game.IsFinished;
        public Color GameResult => m_Game.GameResult;
        public int TotalMoves => m_Game.NumberOfMoves;
        public Move LastMove => m_Game.LastMove;

        public event Action MoveMade;
        public event Action<Move> buttonToAdd;
        
        public GameManager(char whiteGap, char blackGap
            , Player player1, Player player2)
        {
            m_Game = new Game(whiteGap, blackGap, player1, player2);
            CurrentPlayer.MoveProduced += SelectMove;
            CurrentPlayer.TakeTurn();
        }

        public bool IsValidMove(Move move)
        {
            IList<Move> moves = CurrentPlayer.GetAvailableMovesForPawn(move.From);
            foreach (Move m in moves)
            {
                if (m.Equals(move))
                {
                    return true;
                }
            }

            return false;
        }

        public void SelectMove(IPlayer player, Move move)
        {
            if (CurrentPlayer == player && !m_Game.IsFinished)
            {
                CurrentPlayer.MoveProduced -= SelectMove;
                m_Game.ApplyMove(move);
                MoveMade?.Invoke();
                buttonToAdd?.Invoke(move);
                CurrentPlayer.MoveProduced += SelectMove;
                CurrentPlayer.TakeTurn();
            }
        }
    }
}