using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class PawnTests
    {
        [TestMethod]
        public void CanWhitePawnMove()
        {
            var board = new Board();
            var pawn = board[1, 1].OccupiedBy;

            Assert.IsTrue(pawn.Move(1, 3, board));
            Assert.IsTrue(pawn.Move(1, 4, board));
            Assert.IsTrue(pawn.Move(1, 5, board));

            Assert.IsFalse(pawn.Move(1, 7, board));
            Assert.IsFalse(pawn.Move(2, 1, board));
            Assert.IsFalse(pawn.Move(5, 1, board));
            Assert.IsFalse(pawn.Move(1, 6, board));

            Assert.IsTrue(pawn.Move(2, 6, board));
            Assert.IsTrue(board[2, 6].OccupiedBy.Color == PieceColor.White);
            Assert.IsNull(board[1, 1].OccupiedBy);

            var pawn1 = new Pawn(2, 1, PieceColor.White);
            Board.Occupy(board[2, 2], pawn1);
            Assert.IsFalse(pawn1.Move(2, 3, board));
        }

        [TestMethod]
        public void CanBlackPawnMove()
        {
            var board = new Board();
            var pawn = board[1, 6].OccupiedBy;

            Assert.IsTrue(pawn.Move(1, 4, board));
            Assert.IsTrue(pawn.Move(1, 3, board));
            Assert.IsTrue(pawn.Move(1, 2, board));

            Assert.IsFalse(pawn.Move(1, 7, board));
            Assert.IsFalse(pawn.Move(2, 6, board));
            Assert.IsFalse(pawn.Move(5, 1, board));

            Assert.IsTrue(pawn.Move(2, 1, board));
            Assert.IsTrue(board[2, 1].OccupiedBy.Color == PieceColor.Black);
            Assert.IsNull(board[1, 6].OccupiedBy);

            var pawn1 = new Pawn(2, 6, PieceColor.White);
            Board.Occupy(board[2, 5], new Pawn(2, 5, PieceColor.Black));
            Assert.IsFalse(pawn1.Move(2, 4, board));
        }
    }
}
