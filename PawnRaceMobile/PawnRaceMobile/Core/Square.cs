namespace PawnRaceMobile.Core
{
    public class Square
    {
        public Color Color
        {
            get; set;
        }

        public bool IsBlack => IsOccupiedBy(Color.Black);
        public bool IsOccupied => Color != Color.None;
        public bool IsWhite => IsOccupiedBy(Color.White);
        public string Notation => ((char)('a' + X)) + (Y + 1).ToString();

        public int X
        {
            get; private set;
        }

        public int Y
        {
            get; private set;
        }

        public Square(int x, int y) : this(x, y, Color.None)
        {
        }

        public Square(int x, int y, Color color)
        {
            //    assert X<Board.MAX_COORDINATE
            //&& y < Board.MAX_COORDINATE && X >= 0 && y >= 0
            //      : "Wrong square coordinate";
            X = x;
            Y = y;
            Color = color;
        }

        public bool IsOccupiedBy(Color color) => color == Color;

        public override string ToString() => "Square " + X + " " + Y;

        public bool Equals(Square otherSquare) => X == otherSquare.X && Y == otherSquare.Y;
    }
}