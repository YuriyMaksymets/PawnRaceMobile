using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System;

namespace PawnRaceMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RulesPage : ContentPage
    {
        private readonly Uri r_AuthorUri1 = new Uri("https://github.com/dragosmartac");
        private readonly Uri r_AuthorUri2 = new Uri("https://github.com/YuriyMaksymets");

        public RulesPage()
        {
            InitializeComponent();
            SetNavBar();
            backButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await Navigation.PopAsync())
            });
            authorLink1.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Device.OpenUri(r_AuthorUri1))
            });
            authorLink2.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Device.OpenUri(r_AuthorUri2))
            });
        }

        private void SetNavBar() => Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
    }
}