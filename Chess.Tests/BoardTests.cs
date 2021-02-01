using System.Diagnostics;

using Chess.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void AreSquaresProtectedByPawns()
        {
            var board = new Board();

            Assert.IsTrue(board[0, 2].IsWhiteProtected);
            Assert.IsTrue(board[2, 2].IsWhiteProtected);

            board[1, 1].Move(1, 3, board);
            Assert.IsTrue(board[0, 4].IsWhiteProtected);
            Assert.IsTrue(board[2, 4].IsWhiteProtected);

            board[1, 3].Move(1, 4, board);
            Assert.IsTrue(board[0, 5].IsWhiteProtected);
            Assert.IsTrue(board[2, 5].IsWhiteProtected);

            board[1, 4].Move(1, 5, board);
            Assert.IsTrue(board[0, 6].IsWhiteProtected);
            Assert.IsTrue(board[2, 6].IsWhiteProtected);

            Assert.IsTrue(board[5, 5].IsBlackProtected);
            Assert.IsTrue(board[7, 5].IsBlackProtected);

            board[6, 6].Move(6, 4, board);
            Assert.IsTrue(board[5, 3].IsBlackProtected);
            Assert.IsTrue(board[7, 3].IsBlackProtected);

            board[6, 4].Move(6, 3, board);
            Assert.IsTrue(board[5, 2].IsBlackProtected);
            Assert.IsTrue(board[7, 2].IsBlackProtected);

            board[6, 3].Move(6, 2, board);
            Assert.IsTrue(board[5, 1].IsBlackProtected);
            Assert.IsTrue(board[7, 1].IsBlackProtected);
        }

        [TestMethod]
        public void AreSquaresProtectedByKnights()
        {
            var board = new Board();

            Assert.IsTrue(board[1, 0].Move(2, 2, board));

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
        public void AreSquaresProtectedByBishops()
        {
            var board = new Board();

            board[4, 1].Move(4, 2, board);
            board[5, 0].Move(2, 3, board);
            board[2, 3].Move(3, 4, board);

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

            Assert.IsTrue(board[4, 2].Move(4, 3, board));
            Assert.IsTrue(board[6, 1].Move(6, 2, board));
            Assert.IsTrue(board[6, 0].Move(7, 2, board));
            Assert.IsTrue(board[3, 0].Move(4, 1, board));
            Assert.IsTrue(board[4, 1].Move(2, 3, board));

            Assert.IsTrue(board[4, 3].IsWhiteProtected);
            Assert.IsFalse(board[5, 2].IsWhiteProtected);
        }

        [TestMethod]
        public void AreSquaresProtectedByRooks()
        {
            var board = new Board();

            board[0, 1].Move(0, 3, board);
            board[0, 0].Move(0, 2, board);
            board[0, 2].Move(3, 2, board);
            board[3, 2].Move(3, 3, board);

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

            board[1, 1].Move(1, 3, board);

            Assert.IsFalse(board[0, 3].IsWhiteProtected);
        }

        [TestMethod]
        public void AreSquaresProtectedByQueens()
        {
            var board = new Board();

            board[4, 1].Move(4, 2, board);
            board[3, 0].Move(7, 4, board);
            board[7, 4].Move(3, 4, board);

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

            board[4, 2].Move(4, 3, board);
            board[6, 1].Move(6, 2, board);
            board[6, 0].Move(7, 2, board);

            Assert.IsFalse(board[5, 2].IsWhiteProtected);
        }

        [TestMethod]
        public void AreSquaresProtectedByKings()
        {
            var board = new Board();
            board[4, 1].Move(4, 2, board);
            board[4, 0].Move(4, 1, board);

            Assert.IsTrue(board[4, 2].IsWhiteProtected);
            Assert.IsTrue(board[5, 2].IsWhiteProtected);
            Assert.IsTrue(board[5, 1].IsWhiteProtected);
            Assert.IsTrue(board[5, 0].IsWhiteProtected);
            Assert.IsTrue(board[4, 0].IsWhiteProtected);
            Assert.IsTrue(board[3, 0].IsWhiteProtected);
            Assert.IsTrue(board[3, 1].IsWhiteProtected);
            Assert.IsTrue(board[3, 2].IsWhiteProtected);
        }
    }
}
