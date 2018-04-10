using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PawnRaceMobile.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModeSelection : ContentPage
    {
        public ModeSelection()
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
            onePlayerButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => GoOn(false))
            });
            twoPlayersButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => GoOn(true))
            });
        }

        private async void GoOn(bool multiplayer)
        {
            if (multiplayer)
            {
                await Navigation.PushAsync(new GapSelectionPage());
            }
            else
            {
                await Navigation.PushAsync(new ColorSelectionPage());
            }
        }
    }
}