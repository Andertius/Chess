using System;

namespace Chess.Core.Pieces
{
    public class King : ChessPiece, IEquatable<King>
    {
        public King(int x, int y, PieceColor color)
            : base(x, y, color, KingValue) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if ((Color == PieceColor.White && !board[newX, newY].IsBlackProtected) ||
                Color == PieceColor.Black && !board[newX, newY].IsWhiteProtected)
            {
                if (newX < 8 && newY < 8 && newX > -1 && newY > -1 && Color != board[newX, newY].OccupiedBy?.Color)
                {
                    return IsValid(newX, newY);
                }
            }

            return false;
        }

        public override bool Protects(int x, int y, Board board)
        {
            if ((x != X || y != Y) && x < 8 && y < 8 && x > -1 && y > -1)
            {
                return IsValid(x, y);
            }

            return false;
        }

        public override string ToString()
        {
            return "K";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        public override bool Equals(object obj)
        {
            return obj is King king && Equals(king);
        }

        public bool Equals(King king)
        {
            return X == king.X && Y == king.Y && Value == king.Value && Color == king.Color;
        }

        private bool IsValid(int x, int y)
        {
            if (x == X && y == Y + 1)
            {
                return true;
            }
            else if (x == X + 1 && y == Y + 1)
            {
                return true;
            }
            else if (x == X + 1 && y == Y)
            {
                return true;
            }
            else if (x == X + 1 && y == Y - 1)
            {
                return true;
            }
            else if (x == X && y == Y - 1)
            {
                return true;
            }
            else if (x == X - 1 && y == Y - 1)
            {
                return true;
            }
            else if (x == X - 1 && y == Y)
            {
                return true;
            }
            else if (x == X - 1 && y == Y + 1)
            {
                return true;
            }

            return false;
        }
    }
}
