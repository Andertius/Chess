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

        public bool Move(int x, int y, Board board)
        {
            return OccupiedBy?.Move(x, y, board) ?? false;
        }
    }
}
