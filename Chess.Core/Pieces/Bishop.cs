namespace Chess.Core.Pieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(int x, int y, bool isWhite)
            : base(x, y, isWhite, 3) { }

        public override bool IsValidMove(int newX, int newY)
        {
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
