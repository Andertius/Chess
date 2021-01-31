namespace Chess.Core.Pieces
{
    public class Queen : ChessPiece
    {
        public Queen(int x, int y, bool isWhite)
            : base(x, y, isWhite, 9) { }

        public override bool IsValidMove(int newX, int newY)
        {
            if ((newY > 0 || newY < 9) && newX == X)
            {
                return true;
            }
            else if ((newX > 0 || newX < 9) && newY == Y)
            {
                return true;
            }

            for (int i = 0; X + i < 9 && Y + i < 9; i++)
            {
                if (newX == X + i && newY == Y + i)
                {
                    return true;
                }
            }

            for (int i = 0; X + i < 9 && Y - i > 0; i++)
            {
                if (newX == X + i && newY == Y - i)
                {
                    return true;
                }
            }

            for (int i = 0; X - i > 0 && Y - i > 0; i++)
            {
                if (newX == X - i && newY == Y - i)
                {
                    return true;
                }
            }

            for (int i = 0; X - i > 0 && Y + i < 9; i++)
            {
                if (newX == X - i && newY == Y + i)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
