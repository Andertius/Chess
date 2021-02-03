using System;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class PawnPromotionEventArgs : EventArgs
    {
        public PawnPromotionEventArgs(int x, PieceColor color)
        {
            X = x;
            Color = color;
        }

        public ChessPiece Piece { get; set; }

        public int X { get; }

        public PieceColor Color { get; }
    }
}
