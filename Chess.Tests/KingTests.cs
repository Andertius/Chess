using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class KingTests
    {
        [TestMethod]
        public void CanKingMove()
        {
            var king = new King(3, 3, PieceColor.White);
            var board = new Board();
            Board.Occupy(board[2, 2], king);

            Assert.IsTrue(king.Move(3, 4, board));
            Assert.IsTrue(king.Move(4, 5, board));
            Assert.IsTrue(king.Move(5, 5, board));
            Assert.IsTrue(king.Move(6, 4, board));
            Assert.IsTrue(king.Move(6, 3, board));
            Assert.IsTrue(king.Move(5, 2, board));
            Assert.IsTrue(king.Move(4, 2, board));
            Assert.IsTrue(king.Move(3, 3, board));

            Assert.IsTrue(king.Move(3, 2, board));
            Assert.IsFalse(king.Move(3, 1, board));
            Assert.IsFalse(king.Move(7, 1, board));
            Assert.IsFalse(king.Move(3, 5, board));
            Assert.IsFalse(king.Move(5, 2, board));
        }
    }
}
