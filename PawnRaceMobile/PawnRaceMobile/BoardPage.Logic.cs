using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PawnRaceMobile
{
    public partial class BoardPage : ContentPage
    {
        private GameManager m_GameManager;
        private Square m_SelectedPawn;
        private HumanPlayer m_User;
        private bool m_ControlEnabled;

        public BoardPage(char whiteGap, char blackGap, bool userPlaysWhite)
        {
            InitializeComponent();
            InitializeBoardGrid();
            SetUpGame(whiteGap, blackGap, userPlaysWhite);
            RenderAllPawns();
        }

        private void SetUpGame(char whiteGap, char blackGap, bool userPlaysWhite)
        {
            Core.Color userColor = userPlaysWhite ? Core.Color.White : Core.Color.Black;
            m_ControlEnabled = m_BoardRotated = userPlaysWhite;
            m_User = new HumanPlayer(userColor);
            m_User.TurnTaken += EnableControl;
            m_User.MoveProduced += DisableControl;
            Player opponent = new RandomAI(userColor.Inverse());
            m_GameManager = new GameManager(whiteGap, blackGap, m_User, opponent);
            m_GameManager.MoveMade += RenderAllPawns;
        }

        private void EnableControl() => m_ControlEnabled = true;

        private void DisableControl(Player _, Move __) => m_ControlEnabled = false;

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
            Square selectedSquare = SquareFromImage(sender);
            //if selectedcolor == playercolor
            m_SelectedPawn = selectedSquare;
            Log(selectedSquare);
        }

        protected void OnSquareTapped(object sender, EventArgs e)
        {
            if (!m_ControlEnabled)
            {
                return;
            }
            if (m_SelectedPawn != null)
            {
                //if selectedSquare in possibleMoves
                try
                {
                    Square selectedSquare = SquareFromImage(sender);
                    Move move = new Move(m_SelectedPawn, selectedSquare);
                    m_SelectedPawn = null;
                    if (m_GameManager.IsValidMove(move))
                    {
                        m_User.ParseMove(move);
                    }
                    else
                    {
                        Alert("Invalid move");
                    }
                }
                catch (Exception exc)
                {
                    Log(exc);
                }
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