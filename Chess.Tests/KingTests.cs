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

            Assert.IsTrue(board.CheckForCheck(PieceColor.White));

            board[6, 1].Move(5, 2, board, out _, false);

            board[1, 0].Move(2, 2, board, out _, false);
            board[2, 2].Move(3, 4, board, out _, false);
            board[3, 4].Move(5, 5, board, out _, false);

            Assert.IsTrue(board.CheckForCheck(PieceColor.Black));
        }

        [TestMethod]
        public void CannotMovePieceUnlessPreventsCheck()
        {
            var board = new Board();

            board[4, 1].Move(4, 3, board, out _, false);
            board[4, 6].Move(4, 4, board, out _, false);
            board[5, 1].Move(5, 3, board, out _, false);
            board[3, 7].Move(7, 3, board, out _, false);

            Assert.IsTrue(board.CheckForCheck(PieceColor.White));
            Assert.IsFalse(board[0, 1].Move(0, 2, board, out _, false));
            Assert.IsTrue(board[4, 0].Move(4, 1, board, out _, false));

            board[7, 3].Move(3, 7, board, out _, false);
            board[4, 1].Move(4, 0, board, out _, false);
            board[5, 6].Move(5, 5, board, out _, false);
            board[3, 0].Move(7, 4, board, out _, false);

            Assert.IsTrue(board.CheckForCheck(PieceColor.Black));
            Assert.IsFalse(board[0, 6].Move(0, 4, board, out _, false));
            Assert.IsTrue(board[4, 7].Move(4, 6, board, out _, false));
        }

        [TestMethod]
        public void WhiteKingCastlesRight_Castles()
        {
            var board = new Board();

            Assert.IsTrue(board[4, 1].Move(4, 3, board, out _, false));
            Assert.IsTrue(board[6, 7].Move(5, 5, board, out _, false));
            Assert.IsTrue(board[5, 0].Move(2, 3, board, out _, false));
            Assert.IsTrue(board[5, 5].Move(4, 3, board, out _, false));
            Assert.IsTrue(board[6, 0].Move(5, 2, board, out _, false));
            Assert.IsTrue(board[4, 3].Move(5, 1, board, out _, false));

            Assert.IsTrue(board[4, 0].Move(6, 0, board, out _, false));
            Assert.IsNull(board[4, 0].OccupiedBy);

            var king = (King)board[6, 0].OccupiedBy;
            var rook = (Rook)board[5, 0].OccupiedBy;

            Assert.IsTrue(king.IsMoved);
            Assert.IsTrue(rook.IsMoved);
        }

        [TestMethod]
        public void WhiteKingCastlesRight_Fails()
        {
            var board = new Board();

            Assert.IsTrue(board[4, 1].Move(4, 3, board, out _, false));
            Assert.IsTrue(board[6, 7].Move(5, 5, board, out _, false));
            Assert.IsTrue(board[5, 0].Move(2, 3, board, out _, false));
            Assert.IsTrue(board[5, 5].Move(4, 3, board, out _, false));
            Assert.IsTrue(board[6, 0].Move(5, 2, board, out _, false));
            Assert.IsTrue(board[4, 3].Move(6, 2, board, out _, false));

            Assert.IsFalse(board[4, 0].Move(6, 0, board, out _, false));

            Assert.IsTrue(board[6, 2].Move(4, 1, board, out _, false));
            Assert.IsFalse(board[4, 0].Move(6, 0, board, out _, false));

            Assert.IsTrue(board[4, 1].Move(3, 3, board, out _, false));
            Assert.IsTrue(board[3, 3].Move(2, 1, board, out _, false));
            Assert.IsFalse(board[4, 0].Move(6, 0, board, out _, false));
        }

        [TestMethod]
        public void WhiteKingCastlesLeft_Castles()
        {
            var board = new Board();

            board[6, 6].Move(6, 5, board, out _, false);
            board[5, 7].Move(6, 6, board, out _, false);
            board[6, 6].Move(1, 1, board, out _, false);
            board[1, 7].Move(2, 5, board, out _, false);
            board[2, 5].Move(0, 4, board, out _, false);
            board[0, 4].Move(2, 3, board, out _, false);
            board[2, 3].Move(0, 2, board, out _, false);

            board[1, 0].Move(2, 2, board, out _, false);
            board[3, 1].Move(3, 3, board, out _, false);
            board[2, 0].Move(6, 4, board, out _, false);
            board[3, 0].Move(3, 1, board, out _, false);
            board[1, 1].Move(2, 2, board, out _, false);

            Assert.IsTrue(board[4, 0].Move(2, 0, board, out _, false));
        }

        [TestMethod]
        public void WhiteKingCastlesLeft_Fails()
        {
            var board = new Board();

            board[6, 6].Move(6, 5, board, out _, false);
            board[5, 7].Move(6, 6, board, out _, false);
            board[6, 6].Move(1, 1, board, out _, false);
            board[1, 7].Move(2, 5, board, out _, false);
            board[2, 5].Move(0, 4, board, out _, false);
            board[0, 4].Move(2, 3, board, out _, false);
            board[2, 3].Move(0, 2, board, out _, false);

            board[1, 0].Move(2, 2, board, out _, false);
            board[3, 1].Move(3, 3, board, out _, false);
            board[2, 0].Move(6, 4, board, out _, false);
            board[3, 0].Move(3, 1, board, out _, false);

            Assert.IsFalse(board[4, 0].Move(2, 0, board, out _, false));

            board[0, 2].Move(1, 4, board, out _, false);
            board[2, 2].Move(3, 4, board, out _, false);
            board[1, 1].Move(3, 3, board, out _, false);
            board[1, 4].Move(2, 2, board, out _, false);

            Assert.IsFalse(board[4, 0].Move(2, 0, board, out _, false));
        }

        [TestMethod]
        public void BlackKingCastlesRight_Castles()
        {
            var board = new Board();

            board[4, 1].Move(4, 3, board, out _, false);
            board[4, 6].Move(4, 5, board, out _, false);
            board[3, 0].Move(7, 4, board, out _, false);
            board[6, 7].Move(5, 5, board, out _, false);
            board[7, 4].Move(7, 6, board, out _, false);
            board[5, 7].Move(2, 4, board, out _, false);
            board[7, 6].Move(7, 5, board, out _, false);
            Assert.IsTrue(board[4, 7].Move(6, 7, board, out _, false));
        }

        [TestMethod]
        public void BlackKingCastlesRight_Fails()
        {
            var board = new Board();

            board[4, 1].Move(4, 3, board, out _, false);
            board[4, 6].Move(4, 5, board, out _, false);
            board[3, 0].Move(7, 4, board, out _, false);
            board[6, 7].Move(5, 5, board, out _, false);
            board[7, 4].Move(7, 6, board, out _, false);
            board[5, 7].Move(2, 4, board, out _, false);
            Assert.IsFalse(board[4, 7].Move(6, 7, board, out _, false));

            board[7, 6].Move(6, 6, board, out _, false);
            Assert.IsFalse(board[4, 7].Move(6, 7, board, out _, false));
        }

        [TestMethod]
        public void BlackKingCastlesLeft_Castles()
        {
            var board = new Board();

            board[2, 1].Move(2, 3, board, out _, false);
            board[1, 7].Move(2, 5, board, out _, false);
            board[3, 0].Move(0, 3, board, out _, false);
            board[3, 6].Move(3, 5, board, out _, false);
            board[0, 3].Move(0, 6, board, out _, false);
            board[2, 7].Move(4, 5, board, out _, false);
            board[0, 6].Move(0, 5, board, out _, false);
            board[3, 7].Move(3, 6, board, out _, false);

            Assert.IsTrue(board[4, 7].Move(2, 7, board, out _, false));
        }

        [TestMethod]
        public void BlackKingCastlesLeft_Fails()
        {
            var board = new Board();

            board[2, 1].Move(2, 3, board, out _, false);
            board[1, 7].Move(2, 5, board, out _, false);
            board[3, 0].Move(0, 3, board, out _, false);
            board[3, 6].Move(3, 5, board, out _, false);
            board[0, 3].Move(0, 6, board, out _, false);
            board[2, 7].Move(4, 5, board, out _, false);
            board[0, 6].Move(1, 5, board, out _, false);
            board[3, 7].Move(3, 6, board, out _, false);
            board[1, 5].Move(2, 6, board, out _, false);

            Assert.IsFalse(board[4, 7].Move(2, 7, board, out _, false));

            Debug.WriteLine(board);
        }
    }
}
