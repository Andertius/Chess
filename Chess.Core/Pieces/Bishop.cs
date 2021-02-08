using System;

namespace Chess.Core.Pieces
{
    /// <summary>
    /// Represents the <see cref="Bishop"/> piece and is derived from the <see cref="ChessPiece"/> class.
    /// </summary>
    public class Bishop : ChessPiece, IEquatable<Bishop>
    {
        /// <inheritdoc/>
        public Bishop(int x, int y, PieceColor color)
            : base(x, y, color, 3, Piece.Bishop) { }

        /// <inheritdoc/>
        public override bool CheckIfIsValidMove(int newX, int newY, Board board)
        {
            if (newX > -1 && newX < 8 && newY > -1 && newY < 8 && Color != board[newX, newY].OccupiedBy?.Color)
            {
                return IsValid(newX, newY, board);
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Protects(int x, int y, Board board)
        {
            if ((x != X || y != Y) && x > -1 && x < 8 && y > -1 && y < 8)
            {
                return IsValid(x, y, board);
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "B";
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Bishop bishop && Equals(bishop);
        }

        /// <inheritdoc/>
        public bool Equals(Bishop bishop)
        {
            return !(bishop is null) && X == bishop.X && Y == bishop.Y && Value == bishop.Value && Color == bishop.Color;
        }

        private bool IsValid(int x, int y, Board board)
        {
            for (int i = 0; X + i < 8 && Y + i < 8; i++)
            {
                if (x == X + i && y == Y + i)
                {
                    return IsValidUpRightMove(x, board);
                }
            }

            for (int i = 0; X + i < 8 && Y - i > -1; i++)
            {
                if (x == X + i && y == Y - i)
                {
                    return IsValidDownRightMove(x, board);
                }
            }

            for (int i = 0; X - i > -1 && Y - i > -1; i++)
            {
                if (x == X - i && y == Y - i)
                {
                    return IsValidDownLeftMove(x, board);
                }
            }

            for (int i = 0; X - i > -1 && Y + i < 8; i++)
            {
                if (x == X - i && y == Y + i)
                {
                    return IsValidUpLeftMove(x, board);
                }
            }

            return false;
        }

        private bool IsValidUpRightMove(int newX, Board board)
        {
            for (int i = X + 1, j = Y + 1; i < newX; i++, j++)
            {
                if (!(board[i, j].OccupiedBy is null))
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
                if (!(board[i, j].OccupiedBy is null))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidDownLeftMove(int newX, Board board)
        {
            for (int i = X - 1, j = Y - 1; i > newX; i--, j--)
            {
                if (!(board[i, j].OccupiedBy is null))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidUpLeftMove(int newX, Board board)
        {
            for (int i = X - 1, j = Y + 1; i > newX; i--, j++)
            {
                if (!(board[i, j].OccupiedBy is null))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
