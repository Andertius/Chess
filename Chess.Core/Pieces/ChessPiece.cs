using System;
using System.Diagnostics;

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

        protected static int KingValue => 0;

        protected static int RightLowerBoundary => -1;
        protected static int LeftUppeBoundary => 8;

        public int X { get; protected set; }

        public int Y { get; protected set; }

        public PieceColor Color { get; }

        public int Value { get; }

        public virtual bool Move(int newX, int newY, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                if (!isMock && CheckForChecksAfterMove(newX, newY, board))
                {
                    capturedPiece = null;
                    return false;
                }

                capturedPiece = board[newX, newY]?.OccupiedBy;

                Board.Occupy(board[newX, newY], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
                Y = newY;

                board.CheckForProtection();
                return true;
            }

            capturedPiece = null;
            return false;
        }

        public virtual bool IsValidMove(int newX, int newY, Board board) => true;

        public virtual bool Protects(int x, int y, Board board) => true;

        public bool HasValidMoves(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (IsValidMove(i, j, board) && !CheckForChecksAfterMove(i, j, board))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckForChecksAfterMove(int newX, int newY, Board board)
        {
            var mockBoard = new Board(board);
            mockBoard[X, Y].Move(newX, newY, mockBoard, out _, true);

            return mockBoard.CheckForCheck(Color);
        }
    }
}
