using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PawnRaceMobile
{
    public partial class BoardPage : ContentPage
    {
        private int[] m_GapIndecies = new int[2] { 0, 0 };
        private Image[] m_BlockedImages;

        private void RenderGapSelectionPawns()
        {
            m_PawnImages = new Dictionary<Square, Image>(16);
            m_BlockedImages = new Image[2];
            for (int i = 0; i < 8; i++)
            {
                Image whiteImage = new Image
                {
                    Source = r_WhiteImageSource
                };
                Image blackImage = new Image
                {
                    Source = r_BlackImageSource
                };
                int actualWhiteY = YBasedOnBoardRotation(1);
                int actualBlackY = YBasedOnBoardRotation(6);
                mainGrid.Children.Add(whiteImage, i, actualWhiteY);
                mainGrid.Children.Add(blackImage, i, actualBlackY);
                AddTapRecognition(whiteImage, OnDummyPawnTapped);
                AddTapRecognition(blackImage, OnDummyPawnTapped);
                m_PawnImages.Add(new Square(i, 1), whiteImage);
                m_PawnImages.Add(new Square(i, 6), blackImage);
                if (i == m_GapIndecies[0])
                {
                    whiteImage.IsVisible = false;
                    m_BlockedImages[0] = new Image
                    {
                        Source = r_BlockHighlightingImageSource
                    };
                    mainGrid.Children.Add(m_BlockedImages[0], i, actualWhiteY);
                }
                if (i == m_GapIndecies[1])
                {
                    blackImage.IsVisible = false;
                    m_BlockedImages[1] = new Image
                    {
                        Source = r_BlockHighlightingImageSource
                    };
                    mainGrid.Children.Add(m_BlockedImages[1], i, actualBlackY);
                }
            }
        }

        private void LinkDummyPawns()
        {
            Dictionary<Square, Image> newPawnImages = new Dictionary<Square, Image>(14);
            m_PawnImages.ForEach(x =>
            {
                x.Value.GestureRecognizers.Clear();
                Square newSquare = m_GameManager.Board.Pawns
                .Where(pawn => pawn.X == x.Key.X && pawn.Y == x.Key.Y)
                .DefaultIfEmpty(null).First();
                if (newSquare == null)
                {
                    return;
                }
                Image image = x.Value;
                AddTapRecognition(image, OnPawnTapped);
                newPawnImages.Add(newSquare, image);
            });
            m_PawnImages = newPawnImages;
        }

        protected void OnDummyPawnTapped(object sender, EventArgs e)
        {
            Image senderImage = (Image)sender;
            int senderY = Grid.GetRow(senderImage);
            m_PawnImages
                .Where(x => x.Key.Y == YBasedOnBoardRotation(senderY) && !x.Value.IsVisible)
                .ForEach(x => x.Value.IsVisible = true);
            senderImage.IsVisible = false;

            bool whitePawnTapped = YBasedOnBoardRotation(senderY) == 1;
            int colorIndex = whitePawnTapped ? 0 : 1;
            int senderX = Grid.GetColumn(senderImage);
            m_GapIndecies[colorIndex] = senderX;
            Image blockedImage = m_BlockedImages[colorIndex];
            mainGrid.Children.Add(blockedImage, senderX, senderY);
        }

        protected void StartGameAfterSelectingGaps()
        {
            HideStartButton();
            SetUpGame();
            LinkDummyPawns();
            m_BlockedImages.ForEach(x
                => Device.BeginInvokeOnMainThread(() =>
                {
                    x.IsVisible = false;
                    mainGrid.Children.Remove(x);
                }));

            m_GameManager.CurrentPlayer.TakeTurn();
        }

        private void SetButtons()
        {
            backButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await GoToMainMenu())
            });
            startButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    try { StartGameAfterSelectingGaps(); }
                    catch (InvalidOperationException) { }
                })
            });
        }
    }
}