using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
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
        private readonly ImageSource r_MoveHighlightingImageSource = "movehighlight.png";
        private readonly ImageSource r_BlockHighlightingImageSource = "closedhighlight.png";
        private readonly ImageSource r_DimOverlayImageSource = "dimoverlay.png";

        #endregion Image Sources

        #region Animations

        private const uint c_DestroyAnimLength = 120;
        private const uint c_MoveAnimLength = 280;
        private const uint c_DimmingTime = 120;
        private readonly Easing r_MoveAnimEasing = Easing.SinOut;
        private readonly Easing r_DestroyAnimEasing = Easing.Linear;

        #endregion Animations

        private const double c_DimmingOpacity = 0.9;

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
            mainGridRow.Height = width;
            layoutGrid.ForceLayout();
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
                    Source = r_MoveHighlightingImageSource,
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

        private int YBasedOnBoardRotation(int y) => m_BoardRotated ? Board.c_MAX_INDEX - y : y;

        private void HideStartButton()
        {
            startButton.IsEnabled = false;
            startButton.IsVisible = false;
        }

        private View DimTheScreen(Command onPress = null)
        {
            Image dimmedImage = new Image
            {
                Source = r_DimOverlayImageSource,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Opacity = 0
            };

            if (onPress != null)
            {
                dimmedImage.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = onPress
                });
            }

            overlayLayout.Children.Add
                (dimmedImage, new Rectangle(.5, .5, 10, 10), AbsoluteLayoutFlags.All);
#pragma warning disable CS4014
            dimmedImage.FadeTo(c_DimmingOpacity, c_DimmingTime);
#pragma warning restore CS4014
            return dimmedImage;
        }

        private void DisplayEndgameAlert()
        {
            View dimmedScreen = DimTheScreen();
            BoxView box = new BoxView
            {
                BackgroundColor = (Xamarin.Forms.Color)Application.Current.Resources["light"],
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            BoxView hButtonSeparator = new BoxView
            {
                BackgroundColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                HeightRequest = 1,
                MinimumHeightRequest = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            BoxView vButtonSeparator = new BoxView
            {
                BackgroundColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                WidthRequest = 1,
                MinimumWidthRequest = 1,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            BoxView menuButton = new BoxView
            {
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Opacity = 0
            };

            BoxView restartButton = new BoxView
            {
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Opacity = 0
            };

            menuButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await GoToMainMenu())
            });

            string secondaryMessage = $"Total number of moves: {m_GameManager.TotalMoves}";
            string mainMessage;
            if (m_GameManager.GameResult == Core.Color.None)
            {
                mainMessage = "A Tie!";
            }
            else
            {
                if (m_LocalMultiplayer)
                {
                    mainMessage = $"{m_GameManager.GameResult}s Won!";
                }
                else
                {
                    mainMessage = m_GameManager.GameResult == m_User.Color ?
                        "You Won!"
                        : "You Lost!";
                }
            }

            Label mainLabel = new Label
            {
                Style = (Style)Application.Current.Resources["darkLabel"],
                FontSize = 48,
                Text = mainMessage
            };
            Label secondaryLabel = new Label
            {
                Style = (Style)Application.Current.Resources["descriptionLabel"],
                Text = secondaryMessage
            };
            Label mainMenuLabel = new Label
            {
                Style = (Style)Application.Current.Resources["descriptionLabel"],
                TextColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                Text = "Main Menu"
            };
            Label restartLabel = new Label
            {
                Style = (Style)Application.Current.Resources["descriptionLabel"],
                TextColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                Text = "Restart"
            };

            double winHeight = Application.Current.MainPage.Height;
            double winWidth = Application.Current.MainPage.Width;
            double boxWidth = winWidth * 0.9;
            double boxHeight = Application.Current.MainPage.Height * 0.24;

            Rectangle boxBounds = new Rectangle(.5, .5, .9, .24);
            Rectangle mainLabelBounds = new Rectangle(.5, .42, .7, .08);
            Rectangle secondaryLabelBounds = new Rectangle(.5, .5, .7, .08);

            Rectangle hButtonSeparatorBounds = new Rectangle(.5, .54, boxWidth, 1);
            Rectangle vButtonSeparatorBounds = new Rectangle
                (winWidth / 2, winHeight / 2 + boxHeight / 6, 1, boxHeight / 3);

            Rectangle menuLabelBounds = new Rectangle(.1, .58, .45, .08);
            Rectangle restartLabelBounds = new Rectangle(.9, .58, .45, .08);

            AbsoluteLayoutFlags all = AbsoluteLayoutFlags.All;
            AbsoluteLayoutFlags position = AbsoluteLayoutFlags.PositionProportional;
            AbsoluteLayoutFlags none = AbsoluteLayoutFlags.None;

            View[] elements = new View[]
            {
                box, hButtonSeparator, vButtonSeparator, mainLabel
                , secondaryLabel, mainMenuLabel, restartLabel, dimmedScreen
                , menuButton, restartButton
            };

            restartButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    elements.ForEach(x =>
                    {
                        overlayLayout.Children.Remove(x);
                        x.IsEnabled = false;
                    });
                    Restart();
                })
            });

            elements.ForEach(x => x.Opacity = 0);

            overlayLayout.Children.Add(box, boxBounds, all);
            overlayLayout.Children.Add(mainLabel, mainLabelBounds, all);
            overlayLayout.Children.Add(secondaryLabel, secondaryLabelBounds, all);

            overlayLayout.Children.Add(hButtonSeparator, hButtonSeparatorBounds, position);
            overlayLayout.Children.Add(vButtonSeparator, vButtonSeparatorBounds, none);
            overlayLayout.Children.Add(mainMenuLabel, menuLabelBounds, all);
            overlayLayout.Children.Add(restartLabel, restartLabelBounds, all);
            overlayLayout.Children.Add(menuButton, menuLabelBounds, all);
            overlayLayout.Children.Add(restartButton, restartLabelBounds, all);

            vButtonSeparator.WidthRequest = 1;

            elements.ForEach(x => x.FadeTo(1, c_DimmingTime));
        }

        private void DisplayMenu()
        {
            BoxView box = new BoxView
            {
                BackgroundColor = (Xamarin.Forms.Color)Application.Current.Resources["light"],
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            BoxView hButtonSeparator = new BoxView
            {
                BackgroundColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                HeightRequest = 1,
                MinimumHeightRequest = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            Label mainMenuLabel = new Label
            {
                Style = (Style)Application.Current.Resources["descriptionLabel"],
                TextColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                Text = "Main Menu",
                FontSize = 40
            };
            Label restartLabel = new Label
            {
                Style = (Style)Application.Current.Resources["descriptionLabel"],
                TextColor = (Xamarin.Forms.Color)Application.Current.Resources["verydark"],
                Text = "Restart",
                FontSize = 40
            };

            double boxWidth = Application.Current.MainPage.Width * 0.9;

            Rectangle boxBounds = new Rectangle(.5, .5, .9, .24);
            Rectangle hButtonSeparatorBounds = new Rectangle(.5, .5, boxWidth, 1);
            Rectangle menuLabelBounds = new Rectangle(.5, .43, .9, .1);
            Rectangle restartLabelBounds = new Rectangle(.5, .56, .9, .1);

            AbsoluteLayoutFlags all = AbsoluteLayoutFlags.All;
            AbsoluteLayoutFlags position = AbsoluteLayoutFlags.PositionProportional;

            View[] elements = new View[]
            {
                box, hButtonSeparator, mainMenuLabel, restartLabel
            };

            elements.ForEach(x => x.Opacity = 0);

            DimTheScreen(new Command(() =>
            {
                elements.ForEach(x => x.FadeTo(0));
            }));

            overlayLayout.Children.Add(box, boxBounds, all);
            overlayLayout.Children.Add(hButtonSeparator, hButtonSeparatorBounds, position);
            overlayLayout.Children.Add(mainMenuLabel, menuLabelBounds, all);
            overlayLayout.Children.Add(restartLabel, restartLabelBounds, all);

            elements.ForEach(x => x.FadeTo(1));
        }
    }
}