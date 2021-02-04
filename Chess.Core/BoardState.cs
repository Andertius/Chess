using System;
using System.Collections.Generic;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class BoardState
    {
        public BoardState (
            ChessPiece piece,
            ChessPiece pawnProm,
            bool isShortCastle,
            bool isLongCastle,
            bool isCheck,
            bool isMate,
            bool isCapturing,
            bool couldAnotherVert,
            bool couldAnotherHor,
            (int, int) start,
            (int, int) end)
        {
            CurrentPiece = piece;
            PawnPromotion = pawnProm;
            IsShortCastle = isShortCastle;
            IsLongCastle = isLongCastle;
            IsCheck = isCheck;
            IsMate = isMate;
            IsCapturing = isCapturing;
            CouldAnotherPieceCapture = couldAnotherVert;
            CouldAnotherPieceCaptureSameFile = couldAnotherHor;
            Start = start;
            End = end;
        }

        public ChessPiece CurrentPiece { get; }

        public ChessPiece PawnPromotion { get; set; }

        public Board Board { get; set; }

        public int TimesRepeated { get; set; }

        public bool IsShortCastle { get; set; }

        public bool IsLongCastle { get; set; }

        public bool IsCheck { get; set; }

        public bool IsMate { get; set; }

        public bool IsCapturing { get; set; }

        public bool CouldAnotherPieceCapture {get; set; }

        public bool CouldAnotherPieceCaptureSameFile {get; set; }

        public (int X, int Y) Start { get; }

        public (int X, int Y) End { get; }

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
