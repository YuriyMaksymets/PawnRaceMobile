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
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
    }
}