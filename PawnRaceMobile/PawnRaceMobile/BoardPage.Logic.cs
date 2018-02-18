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
        private Square? m_SelectedPawn;

        public delegate void MoveEventHandler(Move move);

        public event MoveEventHandler OnMoveConstructed;

        public BoardPage()
        {
            InitializeComponent();
            InitializeBoardGrid();

            m_GameManager = new GameManager('a', 'b');
            m_GameManager.Play();
            OnMoveConstructed += m_GameManager.SelectMove;
            m_GameManager.Board.Pawns.ForEach(x =>
            {
                Image image = new Image
                {
                    Source = ImageSource.FromResource
                    (
                        "PawnRaceMobile.Resourses.blackpawn.png"
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
                        OnMoveConstructed?.Invoke(move);
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