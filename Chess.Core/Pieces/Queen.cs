using System;

namespace Chess.Core.Pieces
{
    public class Queen : ChessPiece, IEquatable<Queen>
    {
        public Queen(int x, int y, PieceColor color)
            : base(x, y, color, 9) { }

        public override bool IsValidMove(int newX, int newY, Board board)
        {
            if (newX > -1 && newX < 8 && newY > -1 && newY < 8 && Color != board[newX, newY].OccupiedBy)
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
            return "Q";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        public override bool Equals(object obj)
        {
            return obj is Queen queen && Equals(queen);
        }

        public bool Equals(Queen queen)
        {
            return X == queen.X && Y == queen.Y && Value == queen.Value && Color == queen.Color;
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
            for (int i = X - 1, j = Y - 1; i < newX; i--, j--)
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
            for (int i = X - 1, j = Y + 1; i < newX; i--, j++)
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
