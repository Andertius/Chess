using System;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class Square
    {
        public Square(PieceColor? occupied)
        {
            OccupiedBy = occupied;
        }

        public PieceColor? OccupiedBy { get; private set; }

        public void WhiteOccupied()
        {
            if (OccupiedBy == PieceColor.White)
            {
                throw new InvalidOperationException("A piece with the same color is on the square");
            }
            else
            {
                OccupiedBy = PieceColor.White;
            }
        }

        public void BlackOccupied()
        {
            if (OccupiedBy == PieceColor.Black)
            {
                throw new InvalidOperationException("A piece with the same color is on the square");
            }
            else
            {
                OccupiedBy = PieceColor.Black;
            }
        }

        public void BlankOccupied()
        {
            OccupiedBy = null;
        }

        public override string ToString()
        {
            return OccupiedBy is null ? "Blank" : OccupiedBy == PieceColor.Black ? "Black" : "White";
        }
    }
}
