using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using DLToolkit.Forms.Controls;

namespace PawnRaceMobile
{
    public partial class BoardPage : ContentPage
    {
        private GameManager m_GameManager;
        private Square m_Source;
        private Square m_Destination;
        private HumanPlayer m_User;
        private bool m_ControlEnabled;
        private bool m_LocalMultiplayer;

        public BoardPage(char whiteGap, char blackGap, bool userPlaysWhite, bool localMultiplayer)
        {
            //m_LocalMultiplayer = localMultiplayer;
            // SetUpGame(whiteGap, blackGap, userPlaysWhite);
            InitializeComponent();
            InitializeBoardGrid();
            //FlowListView.
            //RenderAllPawns();
        }

        public void SetUpGame(char whiteGap, char blackGap, bool userPlaysWhite, bool localMultiplayer)
        {
            m_LocalMultiplayer = localMultiplayer;
            Core.Color userColor = userPlaysWhite ? Core.Color.White : Core.Color.Black;
            m_ControlEnabled = m_BoardRotated = userPlaysWhite;
            m_User = new HumanPlayer(userColor);
            m_User.TurnTaken += EnableControl;
            m_User.MoveProduced += DisableControl;
            Player opponent;
            if (m_LocalMultiplayer)
            {
                opponent = new HumanPlayer(userColor.Inverse());
            }
            else
            {
                opponent = new RandomAI(userColor.Inverse());
            }
            m_GameManager = new GameManager(whiteGap, blackGap, m_User, opponent);
            m_GameManager.MoveMade += RenderAllPawns;
            RenderAllPawns();
        }

        private void EnableControl() => m_ControlEnabled = true;

        private void DisableControl(IPlayer _, Move __) => m_ControlEnabled = false;

        protected void OnPawnTapped(object sender, EventArgs e)
        {
            if (!m_ControlEnabled)
            {
                return;
            }
            //Hightlight this square and possible move squares
            //Highlight colors:
            //Current
            //Move
            //Attack
            //No available moves

            //Getting the selected Square
            IPlayer currentPlayer = m_GameManager.CurrentPlayer;
            Square currentSquare = SquareFromImage(sender);

            if (m_Source == null)
            {
                //Necesary for displaying the valid moves
                if (currentSquare.IsOccupiedBy(currentPlayer.Color))
                {
                    m_Source = currentSquare;
                    DisplayAvailableMoves();
                }
            }
            else
            {
                m_Destination = SquareFromImage(sender);
                Move move = (m_Destination.X != m_Source.X)
                    ? new Move(m_Source, m_Destination, true, false)
                    : new Move(m_Source, m_Destination);

                ManageMove(move);

                UndisplayAvailableMoves();
                m_Source = m_Destination = null;
            }
        }

        protected void OnSquareTapped(object sender, EventArgs e)
        {
            if (!m_ControlEnabled || m_Source == null)
            {
                return;
            }
            m_Destination = SquareFromImage(sender);
            Move move = (m_Destination.X != m_Source.X)
                ? new Move(m_Source, m_Destination, true, true)
                : new Move(m_Source, m_Destination);
            ManageMove(move);
            UndisplayAvailableMoves();
            m_Source = m_Destination = null;
        }

        private void ManageMove(Move move)
        {
            if (m_GameManager.IsValidMove(move))
            {
                m_User.ParseMove(move);
                if (m_LocalMultiplayer)
                {
                    m_User = m_User.Opponent as HumanPlayer;
                    EnableControl();
                }
            }
            else
            {
                Alert("Invalid move");
            }
        }

        private Square SquareFromImage(object sender)
        {
            Image senderImage = (Image)sender;
            return m_GameManager.Board
                .GetSquare(Grid.GetColumn(senderImage),
                m_BoardRotated ? Board.c_MAX_INDEX - Grid.GetRow(senderImage)
                : Grid.GetRow(senderImage));
        }
    }
}