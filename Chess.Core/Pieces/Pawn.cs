using System;

namespace Chess.Core.Pieces
{
    public class Pawn : ChessPiece, IEquatable<Pawn>
    {
        public Pawn(int x, int y, PieceColor color)
            : base(x, y, color, 1) { }

        public bool IsMoved { get; private set; }

        public bool IsPromoted { get; private set; }

        public override bool Move(int newX, int newY, Board board)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                board[X, Y].BlankOccupied();
                X = newX;
                Y = newY;

                if (Color == PieceColor.White)
                {
                    board[X, Y].WhiteOccupied();
                }
                else
                {
                    board[X, Y].BlackOccupied();
                }

                IsMoved = true;

                if (Y == 7)
                {
                    Promote();
                }

                return true;
            }

            return false;
        }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (Color == PieceColor.White)
            {
                if (newY == Y + 1 && (newX == X + 1 || newX == X - 1) &&
                    (!(board[X + 1, Y + 1].OccupiedBy is null) && board[X + 1, Y + 1].OccupiedBy != Color ||
                    !(board[X + 1, Y + 1].OccupiedBy is null) && board[X - 1, Y + 1].OccupiedBy != Color))
                {
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y + 2 && newX == X && board[X, Y + 1].OccupiedBy is null && board[X, Y + 2].OccupiedBy is null)
                    {
                        return true;
                    }
                    else if (newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                    {
                        return true;
                    }
                }
                else if (IsMoved && newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                {
                    return true;
                }
            }
            else
            {
                if (newY == Y - 1 && (newX == X + 1 || newX == X - 1) &&
                    (!(board[X + 1, Y - 1].OccupiedBy is null) && board[X + 1, Y - 1].OccupiedBy != Color ||
                    !(board[X + 1, Y - 1].OccupiedBy is null) && board[X - 1, Y - 1].OccupiedBy != Color))
                {
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y - 2 && newX == X && board[X, Y - 1].OccupiedBy is null && board[X, Y - 2].OccupiedBy is null)
                    {
                        return true;
                    }
                    else if (newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                    {
                        return true;
                    }
                }
                else if (IsMoved && newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return "p";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Color, Value, IsMoved);
        }

        public override bool Equals(object obj)
        {
            return obj is Pawn pawn && Equals(pawn);
        }

        public bool Equals(Pawn pawn)
        {
            return X == pawn.X && Y == pawn.Y && Color == pawn.Color &&
                Value == pawn.Value && IsMoved == pawn.IsMoved;
        }

        private void Promote()
        {
            throw new NotImplementedException();
        }
    }
}
