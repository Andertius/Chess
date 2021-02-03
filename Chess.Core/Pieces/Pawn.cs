using System;

namespace Chess.Core.Pieces
{
    public class Pawn : ChessPiece, IEquatable<Pawn>
    {
        public Pawn(int x, int y, PieceColor color)
            : base(x, y, color, 1) { }

        public bool IsMoved { get; private set; }

        public bool IsPromoted { get; private set; }

        public bool CanBeEnPassanted { get; private set; }

        public override bool Move(int newX, int newY, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if (!isMock && CheckForChecksAfterMove(newX, newY, board))
            {
                capturedPiece = null;
                return false;
            }

            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                capturedPiece = board[newX, newY]?.OccupiedBy;

                Board.Occupy(board[newX, newY], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
                Y = newY;

                IsMoved = true;

                if (Y == 7)
                {
                    Promote();
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
                      (board[X - 1, Y].OccupiedBy is Pawn leftPawn && leftPawn.CanBeEnPassanted)))
                {
                    CanBeEnPassanted = false;
                    return true;
                }
                else if (newY == Y + 1 && newX == X + 1 &&
                         (!(board[X + 1, Y + 1].OccupiedBy is null) && board[X + 1, Y + 1].OccupiedBy.Color != Color ||
                           (board[X + 1, Y].OccupiedBy is Pawn rightPawn && rightPawn.CanBeEnPassanted)))
                {
                    CanBeEnPassanted = false;
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y + 2 && newX == X && board[X, Y + 1].OccupiedBy is null && board[X, Y + 2].OccupiedBy is null)
                    {
                        CanBeEnPassanted = true;
                        return true;
                    }
                    else if (newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                    {
                        CanBeEnPassanted = false;
                        return true;
                    }
                }
                else if (IsMoved && newY == Y + 1 && newX == X && board[X, Y + 1].OccupiedBy is null)
                {
                    CanBeEnPassanted = false;
                    return true;
                }
            }
            else
            {
                if (newY == Y - 1 && newX == X - 1 &&
                    (!(board[X - 1, Y - 1].OccupiedBy is null) && board[X - 1, Y - 1].OccupiedBy.Color != Color ||
                      (board[X - 1, Y].OccupiedBy is Pawn leftPawn && leftPawn.CanBeEnPassanted)))
                {
                    CanBeEnPassanted = false;
                    return true;
                }
                else if (newY == Y - 1 && newX == X + 1 &&
                         (!(board[X + 1, Y - 1].OccupiedBy is null) && board[X + 1, Y - 1].OccupiedBy.Color != Color ||
                           (board[X + 1, Y].OccupiedBy is Pawn rightPawn && rightPawn.CanBeEnPassanted)))
                {
                    CanBeEnPassanted = false;
                    return true;
                }
                else if (!IsMoved)
                {
                    if (newY == Y - 2 && newX == X && board[X, Y - 1].OccupiedBy is null && board[X, Y - 2].OccupiedBy is null)
                    {
                        CanBeEnPassanted = true;
                        return true;
                    }
                    else if (newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                    {
                        CanBeEnPassanted = false;
                        return true;
                    }
                }
                else if (IsMoved && newY == Y - 1 && newX == X && board[X, Y - 1].OccupiedBy is null)
                {
                    CanBeEnPassanted = false;
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

        private void Promote()
        {
            throw new NotImplementedException();
        }
    }
}
