using System;

namespace Chess.Core.Pieces
{
    public class King : ChessPiece, IEquatable<King>
    {
        private bool isCasltingLeft;
        private bool isCasltingRight;

        public King(int x, int y, PieceColor color)
            : base(x, y, color, KingValue) { }

        public bool IsMoved { get; private set; }

        public override bool Move(int newX, int newY, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                if (isCasltingRight)
                {
                    if (Color == PieceColor.White)
                    {
                        var rook = (Rook)board[7, 0].OccupiedBy;
                        rook.Castle(true, board);
                    }
                    else
                    {
                        var rook = (Rook)board[0, 7].OccupiedBy;
                        rook.Castle(true, board);
                    }
                }
                else if (isCasltingLeft)
                {
                    if (Color == PieceColor.White)
                    {
                        var rook = (Rook)board[0, 0].OccupiedBy;
                        rook.Castle(false, board);
                    }
                    else
                    {
                        var rook = (Rook)board[7, 7].OccupiedBy;
                        rook.Castle(false, board);
                    }
                }

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

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if ((Color == PieceColor.White && !board[newX, newY].IsBlackProtected) ||
                Color == PieceColor.Black && !board[newX, newY].IsWhiteProtected)
            {
                if (newX < 8 && newY < 8 && newX > -1 && newY > -1 && Color != board[newX, newY].OccupiedBy?.Color)
                {
                    return IsValid(newX, newY, out isCasltingLeft, out isCasltingRight, board);
                }
            }

            return false;
        }

        public override bool Protects(int x, int y, Board board)
        {
            if ((x != X || y != Y) && x < 8 && y < 8 && x > -1 && y > -1)
            {
                return IsValid(x, y, out _, out _, board);
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

        private bool IsValid(int x, int y, out bool castlingLeft, out bool castlingRight, Board board)
        {
            castlingLeft = false;
            castlingRight = false;

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
            else if (IsCastling(x, y, out castlingLeft, out castlingRight, board))
            {
                return true;
            }

            return false;
        }

        private bool IsCastling(int x, int y, out bool left, out bool right, Board board)
        {
            if (Color == PieceColor.White && !IsMoved && !board[4, 0].IsBlackProtected)
            {
                if (x == X + 2 && y == Y && board[5, 0].OccupiedBy is null && !board[5, 0].IsBlackProtected &&
                    board[6, 0].OccupiedBy is null && board[7, 0].OccupiedBy is Rook rightRook && !rightRook.IsMoved)
                {
                    left = false;
                    right = true;

                    return true;
                }
                else if (x == X - 2 && y == Y && board[3, 0].OccupiedBy is null && !board[3, 0].IsBlackProtected &&
                    board[2, 0].OccupiedBy is null && board[1, 0].OccupiedBy is null &&
                    board[0, 0].OccupiedBy is Rook leftRook && !leftRook.IsMoved)
                {
                    left = true;
                    right = false;

                    return true;
                }
            }
            else if(!IsMoved && !board[4, 7].IsWhiteProtected)
            {
                if (x == X + 2 && y == Y && board[5, 7].OccupiedBy is null && !board[5, 7].IsWhiteProtected &&
                    board[6, 7].OccupiedBy is null && board[7, 7].OccupiedBy is Rook leftRook && !leftRook.IsMoved)
                {
                    left = true;
                    right = false;

                    return true;
                }
                else if (x == X - 2 && y == Y && board[3, 7].OccupiedBy is null && !board[3, 7].IsWhiteProtected &&
                    board[2, 7].OccupiedBy is null && board[1, 7].OccupiedBy is null &&
                    board[0, 7].OccupiedBy is Rook rightRook && !rightRook.IsMoved)
                {
                    left = false;
                    right = true;

                    return true;
                }
            }

            left = false;
            right = false;

            return false;
        }
    }
}
