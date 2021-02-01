namespace Chess.Core.Pieces
{
    public abstract class ChessPiece
    {
        public ChessPiece(int x, int y, PieceColor color, int value)
        {
            X = x;
            Y = y;
            Color = color;
            Value = value;
        }

        public ChessPiece(ChessPiece piece)
        {
            X = piece.X;
            Y = piece.Y;
            Color = piece.Color;
            Value = piece.Value;
        }

        protected static int KingValue => -1;

        public int X { get; protected set; }

        public int Y { get; protected set; }

        public PieceColor Color { get; }

        public int Value { get; }

        public virtual bool Move(int newX, int newY, Board board)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                board.Occupy(newX, newY, board[X, Y]);
                board.Occupy(X, Y, null);

                X = newX;
                Y = newY;

                return true;
            }

            return false;
        }

        public virtual bool IsValidMove(int newX, int newY, Board board) => true;
    }
}
