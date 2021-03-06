﻿using System.Diagnostics;

using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class RookKnightBishopQueenTests
    {
        [TestMethod]
        public void RookIsMoved()
        {
            var rook = new Rook(2, 2, PieceColor.White);
            var board = new Board();
            Board.Occupy(board[2, 2], rook);

            Assert.IsTrue(rook.Move(7, 2, board, out _, false));
            Assert.IsTrue(rook.Move(7, 5, board, out _, false));
            Assert.IsTrue(rook.Move(7, 3, board, out _, false));
            Assert.IsTrue(rook.Move(3, 3, board, out _, false));

            Assert.IsFalse(rook.Move(4, 4, board, out _, false));
            Assert.IsFalse(rook.Move(2, 5, board, out _, false));
            Assert.IsFalse(rook.Move(8, 3, board, out _, false));
            Assert.IsFalse(rook.Move(3, 1, board, out _, false));
            Assert.IsFalse(rook.Move(3, 7, board, out _, false));

            Assert.IsTrue(rook.Move(3, 6, board, out _, false));

            Assert.IsTrue(board[3, 6].OccupiedBy.Color == PieceColor.White);
            Assert.IsNull(board[2, 2].OccupiedBy);
        }

        [TestMethod]
        public void KnightIsMoved()
        {
            var board = new Board();
            var knight = board[1, 0].OccupiedBy;

            Assert.IsTrue(knight.Move(2, 2, board, out _, false));
            Assert.IsTrue(knight.Move(4, 3, board, out _, false));
            Assert.IsTrue(knight.Move(6, 2, board, out _, false));
            Assert.IsTrue(knight.Move(5, 4, board, out _, false));
            Assert.IsTrue(knight.Move(6, 2, board, out _, false));
            Assert.IsTrue(knight.Move(4, 3, board, out _, false));

            Assert.IsFalse(knight.Move(3, 1, board, out _, false));

            Assert.IsTrue(knight.Move(3, 5, board, out _, false));
            Assert.IsTrue(knight.Move(2, 3, board, out _, false));

            Assert.IsTrue(knight.Move(3, 5, board, out _, false));
            Assert.IsTrue(knight.Move(2, 7, board, out _, false));

            Assert.IsFalse(knight.Move(0, 1, board, out _, false));
            Assert.IsFalse(knight.Move(0, 2, board, out _, false));
            Assert.IsFalse(knight.Move(7, 7, board, out _, false));
            Assert.IsFalse(knight.Move(1, 7, board, out _, false));

            Assert.IsTrue(board[2, 7].OccupiedBy.Color == PieceColor.White);
            Assert.IsNull(board[1, 0].OccupiedBy);
        }

        [TestMethod]
        public void BishopIsMoved()
        {
            var bishop = new Bishop(2, 2, PieceColor.White);
            var board = new Board();
            Board.Occupy(board[2, 2], bishop);

            Assert.IsTrue(bishop.Move(3, 3, board, out _, false));

            Assert.IsFalse(bishop.Move(7, 7, board, out _, false));

            Assert.IsTrue(bishop.Move(1, 5, board, out _, false));
            Assert.IsTrue(bishop.Move(2, 4, board, out _, false));
            Assert.IsTrue(bishop.Move(0, 2, board, out _, false));

            Assert.IsFalse(bishop.Move(1, 1, board, out _, false));
            Assert.IsFalse(bishop.Move(0, 1, board, out _, false));
            Assert.IsFalse(bishop.Move(7, 2, board, out _, false));
            Assert.IsFalse(bishop.Move(0, 5, board, out _, false));

            Assert.IsTrue(board[0, 2].OccupiedBy.Color == PieceColor.White);
            Assert.IsNull(board[2, 2].OccupiedBy);
        }

        [TestMethod]
        public void QueenIsMoved()
        {
            var queen = new Queen(2, 2, PieceColor.White);
            var board = new Board();
            Board.Occupy(board[2, 2], queen);

            Assert.IsTrue(queen.Move(7, 2, board, out _, false));
            Assert.IsTrue(queen.Move(7, 5, board, out _, false));
            Assert.IsTrue(queen.Move(7, 3, board, out _, false));
            Assert.IsTrue(queen.Move(3, 3, board, out _, false));

            Assert.IsTrue(queen.Move(4, 4, board, out _, false));
            Assert.IsTrue(queen.Move(2, 2, board, out _, false));
            Assert.IsTrue(queen.Move(0, 4, board, out _, false));
            Assert.IsTrue(queen.Move(1, 3, board, out _, false));

            Assert.IsFalse(queen.Move(2, 5, board, out _, false));
            Assert.IsFalse(queen.Move(8, 3, board, out _, false));
            Assert.IsFalse(queen.Move(3, 1, board, out _, false));
            Assert.IsFalse(queen.Move(3, 7, board, out _, false));

            Assert.IsTrue(board[1, 3].OccupiedBy.Color == PieceColor.White);
            Assert.IsNull(board[2, 2].OccupiedBy);
        }
    }
}
