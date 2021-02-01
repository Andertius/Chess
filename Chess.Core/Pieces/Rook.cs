using System;

namespace Chess.Core.Pieces
{
    public class Rook : ChessPiece, IEquatable<Rook>
    {
        public Rook(int x, int y, PieceColor color)
            : base(x, y, color, 5) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (newX > -1 && newX < 8 && newY > -1 && newY < 8 && Color != board[newX, newY].OccupiedBy?.Color)
            {
                return IsValid(newX, newY, board);
            }

            return false;
        }

        public override bool Protects(int x, int y, Board board)
        {
            if ((x != X || y != Y) && x > -1 && x < 8 && y > -1 && y < 8)
            {
                return IsValid(x, y, board);
            }

            return false;
        }

        public override string ToString()
        {
            return "R";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Color, Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Rook rook && Equals(rook);
        }

        public bool Equals(Rook rook)
        {
            return X == rook.X && Y == rook.Y && Value == rook.Value && Color == rook.Color;
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
