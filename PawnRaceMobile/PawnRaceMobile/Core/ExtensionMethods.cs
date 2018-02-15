using System;

namespace PawnRaceMobile.Core
{
    public static class ExtensionMethods
    {
        public static char Char(this Color color)
        {
            switch (color)
            {
                case Color.NONE:
                    return '.';

                case Color.BLACK:
                    return 'B';

                case Color.WHITE:
                    return 'W';

                default:
                    return ' ';
            }
        }

        public static Color Inverse(this Color color)
        {
            switch (color)
            {
                case Color.WHITE:
                    return Color.BLACK;

                case Color.BLACK:
                    return Color.WHITE;

                case Color.NONE:
                    Console.WriteLine("Getting Inverse of empty color");
                    return Color.NONE;

                default:
                    return Color.NONE;
            }
        }
    }
}