using Xamarin.Forms;

namespace PawnRaceMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Color dark = (Color)Current.Resources["dark"];
            NavigationPage mainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = dark
            };
            MainPage = mainPage;
        }

        protected override void OnStart() => Global.Instance.InitializeBoardPage();

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}