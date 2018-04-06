using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        private IList<Image> m_AvailableMoves = new List<Image>(2);
        private bool m_BoardRotated;
        private (double, double) m_Dimensions;
        private IDictionary<Square, Image> m_PawnImages = new Dictionary<Square, Image>(14);

        public void InitializeBackground()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = ((j + i) % 2 == 0) ? new Image
                    {
                        Source = r_BlackFillSource,
                        Aspect = Aspect.AspectFill,
                    } :
                    new Image
                    {
                        Source = r_WhiteFillSource,
                        Aspect = Aspect.AspectFill,
                    };
                    AddTapRecognition(image, OnSquareTapped);
                    mainGrid.Children.Add(image, i, CoordBasedOnBoardRotation(j));
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
            if (m_Dimensions.Item1 == width || m_Dimensions.Item2 == height)
            {
                return;
            }
            Console.WriteLine("Size allocated with width " + width + " and height " + height);
            m_Dimensions = (width, height);
            layout.WidthRequest = width;
            layout.HeightRequest = height;
            if (height >= width)
            {
                mainGrid.HeightRequest = width;
                mainGrid.WidthRequest = width;
                for (int i = 0; i < 8; i++)
                {
                    mainGrid.RowDefinitions[i].Height = width / 8;
                    mainGrid.ColumnDefinitions[i].Width = width / 8;
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    mainGrid.ColumnDefinitions[i].Width = height / 8;
                }
                mainGrid.WidthRequest = height;
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
                m_AvailableMoves.Add(image);
                mainGrid.Children.Add(image, m.To.X
                , m_BoardRotated ? Board.c_MAX_INDEX - m.To.Y : m.To.Y);
            }
        }

        private void InitializeBoardGrid()
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    for (int j = 0; j < 8; j++)
            //    {
            //        Image image = ((j + i) % 2 == 0) ? new Image
            //        {
            //            Source = r_BlackFillSource,
            //            Aspect = Aspect.AspectFill,
            //        } :
            //        new Image
            //        {
            //            Source = r_WhiteFillSource,
            //            Aspect = Aspect.AspectFill,
            //        };
            //        AddTapRecognition(image, OnSquareTapped);
            //        mainGrid.Children.Add(image, i, m_BoardRotated ? Board.c_MAX_INDEX - j : j);
            //    }
            //}
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
            DerenderPawn(lastMove.From);
            if (lastMove.IsEpCapture)
            {
                Square s1 = m_GameManager.Board.GetSquare(lastMove.To.X, lastMove.To.Y - 1);
                Square s2 = m_GameManager.Board.GetSquare(lastMove.To.X, lastMove.To.Y + 1);
                Square pawnToRemove = m_PawnImages.ContainsKey(s1) ? s1 : s2;
                DerenderPawn(pawnToRemove);
            }
            else if (lastMove.IsCapture)
            {
                DerenderPawn(lastMove.To);
            }
            RenderPawn(lastMove.To);
        }

        private void DerenderPawn(Square pawn)
        {
            mainGrid.Children.Remove(m_PawnImages[pawn]);
            m_PawnImages.Remove(pawn);
        }

        private void RenderPawn(Square pawn)
        {
            Image image = pawn.IsBlack ? new Image
            {
                Source = r_BlackImageSource
            }
            : new Image
            {
                Source = r_WhiteImageSource
            };
            m_PawnImages.Add(pawn, image);
            AddTapRecognition(image, OnPawnTapped);
            mainGrid.Children.Add(image, pawn.X, CoordBasedOnBoardRotation(pawn.Y));
        }

        private void UndisplayAvailableMoves()
        {
            m_AvailableMoves.ForEach(x => mainGrid.Children.Remove(x));
            m_AvailableMoves.Clear();
        }

        private int CoordBasedOnBoardRotation(int y)
        {
            return m_BoardRotated ? Board.c_MAX_INDEX - y : y;
        }
    }
}