namespace PawnRaceMobile.Core
{
    internal class BoardUtilis
    {
        public static Color enemyColor(Color c)
        {
            switch (c)
            {
                case Color.White:
                    return Color.Black;

                case Color.Black:
                    return Color.White;

                default:
                    return Color.None;
            }
        }

        public static int getIntFromLetter(char x)
        {
            if (x >= 'a' && x <= 'z')
            {
                return x - 'a';
            }

            return x - 'A';
        }
    }
}