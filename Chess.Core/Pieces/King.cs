using System;

namespace Chess.Core.Pieces
{
    public class King : ChessPiece, IEquatable<King>
    {
        public King(int x, int y, PieceColor color)
            : base(x, y, color, KingValue) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (newX < 8 && newY < 8 && newX > -1 && newY > -1 && Color != board[newX, newY].OccupiedBy?.Color)
            {
                if (newX == X && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y + 1)
                {
                    return true;
                }
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
    }
}
