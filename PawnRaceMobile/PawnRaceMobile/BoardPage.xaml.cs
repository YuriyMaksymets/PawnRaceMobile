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

        private void InitializeBoardGrid()
        {
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
        }

        private void OnClearButton(object sender, EventArgs e) => console.Text = "";

        private void Alert(object obj) => DisplayAlert(obj.ToString(), "", "Close");

        private void Log(object obj) => console.Text += ('\n' + obj.ToString());
    }
}