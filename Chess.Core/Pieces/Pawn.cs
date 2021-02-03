using System;

namespace Chess.Core.Pieces
{
    public class Pawn : ChessPiece, IEquatable<Pawn>
    {
        private bool enPassant;
        private int enPassantMoveNum = 0;

        public Pawn(int x, int y, PieceColor color)
            : base(x, y, color, 1, Piece.Pawn)
        {
            CanBeEnPassanted = false;
        }

        public bool IsMoved { get; private set; }

        public bool IsPromoted { get; private set; }

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

                if (Y == 7)
                {
                    Promote(board);
                }

                board.CheckForProtection();
                return true;
            }

            capturedPiece = null;
            return false;
        }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (Color == PieceColor.White)
            {
                if (newY == Y + 1 && newX == X - 1 &&
                    (!(board[X - 1, Y + 1].OccupiedBy is null) && board[X - 1, Y + 1].OccupiedBy.Color != Color ||
                      (board[X - 1, Y].OccupiedBy is Pawn leftPawn && leftPawn.Color != Color && leftPawn.CanBeEnPassanted)))
                {
                    return true;
                }
                else if (newY == Y + 1 && newX == X + 1 &&
                         (!(board[X + 1, Y + 1].OccupiedBy is null) && board[X + 1, Y + 1].OccupiedBy.Color != Color ||
                           (board[X + 1, Y].OccupiedBy is Pawn rightPawn && rightPawn.Color != Color && rightPawn.CanBeEnPassanted)))
                {
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y + 2 && newX == X && board[X, Y + 1].OccupiedBy is null && board[X, Y + 2].OccupiedBy is null)
                    {
                        return true;
                    }
                    else if (newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                    {
                        return true;
                    }
                }
                else if (IsMoved && newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                {
                    return true;
                }
            }
            else
            {
                if (newY == Y - 1 && newX == X - 1 &&
                    (!(board[X - 1, Y - 1].OccupiedBy is null) && board[X - 1, Y - 1].OccupiedBy.Color != Color ||
                      (board[X - 1, Y].OccupiedBy is Pawn leftPawn && leftPawn.Color == PieceColor.White && leftPawn.CanBeEnPassanted)))
                {
                    return true;
                }
                else if (newY == Y - 1 && newX == X + 1 &&
                         (!(board[X + 1, Y - 1].OccupiedBy is null) && board[X + 1, Y - 1].OccupiedBy.Color != Color ||
                           (board[X + 1, Y].OccupiedBy is Pawn rightPawn && rightPawn.Color == PieceColor.White && rightPawn.CanBeEnPassanted)))
                {
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y - 2 && newX == X && board[X, Y - 1].OccupiedBy is null && board[X, Y - 2].OccupiedBy is null)
                    {
                        return true;
                    }
                    else if (newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                    {
                        return true;
                    }
                }
                else if (IsMoved && newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                {
                    return true;
                }
            }

            return false;
        }

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

        public override string ToString()
        {
            return "p";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Color, Value, IsMoved);
        }

        public override bool Equals(object obj)
        {
            return obj is Pawn pawn && Equals(pawn);
        }

        public bool Equals(Pawn pawn)
        {
            return X == pawn.X && Y == pawn.Y && Color == pawn.Color &&
                Value == pawn.Value && IsMoved == pawn.IsMoved;
        }

        private void Promote(Board board)
        {
            board[X, 7].Occupy(GameHandler.RequestPromotion(this, X, Color));
        }
    }
}
