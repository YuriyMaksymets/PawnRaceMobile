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
                Command = new Command(() => StartGame())
            });
            rulesButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => StartGame())
            });
        }

        private async void StartGame() => await Navigation.PushAsync(new ModeSelection());
    }
}