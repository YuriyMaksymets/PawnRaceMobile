using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PawnRaceMobile
{
    public partial class GameStartSettingsPage : ContentPage
    {
        public GameStartSettingsPage(BoardPage boardPage)
        {
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            m_WhiteColorSelected = true;
            m_LocalMultiplayer = true;
            blackGapPicker.SelectedIndex = 0;
            whiteGapPicker.SelectedIndex = 0;
            m_BoardPage = boardPage;
            backButton.Clicked += async (sender, e) => await Navigation.PopAsync();
        }
    }
}