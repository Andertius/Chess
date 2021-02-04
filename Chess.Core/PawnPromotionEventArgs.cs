using System;

using Chess.Core.Pieces;

namespace Chess.Core
{
    /// <summary>
    /// Represents a class that contains data for when a <see cref="Pawn"/> gets promoted.
    /// </summary>
    public class PawnPromotionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes an instance of a <see cref="PawnPromotionEventArgs"/> class.
        /// </summary>
        /// <param name="x">The file in which the <see cref="Pawn"/> got promoted.</param>
        /// <param name="color">The color of the <see cref="Pawn"/>.</param>
        public PawnPromotionEventArgs(int x, PieceColor color)
        {
            X = x;
            Color = color;
        }

        /// <summary>
        /// Gets or sets the <see cref="ChessPiece"/> into which the <see cref="Pawn"/> promoted into.
        /// </summary>
        public ChessPiece Piece { get; set; }

        /// <summary>
        /// Gets the file in which the <see cref="Pawn"/> got promoted.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the color of the <see cref="Pawn"/>.
        /// </summary>
        public PieceColor Color { get; }
    }
}
