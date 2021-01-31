namespace Chess.Core.Pieces
{
    public abstract class ChessPiece
    {
        public ChessPiece(int x, int y, bool isWhite, int value)
        {
            X = x;
            Y = y;
            IsWhite = isWhite;
            Value = value;
        }

        protected static int KingValue => -1;

        public int X { get; protected set; }

        public int Y { get; protected set; }

        public bool IsWhite { get; }

        public int Value { get; }

        public virtual bool Move(int newX, int newY)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY))
            {
                X = newX;
                Y = newY;

                return true;
            }

            return false;
        }

        public virtual bool IsValidMove(int newX, int newY) => true;
    }
}
