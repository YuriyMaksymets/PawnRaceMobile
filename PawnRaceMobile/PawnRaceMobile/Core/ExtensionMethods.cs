using System;
using System.Collections.Generic;

namespace PawnRaceMobile.Core
{
    public static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

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

                default:
                    return Color.None;
            }
        }
    }
}