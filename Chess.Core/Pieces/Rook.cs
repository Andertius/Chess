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
                if (newX == X)
                {
                    if (newY > Y)
                    {
                        for (int i = Y + 1; i < newY; i++)
                        {
                            if (!(board[X, i].OccupiedBy is null))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = Y - 1; i > newY; i--)
                        {
                            if (!(board[X, i].OccupiedBy is null))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                else if (newY == Y)
                {
                    if (newX > X)
                    {
                        for (int i = X + 1; i < newX; i++)
                        {
                            if (!(board[i, Y].OccupiedBy is null))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = X - 1; i > newX; i--)
                        {
                            if (!(board[i, Y].OccupiedBy is null))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
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
    }
}
