using PawnRaceMobile.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        }

        private async void OnGameStart(object sender, EventArgs e)
            => await Navigation.PushAsync(new GameStartSettingsPage());
    }
}