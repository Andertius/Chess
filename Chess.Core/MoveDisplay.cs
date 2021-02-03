using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class MoveDisplay
    {
        public MoveDisplay (
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
            Piece = piece;
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

        public ChessPiece Piece { get; }

        public ChessPiece PawnPromotion { get; set; }

        public bool IsShortCastle { get; set; }

        public bool IsLongCastle { get; set; }

        public bool IsCheck { get; set; }

        public bool IsMate { get; set; }

        public bool IsCapturing { get; set; }

        public bool CouldAnotherPieceCapture {get; set; }

        public bool CouldAnotherPieceCaptureSameFile {get; set; }

        public (int, int) Start { get; }

        public (int, int) End { get; }

        public override string ToString()
        {
            string result = "";

            if (Piece is Pawn)
            {
                result += IsCapturing ? $"{(char)(Start.Item1 + 97)}x" : "";

                if (!(PawnPromotion is null))
                {
                    result += $"={PawnPromotion}";
                }
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

                result += $"{Piece}";

                if (IsCapturing)
                {
                    result += CouldAnotherPieceCapture ? $"{(char)(Start.Item1 + 97)}x" :
                        CouldAnotherPieceCaptureSameFile ? $"{Start.Item2 + 1}x" : "x";
                }
            }

            result += $"{(char)(End.Item1 + 97)}{End.Item2 + 1}";
            result += IsCheck ? "+" : IsMate ? "#" : "";

            return result;
        }
    }
}
