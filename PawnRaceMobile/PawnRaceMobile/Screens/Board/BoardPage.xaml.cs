using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PawnRaceMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoardPage : ContentPage
    {
        #region Image Sources

        private readonly ImageSource r_BlackFillSource = "blackfill.png";

        private readonly ImageSource r_BlackImageSource = "blackpawn.png";

        private readonly ImageSource r_WhiteFillSource = "whitefill.png";

        private readonly ImageSource r_WhiteImageSource = "whitepawn.png";

        private readonly ImageSource r_YellowPointImageSource = "movehighlight.png";

        #endregion Image Sources

        #region Animations

        private const uint c_DestroyAnimLength = 120;
        private const uint c_MoveAnimLength = 280;
        private readonly Easing r_MoveAnimEasing = Easing.SinOut;
        private readonly Easing r_DestroyAnimEasing = Easing.Linear;

        #endregion Animations

        private IList<Image> m_AvailableMovesImages = new List<Image>(2);
        private bool m_BoardRotated;
        private (double, double) m_Dimensions;
        private IDictionary<Square, Image> m_PawnImages = new Dictionary<Square, Image>(14);
        private double m_SquareWidth;

        public void InitializeBackground()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = ((j + i) % 2 == 0) ? new Image
                    {
                        Source = r_BlackFillSource,
                    } :
                    new Image
                    {
                        Source = r_WhiteFillSource,
                    };
                    image.Aspect = Aspect.AspectFill;
                    AddTapRecognition(image, OnSquareTapped);
                    mainGrid.Children.Add(image, i, YBasedOnBoardRotation(j));
                }
            }
            Console.WriteLine("BoardPage background initialized");
        }

        protected override void OnAppearing()
        {
            mainGrid.ForceLayout();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (m_Dimensions.Item1 == width && m_Dimensions.Item2 == height)
            {
                return;
            }
            Console.WriteLine("Size allocated with width " + width + " and height " + height);
            m_Dimensions = (width, height);
            layout.WidthRequest = width;
            layout.HeightRequest = height;
            m_SquareWidth = width / 8;
            if (height >= width)
            {
                mainGrid.HeightRequest = width;
                mainGrid.WidthRequest = width;
                for (int i = 0; i < 8; i++)
                {
                    mainGrid.RowDefinitions[i].Height = m_SquareWidth;
                    mainGrid.ColumnDefinitions[i].Width = m_SquareWidth;
                }
            }
        }

        private void AddTapRecognition(View element, EventHandler action)
        {
            TapGestureRecognizer iconTap = new TapGestureRecognizer();
            iconTap.Tapped += action;
            element.GestureRecognizers.Add(iconTap);
        }

        private void Alert(object obj) => DisplayAlert(obj.ToString(), "", "Close");

        private void DisplayAvailableMoves()
        {
            Image image;

            IList<Move> availableMoves = m_GameManager.CurrentPlayer
                .GetAvailableMovesForPawn(m_Source);
            foreach (Move m in availableMoves)
            {
                image = new Image
                {
                    Source = r_YellowPointImageSource,
                    Aspect = Aspect.Fill
                };
                if (m.To.IsOccupied)
                {
                    AddTapRecognition(image, OnPawnTapped);
                }
                else
                {
                    AddTapRecognition(image, OnSquareTapped);
                }
                m_AvailableMovesImages.Add(image);
                mainGrid.Children.Add(image, m.To.X, YBasedOnBoardRotation(m.To.Y));
            }
        }

        private void Log(object obj) => console.Text += ('\n' + obj.ToString());

        private void OnClearButton(object sender, EventArgs e) => console.Text = "";

        private void RenderAllPawns()
        {
            m_PawnImages.ForEach(x => mainGrid.Children.Remove(x.Value));
            m_PawnImages = new Dictionary<Square, Image>(m_GameManager.Board.Pawns.Count);
            m_GameManager.Board.Pawns.ForEach(x => RenderPawn(x));
        }

        private void RenderChanges()
        {
            Move lastMove = m_GameManager.LastMove;
            if (lastMove.IsEpCapture)
            {
                Square s1 = m_GameManager.Board.GetSquare(lastMove.To.X, lastMove.To.Y - 1);
                Square s2 = m_GameManager.Board.GetSquare(lastMove.To.X, lastMove.To.Y + 1);
                Square pawnToRemove = m_PawnImages.ContainsKey(s1) ? s1 : s2;
                DerenderPawn(pawnToRemove, true);
            }
            else if (lastMove.IsCapture)
            {
                DerenderPawn(lastMove.To, true);
            }
            RenderPawn(lastMove.To, lastMove.From);
        }

        private async void DerenderPawn(Square pawn, bool animate = false)
        {
            Image pawnImage = m_PawnImages[pawn];
            m_PawnImages.Remove(pawn);
            if (animate)
            {
                await pawnImage.FadeTo(0, c_DestroyAnimLength, r_DestroyAnimEasing);
            }
            mainGrid.Children.Remove(pawnImage);
        }

        private Rectangle PawnBoundsRectangle(Square pawn)
        {
            return new Rectangle
                    (pawn.X * m_SquareWidth
                    , YBasedOnBoardRotation(pawn.Y) * m_SquareWidth
                    , m_SquareWidth
                    , m_SquareWidth);
        }

        private async void RenderPawn(Square pawn, Square animateFrom = null)
        {
            Image image;
            int actualY = YBasedOnBoardRotation(pawn.Y);
            bool controlInitiallyEnabled = m_ControlEnabled;
            if (animateFrom != null)
            {
                DisableControl();
                image = m_PawnImages[animateFrom];
                m_PawnImages.Remove(animateFrom);
                m_PawnImages.Add(pawn, image);
                Rectangle newBounds = PawnBoundsRectangle(pawn);
                await image.LayoutTo(newBounds, c_MoveAnimLength, r_MoveAnimEasing);
            }
            else
            {
                image = pawn.IsBlack ? new Image
                {
                    Source = r_BlackImageSource
                }
                : new Image
                {
                    Source = r_WhiteImageSource
                };
                AddTapRecognition(image, OnPawnTapped);
                m_PawnImages.Add(pawn, image);
            }
            mainGrid.Children.Add(image, pawn.X, actualY);
            if (controlInitiallyEnabled)
            {
                EnableControl();
            }
        }

        private void UndisplayAvailableMoves()
        {
            m_AvailableMovesImages.ForEach(x => mainGrid.Children.Remove(x));
            m_AvailableMovesImages.Clear();
        }

        private int YBasedOnBoardRotation(int y)
        {
            return m_BoardRotated ? Board.c_MAX_INDEX - y : y;
        }
    }
}