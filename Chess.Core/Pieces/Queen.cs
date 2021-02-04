using System;

namespace Chess.Core.Pieces
{
    /// <summary>
    /// Represents the <see cref="Queen"/> piece and is derived from the <see cref="ChessPiece"/> class.
    /// </summary>
    public class Queen : ChessPiece, IEquatable<Queen>
    {
        /// <inheritdoc/>
        public Queen(int x, int y, PieceColor color)
            : base(x, y, color, 9, Piece.Queen) { }

        /// <inheritdoc/>
        public override bool IsValidMove(int newX, int newY, Board board)
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
            return "Q";
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Queen queen && Equals(queen);
        }

        /// <inheritdoc/>
        public bool Equals(Queen queen)
        {
            return !(queen is null) && X == queen.X && Y == queen.Y && Value == queen.Value && Color == queen.Color;
        }

        private bool IsValid(int x, int y, Board board)
        {
            if (x == X)
            {
                if (y > Y)
                {
                    for (int i = Y + 1; i < y; i++)
                    {
                        if (!(board[X, i].OccupiedBy is null))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = Y - 1; i > y; i--)
                    {
                        if (!(board[X, i].OccupiedBy is null))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else if (y == Y)
            {
                if (x > X)
                {
                    for (int i = X + 1; i < x; i++)
                    {
                        if (!(board[i, Y].OccupiedBy is null))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = X - 1; i > x; i--)
                    {
                        if (!(board[i, Y].OccupiedBy is null))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

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
