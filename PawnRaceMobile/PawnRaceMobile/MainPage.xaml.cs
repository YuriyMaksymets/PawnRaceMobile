using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PawnRaceMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = ((j + i) % 2 == 0) ? new Image
                    {
                        Source = "blackfill.png",
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

            #region Previous tries

            //grid = new Grid();

            //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //var topLeft = new Label { Text = "Top Left" };
            //var topRight = new Label { Text = "Top Right" };
            //var bottomLeft = new Label { Text = "Bottom Left" };
            //var bottomRight = new Label { Text = "Bottom Right" };

            //grid.Children.Add(topLeft, 0, 0);
            //grid.Children.Add(topRight, 0, 1);
            //grid.Children.Add(bottomLeft, 1, 0);
            //grid.Children.Add(bottomRight, 1, 1);

            //for (int i = 0; i < 8; i++)
            //{
            //    mainGrid.RowDefinitions.Add(new RowDefinition
            //    {
            //        Height = new GridLength(1, GridUnitType.Auto)
            //    });
            //}
            //for (int i = 0; i < 8; i++)
            //{
            //    mainGrid.ColumnDefinitions.Add(new ColumnDefinition
            //    {
            //        Width = new GridLength(1, GridUnitType.Auto)
            //    });
            //}
            //var topLeft = new Label { Text = "Top Left" };
            //mainGrid.Children.Add(topLeft, 0, 0);

            //for (int i = 0; i < 8; i++)
            //{
            //    for (int j = 0; j < 8; j++)
            //    {
            //        Color randomColor
            //            = Color.FromRgb(m_Rnd.Next(256), m_Rnd.Next(256), m_Rnd.Next(256));
            //        BoxView boxView = new BoxView
            //        {
            //            BackgroundColor = randomColor
            //        };
            //        grid.Children.Add(boxView, i, j);
            //    }
            //}

            #endregion Previous tries
        }

        protected void OnSquareTapped(object sender, EventArgs e)
        {
            DisplayAlert("R: " + Grid.GetRow((Image)sender).ToString()
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