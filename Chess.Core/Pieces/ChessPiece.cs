﻿using System;

namespace Chess.Core.Pieces
{
    public abstract class ChessPiece : IEquatable<ChessPiece>
    {
        public ChessPiece(int x, int y, PieceColor color, int value, Piece piece)
        {
            X = x;
            Y = y;
            Color = color;
            Value = value;
            Piece = piece;
        }

        public ChessPiece(ChessPiece piece)
        {
            X = piece.X;
            Y = piece.Y;
            Color = piece.Color;
            Value = piece.Value;
            Piece = piece.Piece;
        }

        protected static int KingValue => 0;

        protected static int RightLowerBoundary => -1;
        protected static int LeftUppeBoundary => 8;

        public bool JustLongCastled { get; set; }
        public bool JustShortCastled { get; set; }

        public int X { get; protected set; }

        public int Y { get; protected set; }

        public PieceColor Color { get; }

        public Piece Piece { get; }

        public int Value { get; }

        public virtual bool Move(int newX, int newY, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if ((newX != X || newY != Y) && IsValidMove(newX, newY, board))
            {
                if (!isMock && CheckForChecksAfterMove(newX, newY, board))
                {
                    capturedPiece = null;
                    return false;
                }

                JustLongCastled = false;
                JustShortCastled = false;

                capturedPiece = board[newX, newY]?.OccupiedBy;

                Board.Occupy(board[newX, newY], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
                Y = newY;

                board.CheckForProtection();
                return true;
            }

            capturedPiece = null;
            return false;
        }

        public virtual bool IsValidMove(int newX, int newY, Board board) => true;

        public virtual bool Protects(int x, int y, Board board) => true;

        public bool HasValidMoves(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (IsValidMove(i, j, board) && !CheckForChecksAfterMove(i, j, board))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckForChecksAfterMove(int newX, int newY, Board board)
        {
            var mockBoard = new Board(board);
            mockBoard[X, Y].Move(newX, newY, mockBoard, out _, true);

            return mockBoard.CheckForCheck(Color);
        }

        public override bool Equals(object obj)
        {
            return obj is ChessPiece chess && Equals(chess);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        public bool Equals(ChessPiece other)
        {
            return !(other is null) && X == other.X && Y == other.Y && Value == other.Value && Color == other.Color && Piece == other.Piece;
        }
    }
}
