using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PawnRaceMobile
{
    public partial class GameStartSettingsPage : ContentPage
    {
        public GameStartSettingsPage(BoardPage boardPage)
        {
            InitializeComponent();
            SetNavBar();
            InitialSetting(boardPage);
            backButton.Clicked += async (sender, e) => await Navigation.PopAsync();
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
    }
}