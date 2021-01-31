using System;

namespace Chess.Core.Pieces
{
    public class Pawn : ChessPiece
    {
        public Pawn(int x, int y, bool isWhite)
            : base(x, y, isWhite, 1) { }

        public bool IsMoved { get; private set; }

        public bool IsPromoted { get; private set; }

        public override bool Move(int newX, int newY)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY))
            {
                X = newX;
                Y = newY;
                IsMoved = true;

                if (Y == 8)
                {
                    Promote();
                }

                return true;
            }

            return false;
        }

        public override bool IsValidMove(int newX, int newY)
        {
            if (IsWhite)
            {
                if (!IsMoved && (newY == Y + 2 || newY == Y + 1) && newX == X)
                {
                    return true;
                }
                else if (IsMoved && newY == Y + 1 && newX == X)
                {
                    return true;
                }
            }
            else
            {
                if (!IsMoved && (newY == Y - 2 || newY == Y - 1) && newX == X)
                {
                    return true;
                }
                else if (IsMoved && newY == Y - 1 && newX == X)
                {
                    return true;
                }
            }

            return false;
        }

        private void Promote()
        {
            throw new NotImplementedException();
        }
    }
}
