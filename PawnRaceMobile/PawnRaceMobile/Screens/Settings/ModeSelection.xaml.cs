using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        private void SetNavBar() => NavigationPage.SetHasNavigationBar(this, false);

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
                Global.Instance.BoardPage.Initialize(true, true);
                await Navigation.PushAsync(Global.Instance.BoardPage);
            }
            else
            {
                await Navigation.PushAsync(new ColorSelectionPage());
            }
        }
    }
}