using System;

namespace Chess.Core.Pieces
{
    public class Bishop : ChessPiece, IEquatable<Bishop>
    {
        public Bishop(int x, int y, PieceColor color)
            : base(x, y, color, 3) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (newX > -1 && newX < 8 && newY > -1 && newY < 8 && Color != board[newX, newY]?.Color)
            {
                for (int i = 0; X + i < 8 && Y + i < 8; i++)
                {
                    if (newX == X + i && newY == Y + i)
                    {
                        return IsValidUpRightMove(newX, board);
                    }
                }

                for (int i = 0; X + i < 8 && Y - i > -1; i++)
                {
                    if (newX == X + i && newY == Y - i)
                    {
                        return IsValidDownRightMove(newX, board);
                    }
                }

                for (int i = 0; X - i > -1 && Y - i > -1; i++)
                {
                    if (newX == X - i && newY == Y - i)
                    {
                        return IsValidDownLeftMove(newX, board);
                    }
                }

                for (int i = 0; X - i > -1 && Y + i < 8; i++)
                {
                    if (newX == X - i && newY == Y + i)
                    {
                        return IsValidUpLeftMove(newX, board);
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            return "B";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        public override bool Equals(object obj)
        {
            return obj is Bishop bishop && Equals(bishop);
        }

        public bool Equals(Bishop bishop)
        {
            return X == bishop.X && Y == bishop.Y && Value == bishop.Value && Color == bishop.Color;
        }

        private bool IsValidUpRightMove(int newX, Board board)
        {
            for (int i = X + 1, j = Y + 1; i < newX; i++, j++)
            {
                if (!(board[i, j] is null))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidDownRightMove(int newX, Board board)
        {
            for (int i = X + 1, j = Y - 1; i < newX; i++, j--)
            {
                if (!(board[i, j] is null))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidDownLeftMove(int newX, Board board)
        {
            for (int i = X - 1, j = Y - 1; i < newX; i--, j--)
            {
                if (!(board[i, j] is null))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidUpLeftMove(int newX, Board board)
        {
            for (int i = X - 1, j = Y + 1; i < newX; i--, j++)
            {
                if (!(board[i, j] is null))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
