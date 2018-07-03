namespace PawnRaceMobile.Core
{
    internal class BoardUtilis
    {
        public static Color EnemyColor(Color c)
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

        public static int GetIntFromLetter(char x)
        {
            if (x >= 'a' && x <= 'z')
            {
                return x - 'a';
            }

            return x - 'A';
        }
    }
}