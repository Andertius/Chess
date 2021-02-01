using System;

namespace Chess.Core.Pieces
{
    public class Knight : ChessPiece, IEquatable<Knight>
    {
        public Knight(int x, int y, PieceColor color)
            : base(x, y, color, 3) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (newX < 8 && newY < 8 && newX > -1 && newY > -1 && Color != board[newX, newY]?.Color)
            {
                if (newX == X + 1 && newY == Y + 2)
                {
                    return true;
                }
                else if (newX == X + 2 && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X + 2 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y - 2)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y - 2)
                {
                    return true;
                }
                else if (newX == X - 2 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X - 2 && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y + 2)
                {
                    return true;
                }
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
    }
}
