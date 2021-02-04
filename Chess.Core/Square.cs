using System;
using System.Collections.Generic;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class Square : IEquatable<Square>
    {
        public Square(ChessPiece piece)
        {
            OccupiedBy = piece;
        }

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

        public ChessPiece OccupiedBy { get; private set; }

        public bool IsWhiteProtected { get; set; }

        public bool IsBlackProtected { get; set; }

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

        public override string ToString()
        {
            return OccupiedBy?.ToString() ?? "0";
        }

        public override bool Equals(object obj)
        {
            return obj is Square sq && Equals(sq);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OccupiedBy, IsWhiteProtected, IsBlackProtected);
        }

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
