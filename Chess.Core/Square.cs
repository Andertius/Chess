using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool IsProtected { get; private set; }

        public bool IsPinned { get; private set; }

        public void Occupy(ChessPiece piece)
        {
            OccupiedBy = piece;
        }
    }
}
