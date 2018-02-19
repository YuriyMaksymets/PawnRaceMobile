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
        private Player m_User = new Player();
        private GameManager m_GameManager;
        private Square? m_SelectedPawn;

        public delegate void MoveEventHandler(Player player, Move move);

        public event MoveEventHandler OnMoveConstructed;

        public BoardPage(char whiteGap, char blackGap)
        {
            InitializeComponent();
            InitializeBoardGrid();

            m_GameManager = new GameManager(whiteGap, blackGap, m_User, new Player());
            m_GameManager.Play();
            OnMoveConstructed += m_GameManager.SelectMove;
            m_GameManager.Board.Pawns.ForEach(x =>
            {
                Image image = x.IsBlack ? new Image
                {
                    Source = ImageSource.FromResource
                    (
                        "PawnRaceMobile.Resourses.blackpawn.png"
                        , Assembly.GetExecutingAssembly()
                    )
                }
                : new Image
                {
                    Source = ImageSource.FromResource
                    (
                        "PawnRaceMobile.Resourses.whitepawn.png"
                        , Assembly.GetExecutingAssembly()
                    )
                };
                TapGestureRecognizer iconTap = new TapGestureRecognizer();
                iconTap.Tapped += OnPawnTapped;
                image.GestureRecognizers.Add(iconTap);
                mainGrid.Children.Add(image, x.X, x.Y);
            });
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
                    Move move = new Move(m_SelectedPawn.Value, selectedSquare);
                    m_SelectedPawn = null;
                    if (m_GameManager.IsValidMove(move))
                    {
                        OnMoveConstructed?.Invoke(m_User, move);
                    }
                    else
                    {
                        //Get rid of highlighting
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
                .GetSquare(Grid.GetRow(senderImage), Grid.GetColumn(senderImage));
        }
    }
}