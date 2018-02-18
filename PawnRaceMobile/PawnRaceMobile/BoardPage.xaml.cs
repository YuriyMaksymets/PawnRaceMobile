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
        private Board m_Board;

        public BoardPage()
        {
            InitializeComponent();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = ((j + i) % 2 == 0) ? new Image
                    {
                        Source = ImageSource.FromResource
                        (
                            "PawnRaceMobile.Resourses.blackfill.png"
                            , Assembly.GetExecutingAssembly()
                        ),
                        Aspect = Aspect.AspectFill,
                    } :
                    new Image
                    {
                        Source = "whitefill.png",
                        Aspect = Aspect.AspectFill,
                    };
                    TapGestureRecognizer iconTap = new TapGestureRecognizer();
                    iconTap.Tapped += OnSquareTapped;
                    image.GestureRecognizers.Add(iconTap);
                    mainGrid.Children.Add(image, i, j);
                }
            }

            m_Board = new Board('a', 'b');
            m_Board.BlackPawns.ForEach(x =>
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

        protected void OnSquareTapped(object sender, EventArgs e)
        {
            DisplayAlert("R: " + Grid.GetRow((Image)sender).ToString()
                + " C: " + Grid.GetColumn((Image)sender).ToString()
                , "-----------------", "Close");
        }

        protected void OnPawnTapped(object sender, EventArgs e)
        {
            DisplayAlert("Pawn SUKA BLYAT R: " + Grid.GetRow((Image)sender).ToString()
                + " C: " + Grid.GetColumn((Image)sender).ToString()
                , "-----------------", "Close");
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Console.WriteLine(width + " " + height);

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
    }
}