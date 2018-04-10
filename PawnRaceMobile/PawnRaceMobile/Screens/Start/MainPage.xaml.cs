using PawnRaceMobile.Screens;
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
            SetButtons();
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        private void SetButtons()
        {
            startGameButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnGameStart())
            });
            rulesButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnGameStart())
            });
        }

        private async void OnGameStart()
        {
            BoardPage boardPage = new BoardPage('a', 'a', true, false);
            boardPage.InitializeBackground();
            await Navigation.PushAsync(new ModeSelection());
        }
    }
}