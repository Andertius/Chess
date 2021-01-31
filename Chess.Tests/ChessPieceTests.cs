using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class ChessPieceTests
    {
        [TestMethod]
        public void CanWhitePawnMove()
        {
            var whitePawn = new Pawn(1, 2, true);

            Assert.IsTrue(whitePawn.Move(1, 4));
            Assert.IsTrue(whitePawn.Move(1, 5));
            Assert.IsTrue(whitePawn.Move(1, 6));

            Assert.IsFalse(whitePawn.Move(1, 8));
            Assert.IsFalse(whitePawn.Move(3, 2));
        }

        [TestMethod]
        public void CanBlackPawnMove()
        {
            var blackPawn = new Pawn(1, 7, false);

            Assert.IsTrue(blackPawn.Move(1, 5));
            Assert.IsTrue(blackPawn.Move(1, 4));
            Assert.IsTrue(blackPawn.Move(1, 3));

            Assert.IsFalse(blackPawn.Move(1, 8));
            Assert.IsFalse(blackPawn.Move(3, 2));
        }

        [TestMethod]
        public void CanRookMove()
        {
            var rook = new Rook(4, 4, true);

            Assert.IsTrue(rook.Move(1, 4));
            Assert.IsTrue(rook.Move(1, 8));
            Assert.IsTrue(rook.Move(1, 7));
            Assert.IsTrue(rook.Move(6, 7));

            Assert.IsFalse(rook.Move(1, 8));
            Assert.IsFalse(rook.Move(3, 2));
        }

        [TestMethod]
        public void CanKnightMove()
        {
            var knight = new Knight(4, 4, true);

            Assert.IsTrue(knight.Move(5, 6));
            Assert.IsTrue(knight.Move(4, 4));

            Assert.IsTrue(knight.Move(6, 5));
            knight.Move(4, 4);

            Assert.IsTrue(knight.Move(6, 3));
            knight.Move(4, 4);

            Assert.IsTrue(knight.Move(5, 2));
            knight.Move(4, 4);

            Assert.IsTrue(knight.Move(3, 2));
            knight.Move(4, 4);

            Assert.IsTrue(knight.Move(2, 3));
            knight.Move(4, 4);

            Assert.IsTrue(knight.Move(2, 5));
            knight.Move(4, 4);

            Assert.IsTrue(knight.Move(3, 6));
            knight.Move(4, 4);

            Assert.IsFalse(knight.Move(6, 6));
            Assert.IsFalse(knight.Move(1, 3));
            Assert.IsFalse(knight.Move(7, 6));
            Assert.IsFalse(knight.Move(4, 5));
            Assert.IsFalse(knight.Move(8, 8));
            Assert.IsFalse(knight.Move(5, 4));

            knight.Move(2, 3);
            Assert.IsFalse(knight.Move(0, 5));
        }

        [TestMethod]
        public void CanBishopMove()
        {
            var bishop = new Bishop(1, 1, true);

            Assert.IsTrue(bishop.Move(6, 6));
            Assert.IsTrue(bishop.Move(7, 5));
            Assert.IsTrue(bishop.Move(4, 2));
            Assert.IsTrue(bishop.Move(2, 4));
            Assert.IsTrue(bishop.Move(3, 3));

            Assert.IsFalse(bishop.Move(3, 4));
            Assert.IsFalse(bishop.Move(1, 4));
            Assert.IsFalse(bishop.Move(7, 1));
        }

        [TestMethod]
        public void CanQueenMove()
        {
            var queen = new Queen(1, 5, true);

            Assert.IsTrue(queen.Move(8, 5));
            Assert.IsTrue(queen.Move(8, 1));
            Assert.IsTrue(queen.Move(5, 1));
            Assert.IsTrue(queen.Move(2, 4));
            Assert.IsTrue(queen.Move(2, 5));
            Assert.IsTrue(queen.Move(5, 8));

            Assert.IsFalse(queen.Move(8, 7));
            Assert.IsFalse(queen.Move(1, 7));
            Assert.IsFalse(queen.Move(3, 3));
            Assert.IsFalse(queen.Move(2, 6));
        }

        [TestMethod]
        public void CanKingMove()
        {
            var king = new King(2, 5, true);

            Assert.IsTrue(king.Move(2, 6));
            Assert.IsTrue(king.Move(3, 7));
            Assert.IsTrue(king.Move(4, 7));
            Assert.IsTrue(king.Move(5, 6));
            Assert.IsTrue(king.Move(4, 6));
            Assert.IsTrue(king.Move(3, 5));
            Assert.IsTrue(king.Move(2, 5));
            Assert.IsTrue(king.Move(1, 6));

            Assert.IsFalse(king.Move(3, 6));
            Assert.IsFalse(king.Move(1, 8));
            Assert.IsFalse(king.Move(4, 4));
            Assert.IsFalse(king.Move(2, 4));
        }
    }
}
