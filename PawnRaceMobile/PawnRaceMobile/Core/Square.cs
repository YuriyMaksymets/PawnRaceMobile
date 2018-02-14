namespace PawnRaceMobile.Core
{
    public struct Square
    {
        public byte X
        {
            get; private set;
        }

        public byte Y
        {
            get; private set;
        }

        public Color Color
        {
            get; set;
        }

        public Square(int x, int y) : this(x, y, Color.NONE)
        {
        }

        public Square(int x, int y, Color color)
        {
            //    assert X<Board.MAX_COORDINATE
            //&& y < Board.MAX_COORDINATE && X >= 0 && y >= 0
            //      : "Wrong square coordinate";
            X = (byte)x;
            Y = (byte)y;
            Color = color;
        }

        public bool IsOccupied() => Color != Color.NONE;

        public bool IsOccupiedBy(Color color) => color == Color;

        public string Notation => ('a' + X) + (Y + 1).ToString();
    }
}