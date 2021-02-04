using System;
using System.Collections.Generic;

using Chess.Core.Pieces;

namespace Chess.Core
{
    /// <summary>
    /// Represents the state in which the board is with the given piece positions.
    /// </summary>
    public class BoardState
    {
        /// <summary>
        /// Initializes the <see cref="BoardState"/> class.
        /// </summary>
        public BoardState (
            ChessPiece piece,
            ChessPiece pawnProm,
            (int, int) start,
            (int, int) end)
        {
            CurrentPiece = piece;
            PawnPromotion = pawnProm;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the <see cref="ChessPiece"/> that moved last.
        /// </summary>
        public ChessPiece CurrentPiece { get; }

        /// <summary>
        /// Gets or sets the <see cref="ChessPiece"/> into which the pawn got promoted.
        /// </summary>
        /// <remarks>
        /// <see langword="null"/> if a pawn was not promoted in the last move.
        /// </remarks>
        public ChessPiece PawnPromotion { get; set; }

        /// <summary>
        /// Gets a <see cref="Board"/> object in which all the piece are located.
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// Gets or sets the number of times the current configuration of the <see cref="Board"/> was repeated.
        /// </summary>
        public int TimesRepeated { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a <see cref="King"/> castled
        /// with the kingside <see cref="Rook"/> in the last move.
        /// </summary>
        public bool IsShortCastle { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a <see cref="King"/> castled
        /// with the queenside <see cref="Rook"/> in the last move.
        /// </summary>
        public bool IsLongCastle { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a <see cref="King"/> was checked in the last move.
        /// </summary>
        public bool IsCheck { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a <see cref="King"/> was mated in the last move.
        /// </summary>
        public bool IsMate { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a <see cref="ChessPiece"/> captured another <see cref="ChessPiece"/>.
        /// </summary>
        public bool IsCapturing { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether another same <see cref="ChessPiece"/> could
        /// capture a <see cref="ChessPiece"/> in the current <see cref="Square"/> 
        /// </summary>
        public bool CouldAnotherPieceCapture {get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether another same <see cref="ChessPiece"/> in the same file could
        /// capture a <see cref="ChessPiece"/> in the current <see cref="Square"/> 
        /// </summary>
        public bool CouldAnotherPieceCaptureSameFile {get; set; }

        /// <summary>
        /// Gets the coordinates of the <see cref="Square"/> from which the current <see cref="ChessPiece"/> came.
        /// </summary>
        public (int X, int Y) Start { get; }

        /// <summary>
        /// Gets the coordinates of the <see cref="Square"/> onto which the current <see cref="ChessPiece"/> came.
        /// </summary>
        public (int X, int Y) End { get; }

        /// <summary>
        /// Checks if this <see cref="BoardState"/> was met before and increments their <see cref="TimesRepeated"/> values.
        /// </summary>
        /// <param name="boardStates">All the previous board states.</param>
        /// <param name="state"><see cref="BoardState"/> to which compare all the other states.</param>
        /// <param name="color">The color of the player.</param>
        public static void CheckForRepeatingStates(Dictionary<string, List<BoardState>> boardStates, BoardState state, PieceColor color)
        {
            for (int i = boardStates[Enum.GetName(typeof(PieceColor), color)].Count - 1; i >= 0; i--)
            {
                var boardState = boardStates[Enum.GetName(typeof(PieceColor), color)][i];

                if (boardState.Board == state.Board)
                {
                    boardState.TimesRepeated++;
                    state.TimesRepeated = boardState.TimesRepeated;
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string result = "";

            if (CurrentPiece.Piece == Piece.Pawn)
            {
                result += IsCapturing ? $"{(char)(Start.X + 97)}x" : "";
            }
            else
            {
                if (IsLongCastle)
                {
                    return "O-O-O";
                }
                else if (IsShortCastle)
                {
                    return "O-O";
                }

                result += $"{CurrentPiece}";

                if (IsCapturing)
                {
                    result += CouldAnotherPieceCapture ? $"{(char)(Start.X + 97)}x" :
                        CouldAnotherPieceCaptureSameFile ? $"{Start.Y + 1}x" : "x";
                }
            }

            result += $"{(char)(End.X + 97)}{End.Y + 1}";

            if (!(PawnPromotion is null))
            {
                result += $"={PawnPromotion}";
            }

            result += IsCheck ? "+" : IsMate ? "#" : "";
            return result;
        }
    }
}
