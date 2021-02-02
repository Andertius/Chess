using System.Diagnostics;

using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class KingTests
    {
        [TestMethod]
        public void KingIsMoved()
        {
            var king = new King(3, 3, PieceColor.White);
            var board = new Board();
            Board.Occupy(board[3, 3], king);

            Debug.WriteLine(board);

            Assert.IsTrue(king.Move(3, 4, board, out _, false));
            Assert.IsTrue(king.Move(3, 3, board, out _, false));
            Assert.IsTrue(king.Move(4, 4, board, out _, false));
            Assert.IsTrue(king.Move(3, 3, board, out _, false));
            Assert.IsTrue(king.Move(4, 3, board, out _, false));
            Assert.IsTrue(king.Move(3, 3, board, out _, false));
            Assert.IsTrue(king.Move(4, 2, board, out _, false));
            Assert.IsTrue(king.Move(3, 3, board, out _, false));

            Assert.IsTrue(king.Move(3, 2, board, out _, false));
            Assert.IsFalse(king.Move(3, 1, board, out _, false));
            Assert.IsFalse(king.Move(7, 1, board, out _, false));
            Assert.IsFalse(king.Move(3, 5, board, out _, false));
            Assert.IsFalse(king.Move(5, 2, board, out _, false));
        }

        [TestMethod]
        public void KingCannotGoOnProtectedSquares()
        {
            var king = new King(3, 4, PieceColor.White);
            var board = new Board();
            Board.Occupy(board[3, 4], king);

            Assert.IsFalse(king.Move(3, 5, board, out _, false));
            Assert.IsFalse(king.Move(2, 5, board, out _, false));
            Assert.IsFalse(king.Move(4, 5, board, out _, false));

            board[1, 7].Move(2, 5, board, out _, false);

            Assert.IsFalse(king.Move(3, 4, board, out _, false));
            Assert.IsFalse(king.Move(4, 4, board, out _, false));
        }

        [TestMethod]
        public void KingIsUnderCheck()
        {
            var board = new Board();

            board[1, 7].Move(2, 5, board, out _, false);
            board[2, 5].Move(3, 3, board, out _, false);
            board[3, 3].Move(5, 2, board, out _, false);

            Assert.IsTrue(board.CheckForWhiteCheck());

            board[6, 1].Move(5, 2, board, out _, false);

            Debug.WriteLine(board);

            board[1, 0].Move(2, 2, board, out _, false);
            board[2, 2].Move(3, 4, board, out _, false);
            board[3, 4].Move(5, 5, board, out _, false);

            Assert.IsTrue(board.CheckForBlackCheck());
        }

        [TestMethod]
        public void CannotMovePieceUnlessPreventsCheck()
        {
            var board = new Board();

            board[4, 1].Move(4, 3, board, out _, false);
            board[4, 6].Move(4, 4, board, out _, false);
            board[5, 1].Move(5, 3, board, out _, false);
            board[3, 7].Move(7, 3, board, out _, false);

            Assert.IsTrue(board.CheckForWhiteCheck());
            Assert.IsFalse(board[0, 1].Move(0, 2, board, out _, false));
            Assert.IsTrue(board[4, 0].Move(4, 1, board, out _, false));

            board[7, 3].Move(3, 7, board, out _, false);
            board[4, 1].Move(4, 0, board, out _, false);
            board[5, 6].Move(5, 5, board, out _, false);
            board[3, 0].Move(7, 4, board, out _, false);

            Assert.IsTrue(board.CheckForBlackCheck());
            Assert.IsFalse(board[0, 6].Move(0, 4, board, out _, false));
            Assert.IsTrue(board[4, 7].Move(4, 6, board, out _, false));
        }
    }
}
