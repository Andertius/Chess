namespace Chess.Core.Pieces
{
    public class Rook : ChessPiece
    {
        public Rook(int x, int y, bool isWhite)
            : base(x, y, isWhite, 5) { }

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

            return false;
        }
    }
}
