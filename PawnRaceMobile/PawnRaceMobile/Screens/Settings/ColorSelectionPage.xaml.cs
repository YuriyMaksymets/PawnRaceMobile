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
            SetButtons();
            SetNavBar();
        }

        private void SetButtons()
        {
            whiteButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async ()
                =>
                {
                    Global.Instance.BoardPage.Initialize(true, false);
                    await Navigation.PushAsync(Global.Instance.BoardPage);
                }
                )
            });
            blackButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async ()
                =>
                {
                    Global.Instance.BoardPage.Initialize(false, false);
                    await Navigation.PushAsync(Global.Instance.BoardPage);
                }
                )
            });
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
    }
}