namespace PawnRaceMobile
{
    public class Global
    {
#pragma warning disable IDE1006 // Naming Styles
        public static readonly Global Instance = new Global();
#pragma warning restore IDE1006 // Naming Styles

        private Global()
        {
        }

        public BoardPage BoardPage
        {
            get; private set;
        }

        public void InitializeBoardPage() => BoardPage = new BoardPage();
    }
}