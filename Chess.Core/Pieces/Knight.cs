namespace Chess.Core.Pieces
{
    public class Knight : ChessPiece
    {
        public Knight(int x, int y, bool isWhite)
            : base(x, y, isWhite, 3) { }

        public override bool IsValidMove(int newX, int newY)
        {
            if (newX < 9 && newY < 9 && newX > 0 && newY > 0)
            {
                if (newX == X + 1 && newY == Y + 2)
                {
                    return true;
                }
                else if (newX == X + 2 && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X + 2 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y - 2)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y - 2)
                {
                    return true;
                }
                else if (newX == X - 2 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X - 2 && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y + 2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
