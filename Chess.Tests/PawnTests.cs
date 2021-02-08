using System;
using System.Diagnostics;

using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class PawnTests
    {
        [TestMethod]
        public void WhitePawnIsMoved()
        {
            var board = new Board();
            var pawn = board[1, 1].OccupiedBy;

            Assert.IsTrue(pawn.Move(1, 3, board, out _, false));
            Assert.IsTrue(pawn.Move(1, 4, board, out _, false));
            Assert.IsTrue(pawn.Move(1, 5, board, out _, false));

            Assert.IsFalse(pawn.Move(1, 7, board, out _, false));
            Assert.IsFalse(pawn.Move(2, 1, board, out _, false));
            Assert.IsFalse(pawn.Move(5, 1, board, out _, false));
            Assert.IsFalse(pawn.Move(1, 6, board, out _, false));

            Assert.IsTrue(pawn.Move(2, 6, board, out _, false));
            Assert.IsTrue(board[2, 6].OccupiedBy.Color == PieceColor.White);
            Assert.IsNull(board[1, 1].OccupiedBy);

            var pawn1 = new Pawn(2, 1, PieceColor.White);
            Board.Occupy(board[2, 2], pawn1);
            Assert.IsFalse(pawn1.Move(2, 3, board, out _, false));
        }

        [TestMethod]
        public void BlackPawnIsMoved()
        {
            var board = new Board();
            var pawn = board[1, 6].OccupiedBy;

            Assert.IsTrue(pawn.Move(1, 4, board, out _, false));
            Assert.IsTrue(pawn.Move(1, 3, board, out _, false));
            Assert.IsTrue(pawn.Move(1, 2, board, out _, false));

            Assert.IsFalse(pawn.Move(1, 7, board, out _, false));
            Assert.IsFalse(pawn.Move(2, 6, board, out _, false));
            Assert.IsFalse(pawn.Move(5, 1, board, out _, false));

            Assert.IsTrue(pawn.Move(2, 1, board, out _, false));
            Assert.IsTrue(board[2, 1].OccupiedBy.Color == PieceColor.Black);
            Assert.IsNull(board[1, 6].OccupiedBy);

            var pawn1 = new Pawn(2, 6, PieceColor.White);
            Board.Occupy(board[2, 5], new Pawn(2, 5, PieceColor.Black));
            Assert.IsFalse(pawn1.Move(2, 4, board, out _, false));
        }

        [TestMethod]
        public void WhitePawnEnPassants()
        {
            var board = new Board();
            var pawn = board[1, 1].OccupiedBy as Pawn;

            Assert.IsTrue(pawn.Move(1, 3, board, out _, false));
            Assert.IsTrue(pawn.Move(1, 4, board, out _, false));

            Assert.IsTrue(board[0, 6].Move(0, 4, board, out _, false));
            Assert.IsTrue(pawn.Move(0, 5, board, out _, false));

            Assert.IsFalse(pawn.CanBeEnPassanted);
        }

        [TestMethod]
        public void BlackPawnEnPassants()
        {
            var board = new Board();
            var pawn = board[1, 6].OccupiedBy as Pawn;

            Assert.IsTrue(pawn.Move(1, 4, board, out _, false));
            Assert.IsTrue(pawn.Move(1, 3, board, out _, false));

            Assert.IsTrue(board[0, 1].Move(0, 3, board, out _, false));
            Assert.IsTrue(pawn.Move(0, 2, board, out _, false));

            Assert.IsFalse(pawn.CanBeEnPassanted);
        }

        [TestMethod]
        public void CannotBeEnPassanted()
        {
            var game = new GameHandler();
            var pawn = game.Board[1, 1].OccupiedBy as Pawn;

            game.Move(1, 1, 1, 3);
            game.Move(0, 6, 0, 4);
            game.Move(1, 3, 1, 4);

            Assert.IsFalse(game.Move(1, 4, 0, 5));
            Assert.IsFalse(pawn.CanBeEnPassanted);
        }

        [TestMethod]
        public void PawnPromoted()
        {
            var game = new GameHandler();
            var pawn = game.Board[1, 1].OccupiedBy as Pawn;

            static void Promote(object sender, PawnPromotionEventArgs e)
            {
                var p = (Pawn)sender;
                e.Piece = new Queen(p.X, 7, p.Color);
            }

            GameHandler.PromotionRequested += Promote;

            pawn.Move(1, 3, game.Board, out _, false);
            pawn.Move(1, 4, game.Board, out _, false);
            pawn.Move(1, 5, game.Board, out _, false);
            pawn.Move(0, 6, game.Board, out _, false);
            pawn.Move(1, 7, game.Board, out _, false);

            Assert.IsTrue(game.Board[1, 7].OccupiedBy is Queen);

            Debug.WriteLine(game.Board);
        }
    }
}
