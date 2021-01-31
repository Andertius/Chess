namespace Chess.Core.Pieces
{
    public class King : ChessPiece
    {
        public King(int x, int y, bool isWhite)
            : base(x, y, isWhite, KingValue) { }

        public override bool IsValidMove(int newX, int newY)
        {
            if (newX < 9 && newY < 9 && newX > 0 && newY > 0)
            {
                if (newX == X && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y + 1)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y)
                {
                    return true;
                }
                else if (newX == X + 1 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y - 1)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y)
                {
                    return true;
                }
                else if (newX == X - 1 && newY == Y + 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
