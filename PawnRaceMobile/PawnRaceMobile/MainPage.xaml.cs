using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PawnRaceMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            SetNavBar();
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        private async void OnGameStart(object sender, EventArgs e)
        {
            BoardPage boardPage = new BoardPage('a', 'c', true, false);
            boardPage.InitializeBackground();
            await Navigation.PushAsync(new GameStartSettingsPage(boardPage));
        }
    }
}