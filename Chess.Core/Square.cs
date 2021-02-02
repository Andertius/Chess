using System;
using System.Collections.Generic;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class Square
    {
        public Square(ChessPiece piece)
        {
            OccupiedBy = piece;
        }

        public Square(Square sq)
        {
            if (sq.OccupiedBy is Bishop)
            {
                OccupiedBy = new Bishop(sq.OccupiedBy.X, sq.OccupiedBy.Y, sq.OccupiedBy.Color);
            }
            else if (sq.OccupiedBy is King)
            {
                OccupiedBy = new King(sq.OccupiedBy.X, sq.OccupiedBy.Y, sq.OccupiedBy.Color);
            }
            else if (sq.OccupiedBy is Knight)
            {
                OccupiedBy = new Knight(sq.OccupiedBy.X, sq.OccupiedBy.Y, sq.OccupiedBy.Color);
            }
            else if (sq.OccupiedBy is Pawn)
            {
                OccupiedBy = new Pawn(sq.OccupiedBy.X, sq.OccupiedBy.Y, sq.OccupiedBy.Color);
            }
            else if (sq.OccupiedBy is Queen)
            {
                OccupiedBy = new Queen(sq.OccupiedBy.X, sq.OccupiedBy.Y, sq.OccupiedBy.Color);
            }
            else if (sq.OccupiedBy is Rook)
            {
                OccupiedBy = new Rook(sq.OccupiedBy.X, sq.OccupiedBy.Y, sq.OccupiedBy.Color);
            }
        }

        public ChessPiece OccupiedBy { get; private set; }

        public bool IsWhiteProtected { get; set; }

        public bool IsBlackProtected { get; set; }

        public bool IsPinned { get; set; }

        public void Occupy(ChessPiece piece)
        {
            OccupiedBy = piece;
        }

        public List<Square> FindProtectedSquares(Board board)
        {
            var result = new List<Square>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (OccupiedBy.Protects(i, j, board))
                    {
                        result.Add(board[i, j]);
                    }
                }
            }

            return result;
        }

        public override string ToString()
        {
            return OccupiedBy?.ToString() ?? "0";
            //return IsWhiteProtected ? "1" : "0";
        }

        public bool Move(int x, int y, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if (OccupiedBy is null)
            {
                capturedPiece = null;
                return false;
            }
            else
            {
                return OccupiedBy.Move(x, y, board, out capturedPiece, isMock);
            }
        }
    }
}
