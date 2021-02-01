using System;

namespace Chess.Core.Pieces
{
    public class Knight : ChessPiece, IEquatable<Knight>
    {
        public Knight(int x, int y, PieceColor color)
            : base(x, y, color, 3) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (newX < 8 && newY < 8 && newX > -1 && newY > -1 && Color != board[newX, newY].OccupiedBy?.Color)
            {
                return IsValid(newX, newY);
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
            return "k";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Color, Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Knight knight && Equals(knight);
        }

        public bool Equals(Knight knight)
        {
            return X == knight.X && Y == knight.Y && Value == knight.Value && Color == knight.Color;
        }

        private bool IsValid(int x, int y)
        {
            if (x == X + 1 && y == Y + 2)
            {
                return true;
            }
            else if (x == X + 2 && y == Y + 1)
            {
                return true;
            }
            else if (x == X + 2 && y == Y - 1)
            {
                return true;
            }
            else if (x == X + 1 && y == Y - 2)
            {
                return true;
            }
            else if (x == X - 1 && y == Y - 2)
            {
                return true;
            }
            else if (x == X - 2 && y == Y - 1)
            {
                return true;
            }
            else if (x == X - 2 && y == Y + 1)
            {
                return true;
            }
            else if (x == X - 1 && y == Y + 2)
            {
                return true;
            }

            return false;
        }
    }
}
