using System;

namespace Chess.Core.Pieces
{
    /// <summary>
    /// Represents the <see cref="Pawn"/> piece and is derived from the <see cref="ChessPiece"/> class.
    /// </summary>
    public class Pawn : ChessPiece, IEquatable<Pawn>
    {
        private bool enPassant;
        private int enPassantMoveNum = 0;

        /// <inheritdoc/>
        public Pawn(int x, int y, PieceColor color)
            : base(x, y, color, 1, Piece.Pawn)
        {
            CanBeEnPassanted = false;
        }

        /// <summary>
        /// Gets the value that indicates whether the <see cref="Pawn"/> was moved before.
        /// </summary>
        public bool IsMoved { get; private set; }

        /// <summary>
        /// Gets or sets the value that indicated whether the <see cref="Pawn"/> just captured another <see cref="Pawn"/> with an en passant.
        /// </summary>
        public bool JustEnPassanted { get; private set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the <see cref="Pawn"/> be captures by the en passant move.
        /// </summary>
        public bool CanBeEnPassanted {
            get
            {
                return enPassant;
            }
            set
            {
                if (value == true)
                {
                    enPassant = value;
                    enPassantMoveNum++;
                    return;
                }
                else if (enPassantMoveNum == 0)
                {
                    enPassant = value;
                }

                enPassantMoveNum = 0;
            }
        }

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

                if (JustEnPassanted && Color == PieceColor.White)
                {
                    capturedPiece = board[newX, newY - 1]?.OccupiedBy;
                }
                else if (JustEnPassanted && Color == PieceColor.Black)
                {
                    capturedPiece = board[newX, newY + 1]?.OccupiedBy;
                }

                Board.Occupy(board[newX, newY], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                if ((Color == PieceColor.White && X == newX && newY == Y + 2) ||
                    (Color == PieceColor.Black && X == newX && newY == Y - 2))
                {
                    CanBeEnPassanted = true;
                }
                else
                {
                    CanBeEnPassanted = false;
                }

                X = newX;
                Y = newY;

                IsMoved = true;

                if (Y == 7 || Y == 0)
                {
                    Promote(board);
                }

                board.CheckForProtection();
                return true;
            }

            capturedPiece = null;
            return false;
        }

        /// <inheritdoc/>
        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (Color == PieceColor.White)
            {
                if (newY == Y + 1 && newX == X - 1 && (!(board[X - 1, Y + 1].OccupiedBy is null) && board[X - 1, Y + 1].OccupiedBy.Color != Color ||
                    (board[X - 1, Y].OccupiedBy is Pawn leftPawn && leftPawn.Color != Color && leftPawn.CanBeEnPassanted)))
                {
                    if (board[X - 1, Y].OccupiedBy is Pawn leftPawn1 && leftPawn1.Color != Color && leftPawn1.CanBeEnPassanted)
                    {
                        JustEnPassanted = true;
                        return true;
                    }

                    JustEnPassanted = false;
                    return true;
                }
                else if (newY == Y + 1 && newX == X + 1 && (!(board[X + 1, Y + 1].OccupiedBy is null) && board[X + 1, Y + 1].OccupiedBy.Color != Color ||
                    (board[X + 1, Y].OccupiedBy is Pawn rightPawn && rightPawn.Color != Color && rightPawn.CanBeEnPassanted)))
                {
                    if (board[X + 1, Y].OccupiedBy is Pawn rightPawn1 && rightPawn1.Color != Color && rightPawn1.CanBeEnPassanted)
                    {
                        JustEnPassanted = true;
                        return true;
                    }

                    JustEnPassanted = false;
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y + 2 && newX == X && board[X, Y + 1].OccupiedBy is null && board[X, Y + 2].OccupiedBy is null)
                    {
                        JustEnPassanted = false;
                        return true;
                    }
                    else if (newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                    {
                        JustEnPassanted = false;
                        return true;
                    }
                }
                else if (IsMoved && newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                {
                    JustEnPassanted = false;
                    return true;
                }
            }
            else
            {
                if (newY == Y - 1 && newX == X - 1 && (!(board[X - 1, Y - 1].OccupiedBy is null) && board[X - 1, Y - 1].OccupiedBy.Color != Color ||
                    (board[X - 1, Y].OccupiedBy is Pawn leftPawn && leftPawn.Color == PieceColor.White && leftPawn.CanBeEnPassanted)))
                {
                    if (board[X - 1, Y].OccupiedBy is Pawn leftPawn1 && leftPawn1.Color == PieceColor.White && leftPawn1.CanBeEnPassanted)
                    {
                        JustEnPassanted = true;
                        return true;
                    }

                    JustEnPassanted = false;
                    return true;
                }
                else if (newY == Y - 1 && newX == X + 1 && (!(board[X + 1, Y - 1].OccupiedBy is null) && board[X + 1, Y - 1].OccupiedBy.Color != Color ||
                    (board[X + 1, Y].OccupiedBy is Pawn rightPawn && rightPawn.Color == PieceColor.White && rightPawn.CanBeEnPassanted)))
                {
                    if (board[X + 1, Y].OccupiedBy is Pawn rightPawn1 && rightPawn1.Color == PieceColor.White && rightPawn1.CanBeEnPassanted)
                    {
                        JustEnPassanted = true;
                        return true;
                    }

                    JustEnPassanted = false;
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y - 2 && newX == X && board[X, Y - 1].OccupiedBy is null && board[X, Y - 2].OccupiedBy is null)
                    {
                        JustEnPassanted = false;
                        return true;
                    }
                    else if (newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                    {
                        JustEnPassanted = false;
                        return true;
                    }
                }
                else if (IsMoved && newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                {
                    JustEnPassanted = false;
                    return true;
                }
            }

            JustEnPassanted = false;
            return false;
        }

        /// <inheritdoc/>
        public override bool Protects(int x, int y, Board board)
        {
            if (Color == PieceColor.White && (x == X + 1 && y == Y + 1 || x == X - 1 && y == Y + 1))
            {
                return true;
            }
            else if (Color == PieceColor.Black && (x == X + 1 && y == Y - 1 || x == X - 1 && y == Y - 1))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "p";
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Color, Value, IsMoved);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Pawn pawn && Equals(pawn);
        }

        /// <inheritdoc/>
        public bool Equals(Pawn pawn)
        {
            return !(pawn is null) && 
                X == pawn.X &&
                Y == pawn.Y &&
                Color == pawn.Color &&
                Value == pawn.Value &&
                IsMoved == pawn.IsMoved &&
                CanBeEnPassanted == pawn.CanBeEnPassanted;
        }

        private void Promote(Board board)
        {
            board[X, 7].Occupy(GameHandler.RequestPromotion(this));
        }
    }
}
