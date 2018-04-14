namespace PawnRaceMobile.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new PawnRaceMobile.App());
        }
    }
}