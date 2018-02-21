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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoardPage : ContentPage
    {
        #region Image Sources

        private readonly ImageSource r_BlackImageSource = ImageSource.FromResource
            ("PawnRaceMobile.Resourses.blackpawn.png", Assembly.GetExecutingAssembly());

        private readonly ImageSource r_WhiteImageSource = ImageSource.FromResource
            ("PawnRaceMobile.Resourses.whitepawn.png", Assembly.GetExecutingAssembly());

        private readonly ImageSource r_BlackFillSource = ImageSource.FromResource
           ("PawnRaceMobile.Resourses.blackfill.png", Assembly.GetExecutingAssembly());

        private readonly ImageSource r_WhiteFillSource = "whitefill.png";

        #endregion Image Sources

        private IList<Image> m_PawnImages = new List<Image>(14);
        private bool m_BoardRotated;

        protected override void OnSizeAllocated(double width, double height)

        {
            base.OnSizeAllocated(width, height);
            Console.WriteLine(width + " " + height);
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

        private void Alert(object obj) => DisplayAlert(obj.ToString(), "", "Close");

        private void InitializeBoardGrid()
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
                    mainGrid.Children.Add(image, i, m_BoardRotated ? Board.c_MAX_INDEX - j : j);
                }
            }
        }

        private void AddTapRecognition(View element, EventHandler action)
        {
            TapGestureRecognizer iconTap = new TapGestureRecognizer();
            iconTap.Tapped += action;
            element.GestureRecognizers.Add(iconTap);
        }

        private void Log(object obj) => console.Text += ('\n' + obj.ToString());

        private void OnClearButton(object sender, EventArgs e) => console.Text = "";

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
            m_PawnImages.Add(image);
            AddTapRecognition(image, OnPawnTapped);
            mainGrid.Children.Add(image, pawn.X
                , m_BoardRotated ? Board.c_MAX_INDEX - pawn.Y : pawn.Y);
        }

        private void RenderAllPawns()
        {
            m_PawnImages.ForEach(x => mainGrid.Children.Remove(x));
            m_PawnImages = new List<Image>(m_GameManager.Board.Pawns.Count);
            m_GameManager.Board.Pawns.ForEach(x => RenderPawn(x));
        }

        //private void RenderPawns(IEnumerable<Square> pawns)
        //{
        //    pawns.ForEach(x => RenderPawn(x));
        //}
    }
}