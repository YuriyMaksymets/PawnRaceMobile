using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PawnRaceMobile.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorSelectionPage : ContentPage
    {
        public ColorSelectionPage()
        {
            InitializeComponent();
            SetNavBar();
            SetButtons();
        }

        private void SetButtons()
        {
            onePlayerButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async ()
                => await Navigation.PushAsync(new BoardPage('a', 'a', true, false)))
            });
            twoPlayersButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async ()
                => await Navigation.PushAsync(new BoardPage('a', 'a', true, false)))
            });
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
    }
}