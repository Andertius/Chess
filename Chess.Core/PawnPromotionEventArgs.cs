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
        /// Gets or sets the <see cref="ChessPiece"/> into which the <see cref="Pawn"/> promoted into.
        /// </summary>
        public ChessPiece Piece { get; set; }
    }
}
