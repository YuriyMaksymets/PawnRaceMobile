using System;

namespace PawnRaceMobile.Core
{
    public static class ExtensionMethods
    {
        public static char Char(this Color color)
        {
            switch (color)
            {
                case Color.None:
                    return '.';

                case Color.Black:
                    return 'B';

                case Color.White:
                    return 'W';

                default:
                    return ' ';
            }
        }

        public static Color Inverse(this Color color)
        {
            switch (color)
            {
                case Color.White:
                    return Color.Black;

                case Color.Black:
                    return Color.White;

                case Color.None:
                    Console.WriteLine("Getting Inverse of empty color");
                    return Color.None;

                default:
                    return Color.None;
            }
        }
    }
}