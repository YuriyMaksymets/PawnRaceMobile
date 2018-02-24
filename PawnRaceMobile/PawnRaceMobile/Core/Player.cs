using System;
using System.Collections.Generic;
using System.Text;

namespace PawnRaceMobile.Core
{
    public abstract class Player : IPlayer
    {
        private readonly int r_MoveShift;
        private readonly int r_PassedY;
        private readonly Random r_Random = new Random();
        private readonly int r_YCannotAttack;
        private readonly int r_YForLongMove;
        private Game m_Game;

        internal Player Opponent
        {
            get; private set;
        }

        public Player(Color color)
        {
            Color = color;
            r_YForLongMove = IsWhite ? 1 : Board.c_MAX_INDEX - 1;
            r_YCannotAttack = Board.c_MAX_INDEX - r_YForLongMove;
            r_MoveShift = IsWhite ? 1 : -1;
            r_PassedY = IsWhite ? Board.c_MAX_INDEX : 0;
        }

        public event Action<IPlayer, Move> MoveProduced;

        public Board Board => m_Game.Board;

        public Color Color
        {
            get; private set;
        }

        public bool IsWhite => Color == Color.White;

        private IList<Square> Pawns
            => IsWhite ? Board.WhitePawns : Board.BlackPawns;

        public abstract Move ProduceMove();

        public abstract void TakeTurn();

        internal void Set(Game game, Player opponent)
        {
            m_Game = game ?? throw new ArgumentNullException(nameof(game));
            Opponent = opponent ?? throw new ArgumentNullException(nameof(opponent));
        }

        //If there is an obvious move, only it is returned
        internal IList<Move> CalculatePossibleMovesOptimized()
        {
            if (m_Game.NumberOfMoves > 0)
            {
                List<Move> maxMovesArray = new List<Move>(18);
                Move lastMove = m_Game.LastMove;
                bool lastMoveWasLong = lastMove.IsLong && lastMove.To.IsOccupiedBy(Opponent.Color);
                // int numOfMoves = 0;
                for (int i = 0; i < Pawns.Count; i++)
                {
                    Square pawn = Pawns[i];
                    int currentY = pawn.Y;
                    int currentX = pawn.X;
                    if (currentY == r_YCannotAttack)
                    {
                        return new Move[1]
                        {
                            new Move(pawn, Board.GetSquare(currentX, currentY + r_MoveShift))
                        };
                    }
                    if (currentY == r_PassedY)
                    {
                        return null;
                    }

                    int newY = currentY + r_MoveShift;
                    Square movePosition = Board.GetSquare(currentX, newY);
                    if (!movePosition.IsOccupied)
                    {
                        maxMovesArray.Add(new Move(pawn, movePosition));
                        //maxMovesArray[numOfMoves++] = new Move(pawn, movePosition);
                        if (currentY == r_YForLongMove)
                        {
                            int longMoveY = newY + r_MoveShift;
                            Square longMovePosition = Board.GetSquare(currentX, longMoveY);
                            if (!longMovePosition.IsOccupied)
                            {
                                maxMovesArray.Add(new Move(pawn, longMovePosition));
                                //maxMovesArray[numOfMoves++] = new Move(pawn, longMovePosition);
                            }
                        }
                    }

                    bool EpCapturePossible = lastMoveWasLong && lastMove.To.Y == currentY;
                    for (int attackShift = -1; attackShift <= 1; attackShift += 2)
                    {
                        int newX = currentX + attackShift;
                        if (newX < 0 || newX > Board.c_MAX_INDEX)
                        {
                            continue;
                        }
                        Square newSquare = Board.GetSquare(newX, newY);
                        if (newSquare.IsOccupiedBy(Opponent.Color))
                        {
                            maxMovesArray.Add(new Move(pawn, newSquare, true, false));
                            //maxMovesArray[numOfMoves++] = new Move(pawn, newSquare, true, false);
                        }
                        else if (EpCapturePossible && lastMove.To.X == newX)
                        {
                            maxMovesArray.Add(new Move(pawn, newSquare, true, true));
                            //maxMovesArray[numOfMoves++] = new Move(pawn, newSquare, true, true);
                        }
                    }
                }
                // Move[] moves = new Move[numOfMoves];
                //Array.Copy(maxMovesArray, moves, numOfMoves);
                return maxMovesArray;
            }
            else
            {
                IList<Move> moves = new List<Move>(14);
                //int numOfMoves = 0;
                Pawns.ForEach(pawn =>
                {
                    moves.Add(new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + r_MoveShift)));
                    moves.Add(new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + (r_MoveShift << 1))));
                    // moves[numOfMoves++]
                    //  = new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + r_MoveShift));
                    // moves[numOfMoves++]
                    //= new Move(pawn, Board.GetSquare(pawn.X, pawn.Y + (r_MoveShift << 1)));
                });
                return moves;
            }
        }

        protected void OnMoveProduced(Move move) => MoveProduced?.Invoke(this, move);

        protected Move SelectRandomMove(IList<Move> possibleMoves)
            => possibleMoves[r_Random.Next(possibleMoves.Count)];

        public IList<Move> GetAvailableMovesForPawn(Square pawn)
        {
            List<Move> movesList = new List<Move>();
            Move move;

            int colorType = 1;
            if (Color == Color.Black)
            {
                colorType = -1;
            }

            Square nextSquare = new Square(pawn.X - 1, pawn.Y + colorType);
            Board board = m_Game.Board;

            if (GameUtilis.ValidSquare(nextSquare, board))
            {

                nextSquare = board.GetSquare(nextSquare.X, nextSquare.Y);

                //Check if simple capture is possible
                if (GameUtilis.CheckSimpleCapture(nextSquare, Color))
                {
                    movesList.Add(new Move(pawn, nextSquare, true, false));
                }
                else
                {
                    //Check if enPassant capture
                    move = m_Game.LastMove;
                    if (move != null && move.IsEnPassant())
                    {
                        if (GameUtilis.CheckEnPassantCapture(pawn, nextSquare, board, Color, move))
                        {
                            movesList.Add(new Move(pawn, nextSquare, true, true));

                        }
                    }
                }
            }

            nextSquare = new Square(pawn.X + 1, pawn.Y + colorType);
            if (GameUtilis.ValidSquare(nextSquare, board))
            {

                nextSquare = board.GetSquare(nextSquare.X, nextSquare.Y);

                //Check if simple capture is possible
                if (GameUtilis.CheckSimpleCapture(nextSquare, Color))
                {
                    movesList.Add(new Move(pawn, nextSquare, true, false));

                }
                else
                {
                    //Check if enPassant capture
                    move = m_Game.LastMove;
                    if (move != null && move.IsEnPassant())
                    {
                        if (GameUtilis.CheckEnPassantCapture(pawn, nextSquare, board, Color, move))
                        {

                            movesList.Add(new Move(pawn, nextSquare, true, true));

                        }
                    }
                }
            }

            //Check for one step forward move
            nextSquare = new Square(pawn.X, pawn.Y + colorType);
            if (GameUtilis.ValidSquare(nextSquare, board))
            {

                nextSquare = board.GetSquare(nextSquare.X, nextSquare.Y);

                if (GameUtilis.CheckForwardMove(nextSquare))
                {

                    //Check for two steps forward move
                    Square nextSquare2 = new Square(pawn.X, pawn.Y + 2 * colorType);
                    if ((Color == Color.White && pawn.Y == 1) || (Color == Color.Black
                        && pawn.Y == board.Size - 2))
                    {
                        if (GameUtilis.ValidSquare(nextSquare2, board))
                        {

                            nextSquare2 = board.GetSquare(nextSquare2.X, nextSquare2.Y);

                            if (GameUtilis.CheckForwardMove(nextSquare2))
                            {

                                movesList.Add(new Move(pawn, nextSquare2, false, false));
                            }

                        }
                    }
                    movesList.Add(new Move(pawn, nextSquare, false, false));
                }
            }

            return movesList;

        }
    }
}