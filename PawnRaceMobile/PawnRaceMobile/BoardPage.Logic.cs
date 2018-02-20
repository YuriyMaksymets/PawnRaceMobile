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
        private HumanPlayer m_User = new HumanPlayer(Core.Color.White);

        public BoardPage(char whiteGap, char blackGap)
        {
            InitializeComponent();
            InitializeBoardGrid();
            Player opponent = new RandomAI(Core.Color.Black);
            m_GameManager = new GameManager(whiteGap, blackGap, m_User, opponent);
            m_GameManager.MoveMade += RenderPawns;
            RenderPawns();
        }

        protected void OnPawnTapped(object sender, EventArgs e)
        {
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
                .GetSquare(Grid.GetColumn(senderImage), Grid.GetRow(senderImage));
        }
    }
}