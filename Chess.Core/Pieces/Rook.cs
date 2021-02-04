using System;

namespace Chess.Core.Pieces
{
    /// <summary>
    /// Represents the <see cref="Rook"/> piece and is derived from the <see cref="ChessPiece"/> class.
    /// </summary>
    public class Rook : ChessPiece, IEquatable<Rook>
    {

        /// <inheritdoc/>
        public Rook(int x, int y, PieceColor color)
            : base(x, y, color, 5, Piece.Rook) { }

        /// <summary>
        /// Gets the value that indicates whether the <see cref="Rook"/> piece was moved before.
        /// </summary>
        public bool IsMoved { get; private set; }

        /// <inheritdoc/>
        public override bool Move(int newX, int newY, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                if (!isMock && CheckForChecksAfterMove(newX, newY, board))
                {
                    capturedPiece = null;
                    return false;
                }

                JustLongCastled = false;
                JustShortCastled = false;

                capturedPiece = board[newX, newY]?.OccupiedBy;

                Board.Occupy(board[newX, newY], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
                Y = newY;

                IsMoved = true;

                board.CheckForProtection();
                return true;
            }

            capturedPiece = null;
            return false;
        }

        /// <summary>
        /// Castles the <see cref="Rook"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Rook"/> will castle no matter what, so check for castling conditions before invoking this method.
        /// </remarks>
        /// <param name="isHeadingLeft">A value that indicates if the <see cref="Rook"/>
        /// is going to the right or to the left in respect to the player.</param>
        /// <param name="board">The board in which the castling is happening.</param>
        internal void Castle(bool isHeadingLeft, Board board)
        {
            if (Color == PieceColor.White)
            {
                int newX = isHeadingLeft ? X - 2 : X + 3;

                Board.Occupy(board[newX, Y], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
            }
            else
            {
                int newX = isHeadingLeft ? X + 3 : X - 2;

                Board.Occupy(board[newX, Y], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
            }

            IsMoved = true;
        }

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
            return "R";
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Color, Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Rook rook && Equals(rook);
        }

        /// <inheritdoc/>
        public bool Equals(Rook rook)
        {
            return !(rook is null) && X == rook.X && Y == rook.Y && Value == rook.Value && Color == rook.Color && IsMoved == rook.IsMoved;
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

            return false;
        }
    }
}
