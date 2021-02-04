using System;
using System.Collections.Generic;

using Chess.Core.Pieces;

namespace Chess.Core
{
    /// <summary>
    /// Represents a single square on the <see cref="Board"/>.
    /// </summary>
    public class Square : IEquatable<Square>
    {
        /// <summary>
        /// Initializes an instance of a <see cref="Square"/> with the <paramref name="piece"/> inside.
        /// </summary>
        /// <param name="piece">The piece inside of the square.</param>
        public Square(ChessPiece piece)
        {
            OccupiedBy = piece;
        }

        /// <summary>
        /// Initializes a completely new <see cref="Square"/> with a copied <see cref="ChessPiece"/>.
        /// </summary>
        /// <param name="sq">The square to copy the <see cref="ChessPiece"/> from.</param>
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

        /// <summary>
        /// Gets the <see cref="ChessPiece"/> that is occupying the <see cref="Square"/>.
        /// </summary>
        public ChessPiece OccupiedBy { get; private set; }

        /// <summary>
        /// Gets the value that indicates whether the <see cref="Square"/> is protected by a white <see cref="ChessPiece"/>.
        /// </summary>
        public bool IsWhiteProtected { get; set; }

        /// <summary>
        /// Gets the value that indicates whether the <see cref="Square"/> is protected by a black <see cref="ChessPiece"/>.
        /// </summary>
        public bool IsBlackProtected { get; set; }

        /// <summary>
        /// Occupies the <see cref="Square"/> by a new <see cref="ChessPiece"/>.
        /// </summary>
        /// <remarks>
        /// To free up the <see cref="Square"/>, <paramref name="piece"/> has to be <see langword="null"/>.
        /// </remarks>
        /// <param name="piece">The piece to occupy the <see cref="Square"/> with.</param>
        public void Occupy(ChessPiece piece)
        {
            OccupiedBy = piece;
        }

        /// <summary>
        /// Returns all squares that are protected by this square.
        /// </summary>
        /// <param name="board">The board in which search for the squares.</param>
        /// <returns>A <see cref="List{T}"/> with all the squares.</returns>
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

        /// <summary>
        /// Moves the <see cref="ChessPiece"/> that occupies this <see cref="Square"/> into another if it is a valid move,
        /// freeing up this <see cref="Square"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="board"></param>
        /// <param name="capturedPiece"></param>
        /// <param name="isMock"></param>
        /// <returns></returns>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return OccupiedBy?.ToString() ?? "0";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Square sq && Equals(sq);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(OccupiedBy, IsWhiteProtected, IsBlackProtected);
        }

        /// <inheritdoc/>
        public bool Equals(Square sq)
        {
            bool chessPpiecesAreSame;

            switch (OccupiedBy?.Piece)
            {
                case Piece.Bishop:
                    var leftBishop = OccupiedBy as Bishop;
                    var rightBishop = sq.OccupiedBy as Bishop;
                    chessPpiecesAreSame = leftBishop.Equals(rightBishop);
                    break;

                case Piece.King:
                    var leftKing = OccupiedBy as King;
                    var rightKing = sq.OccupiedBy as King;
                    chessPpiecesAreSame = leftKing.Equals(rightKing);
                    break;

                case Piece.Knight:
                    var leftKnight = OccupiedBy as Knight;
                    var rightKnight = sq.OccupiedBy as Knight;
                    chessPpiecesAreSame = leftKnight.Equals(rightKnight);
                    break;

                case Piece.Pawn:
                    var leftPawn = OccupiedBy as Pawn;
                    var rightPawn = sq.OccupiedBy as Pawn;
                    chessPpiecesAreSame = leftPawn.Equals(rightPawn);
                    break;

                case Piece.Queen:
                    var leftQueen = OccupiedBy as Queen;
                    var rightQueen = sq.OccupiedBy as Queen;
                    chessPpiecesAreSame = leftQueen.Equals(rightQueen);
                    break;

                case Piece.Rook:
                    var leftRook = OccupiedBy as Rook;
                    var rightRook = sq.OccupiedBy as Rook;
                    chessPpiecesAreSame = leftRook.Equals(rightRook);
                    break;

                default:
                    chessPpiecesAreSame = OccupiedBy is null && sq.OccupiedBy is null;
                    break;
            }

            return chessPpiecesAreSame &&
                IsWhiteProtected == sq.IsWhiteProtected &&
                IsBlackProtected == sq.IsBlackProtected;
        }
    }
}
