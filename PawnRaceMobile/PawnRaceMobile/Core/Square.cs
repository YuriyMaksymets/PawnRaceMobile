namespace PawnRaceMobile.Core
{
    public struct Square
    {
        public Color Color
        {
            get; set;
        }

        public bool IsOccupied => Color != Color.NONE;

        public string Notation => ((char)('a' + X)) + (Y + 1).ToString();

        public byte X
        {
            get; private set;
        }

        public byte Y
        {
            get; private set;
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

        public bool IsOccupiedBy(Color color) => color == Color;

        public override string ToString() => "Square " + X + " " + Y;
    }
}