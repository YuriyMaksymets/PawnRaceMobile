using PawnRaceMobile.Core;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;

namespace PawnRaceMobile
{
    public partial class BoardPage : ContentPage
    {
        private const int c_InitialBlackGapIndex = 0;
        private const int c_InitialWhiteGapIndex = 0;

        private Image[] m_BloackedImages;

        private void RenderDummyPawns()
        {
            m_PawnImages = new Dictionary<Square, Image>(16);
            m_BloackedImages = new Image[2];
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
                if (i == c_InitialWhiteGapIndex)
                {
                    whiteImage.IsVisible = false;
                    m_BloackedImages[0] = new Image
                    {
                        Source = r_YellowPointImageSource
                    };
                    mainGrid.Children.Add(m_BloackedImages[0], i, actualWhiteY);
                }
                if (i == c_InitialBlackGapIndex)
                {
                    blackImage.IsVisible = false;
                    m_BloackedImages[1] = new Image
                    {
                        Source = r_YellowPointImageSource
                    };
                    mainGrid.Children.Add(m_BloackedImages[1], i, actualBlackY);
                }
            }
        }

        private void LinkDummyPawns()
        {
            Dictionary<Square, Image> newPawnImages = new Dictionary<Square, Image>(14);
            m_PawnImages.Keys.ForEach(key =>
            {
                Square newSquare = m_GameManager.Board.Pawns
                .Where(pawn => pawn.X == key.X && pawn.Y == key.Y)
                .First();
                Image image = m_PawnImages[key];
                image.GestureRecognizers.Clear();
                AddTapRecognition(image, OnPawnTapped);
                newPawnImages.Add(newSquare, newPawnImages[key]);
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
            int senderX = Grid.GetColumn(senderImage);
            Image blockedImage = YBasedOnBoardRotation(senderY) == 1
                ? m_BloackedImages[0] : m_BloackedImages[1];
            mainGrid.Children.Add(blockedImage, senderX, senderY);
        }
    }
}