using System.Diagnostics;

using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void SquaresAreProtectedByPawns()
        {
            var board = new Board();

            Assert.IsTrue(board[0, 2].IsWhiteProtected);
            Assert.IsTrue(board[2, 2].IsWhiteProtected);

            board[1, 1].Move(1, 3, board, out _, false);
            Assert.IsTrue(board[0, 4].IsWhiteProtected);
            Assert.IsTrue(board[2, 4].IsWhiteProtected);

            board[1, 3].Move(1, 4, board, out _, false);
            Assert.IsTrue(board[0, 5].IsWhiteProtected);
            Assert.IsTrue(board[2, 5].IsWhiteProtected);

            board[1, 4].Move(1, 5, board, out _, false);
            Assert.IsTrue(board[0, 6].IsWhiteProtected);
            Assert.IsTrue(board[2, 6].IsWhiteProtected);

            Assert.IsTrue(board[5, 5].IsBlackProtected);
            Assert.IsTrue(board[7, 5].IsBlackProtected);

            board[6, 6].Move(6, 4, board, out _, false);
            Assert.IsTrue(board[5, 3].IsBlackProtected);
            Assert.IsTrue(board[7, 3].IsBlackProtected);

            board[6, 4].Move(6, 3, board, out _, false);
            Assert.IsTrue(board[5, 2].IsBlackProtected);
            Assert.IsTrue(board[7, 2].IsBlackProtected);

            board[6, 3].Move(6, 2, board, out _, false);
            Assert.IsTrue(board[5, 1].IsBlackProtected);
            Assert.IsTrue(board[7, 1].IsBlackProtected);
        }

        [TestMethod]
        public void SquaresAreProtectedByKnights()
        {
            var board = new Board();

            Assert.IsTrue(board[1, 0].Move(2, 2, board, out _, false));

            Assert.IsTrue(board[1, 0].IsWhiteProtected);
            Assert.IsTrue(board[0, 1].IsWhiteProtected);
            Assert.IsTrue(board[0, 3].IsWhiteProtected);
            Assert.IsTrue(board[1, 4].IsWhiteProtected);
            Assert.IsTrue(board[3, 4].IsWhiteProtected);
            Assert.IsTrue(board[4, 3].IsWhiteProtected);
            Assert.IsTrue(board[4, 1].IsWhiteProtected);
            Assert.IsTrue(board[3, 0].IsWhiteProtected);
        }

        [TestMethod]
        public void SquaresAreProtectedByBishops()
        {
            var board = new Board();

            board[4, 1].Move(4, 2, board, out _, false);
            board[5, 0].Move(2, 3, board, out _, false);
            board[2, 3].Move(3, 4, board, out _, false);

            Assert.IsTrue(board[0, 1].IsWhiteProtected);
            Assert.IsTrue(board[1, 2].IsWhiteProtected);
            Assert.IsTrue(board[2, 3].IsWhiteProtected);
            Assert.IsTrue(board[4, 5].IsWhiteProtected);
            Assert.IsTrue(board[5, 6].IsWhiteProtected);

            Assert.IsTrue(board[1, 6].IsWhiteProtected);
            Assert.IsTrue(board[2, 5].IsWhiteProtected);
            Assert.IsTrue(board[4, 3].IsWhiteProtected);
            Assert.IsTrue(board[5, 2].IsWhiteProtected);
            Assert.IsTrue(board[6, 1].IsWhiteProtected);

            Assert.IsTrue(board[4, 2].Move(4, 3, board, out _, false));
            Assert.IsTrue(board[6, 1].Move(6, 2, board, out _, false));
            Assert.IsTrue(board[6, 0].Move(7, 2, board, out _, false));
            Assert.IsTrue(board[3, 0].Move(4, 1, board, out _, false));
            Assert.IsTrue(board[4, 1].Move(2, 3, board, out _, false));

            Assert.IsTrue(board[4, 3].IsWhiteProtected);
            Assert.IsFalse(board[5, 2].IsWhiteProtected);
        }

        [TestMethod]
        public void SquaresAreProtectedByRooks()
        {
            var board = new Board();

            board[0, 1].Move(0, 3, board, out _, false);
            board[0, 0].Move(0, 2, board, out _, false);
            board[0, 2].Move(3, 2, board, out _, false);
            board[3, 2].Move(3, 3, board, out _, false);

            Assert.IsTrue(board[0, 3].IsWhiteProtected);
            Assert.IsTrue(board[1, 3].IsWhiteProtected);
            Assert.IsTrue(board[2, 3].IsWhiteProtected);
            Assert.IsTrue(board[4, 3].IsWhiteProtected);
            Assert.IsTrue(board[5, 3].IsWhiteProtected);
            Assert.IsTrue(board[6, 3].IsWhiteProtected);
            Assert.IsTrue(board[7, 3].IsWhiteProtected);

            Assert.IsTrue(board[3, 1].IsWhiteProtected);
            Assert.IsTrue(board[3, 2].IsWhiteProtected);
            Assert.IsTrue(board[3, 4].IsWhiteProtected);
            Assert.IsTrue(board[3, 5].IsWhiteProtected);
            Assert.IsTrue(board[3, 6].IsWhiteProtected);

            board[1, 1].Move(1, 3, board, out _, false);

            Assert.IsFalse(board[0, 3].IsWhiteProtected);
        }

        [TestMethod]
        public void SquaresAreProtectedByQueens()
        {
            var board = new Board();

            board[4, 1].Move(4, 2, board, out _, false);
            board[3, 0].Move(7, 4, board, out _, false);
            board[7, 4].Move(3, 4, board, out _, false);

            Assert.IsTrue(board[1, 6].IsWhiteProtected);
            Assert.IsTrue(board[2, 5].IsWhiteProtected);
            Assert.IsTrue(board[2, 4].IsWhiteProtected);
            Assert.IsTrue(board[2, 3].IsWhiteProtected);
            Assert.IsTrue(board[1, 2].IsWhiteProtected);
            Assert.IsTrue(board[0, 2].IsWhiteProtected);
            Assert.IsTrue(board[3, 6].IsWhiteProtected);
            Assert.IsTrue(board[3, 5].IsWhiteProtected);
            Assert.IsTrue(board[3, 3].IsWhiteProtected);
            Assert.IsTrue(board[3, 2].IsWhiteProtected);
            Assert.IsTrue(board[3, 1].IsWhiteProtected);
            Assert.IsTrue(board[4, 5].IsWhiteProtected);
            Assert.IsTrue(board[4, 4].IsWhiteProtected);
            Assert.IsTrue(board[4, 3].IsWhiteProtected);
            Assert.IsTrue(board[5, 6].IsWhiteProtected);
            Assert.IsTrue(board[5, 4].IsWhiteProtected);
            Assert.IsTrue(board[6, 4].IsWhiteProtected);
            Assert.IsTrue(board[7, 4].IsWhiteProtected);
            Assert.IsTrue(board[5, 2].IsWhiteProtected);
            Assert.IsTrue(board[6, 1].IsWhiteProtected);

            board[4, 2].Move(4, 3, board, out _, false);
            board[6, 1].Move(6, 2, board, out _, false);
            board[6, 0].Move(7, 2, board, out _, false);

            Assert.IsFalse(board[5, 2].IsWhiteProtected);
        }

        [TestMethod]
        public void SquaresAreProtectedByKings()
        {
            var board = new Board();
            board[4, 1].Move(4, 2, board, out _, false);
            board[4, 0].Move(4, 1, board, out _, false);

            Assert.IsTrue(board[4, 2].IsWhiteProtected);
            Assert.IsTrue(board[5, 2].IsWhiteProtected);
            Assert.IsTrue(board[5, 1].IsWhiteProtected);
            Assert.IsTrue(board[5, 0].IsWhiteProtected);
            Assert.IsTrue(board[4, 0].IsWhiteProtected);
            Assert.IsTrue(board[3, 0].IsWhiteProtected);
            Assert.IsTrue(board[3, 1].IsWhiteProtected);
            Assert.IsTrue(board[3, 2].IsWhiteProtected);
        }

        [TestMethod]
        public void PieceGetsCaptured()
        {
            var board = new Board();

            board[4, 1].Move(4, 2, board, out var capturedPiece, false);
            Assert.IsNull(capturedPiece);

            board[3, 0].Move(7, 4, board, out capturedPiece, false);
            Assert.IsNull(capturedPiece);

            board[7, 4].Move(7, 6, board, out capturedPiece, false);
            Assert.IsNotNull(capturedPiece);
            Assert.IsTrue(capturedPiece is Pawn);
        }

        [TestMethod]
        public void PieceIsPinned()
        {
            var board = new Board();

            Assert.IsTrue(board[4, 6].Move(4, 4, board, out _, false));
            Assert.IsTrue(board[3, 7].Move(7, 3, board, out _, false));

            Assert.IsFalse(board[5, 1].Move(5, 2, board, out _, false));
            Assert.IsTrue(board[5, 1].OccupiedBy is Pawn);

            Assert.IsTrue(board[4, 1].Move(4, 2, board, out _, false));
            Assert.IsTrue(board[3, 0].Move(7, 4, board, out _, false));

            Assert.IsFalse(board[5, 6].Move(5, 5, board, out _, false));
            Assert.IsTrue(board[5, 6].OccupiedBy is Pawn);
        }

        [TestMethod]
        public void BoardsAreEqual()
        {
            var board1 = new Board();
            var board2 = new Board();

            board1[4, 1].Move(4, 3, board1, out _, false);
            board2[4, 1].Move(4, 3, board2, out _, false);

            Assert.AreEqual(board1, board2);
        }

        [TestMethod]
        public void BoardsAreNotEqual_OnePawnIsEnPassantable()
        {
            var game1 = new GameHandler();
            var game2 = new GameHandler();

            game1.Move(4, 1, 4, 3);
            game1.Move(1, 7, 2, 5);
            game1.Move(1, 0, 2, 2);
            game1.Move(2, 5, 1, 7);
            game1.Move(2, 2, 1, 0);

            game2.Move(4, 1, 4, 3);

            Assert.AreNotEqual(game1.Board, game2.Board);
        }

        [TestMethod]
        public void BoardsAreNotEqual_OneKingIsMoved()
        {
            var board1 = new Board();
            var board2 = new Board();

            board1[4, 1].Move(4, 3, board1, out _, false);
            board1[4, 0].Move(4, 1, board1, out _, false);
            board1[4, 1].Move(4, 0, board1, out _, false);
            board2[4, 1].Move(4, 3, board2, out _, false);

            Assert.AreNotEqual(board1, board2);
        }

        [TestMethod]
        public void BoardsAreNotEqual_OneRookIsMoved()
        {
            var board1 = new Board();
            var board2 = new Board();

            board1[0, 1].Move(0, 3, board1, out _, false);
            board1[0, 0].Move(0, 1, board1, out _, false);
            board1[0, 1].Move(0, 0, board1, out _, false);
            board2[0, 1].Move(0, 3, board2, out _, false);

            Assert.AreNotEqual(board1, board2);
        }
    }
}
