using System.Diagnostics;

using Chess.Core;
using Chess.Core.Pieces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void CannotTakeTurnUnlessItIsYours()
        {
            var game = new GameHandler();

            var move1 = new MoveEventArgs(1, 0, 2, 2);
            var move2 = new MoveEventArgs(0, 0, 1, 0);

            game.Move(this, move1);
            game.Move(this, move2);

            Assert.IsTrue(move1.Moved);
            Assert.IsFalse(move2.Moved);

            var move3 = new MoveEventArgs(7, 6, 7, 5);
            var move4 = new MoveEventArgs(7, 7, 7, 6);

            game.Move(this, move3);
            game.Move(this, move4);

            Assert.IsTrue(move3.Moved);
            Assert.IsFalse(move4.Moved);
        }

        [TestMethod]
        public void PieceIsPinned()
        {
            var game = new GameHandler();

            game.Move(this, new MoveEventArgs(4, 1, 4, 2));
            game.Move(this, new MoveEventArgs(4, 6, 4, 4));

            game.Move(this, new MoveEventArgs(7, 1, 7, 2));
            game.Move(this, new MoveEventArgs(3, 7, 7, 3));

            game.Move(this, new MoveEventArgs(5, 1, 5, 2));
            Assert.IsTrue(game.Board[5, 1].OccupiedBy is Pawn);
            game.Move(this, new MoveEventArgs(3, 0, 7, 4));

            game.Move(this, new MoveEventArgs(5, 6, 5, 5));
            Assert.IsTrue(game.Board[5, 6].OccupiedBy is Pawn);
        }

        [TestMethod]
        public void CannotMovePieceUnlessPreventsCheck()
        {
            var game = new GameHandler();

            game.Move(this, new MoveEventArgs(4, 1, 4, 3));
            game.Move(this, new MoveEventArgs(4, 6, 4, 4));
            game.Move(this, new MoveEventArgs(5, 1, 5, 3));
            game.Move(this, new MoveEventArgs(3, 7, 7, 3));

            Assert.IsTrue(game.Board.CheckForCheck(PieceColor.White));

            var whiteBadMove = new MoveEventArgs(0, 1, 0, 2);
            game.Move(this, whiteBadMove);
            Assert.IsFalse(whiteBadMove.Moved);

            var whiteGoodMove = new MoveEventArgs(4, 0, 4, 1);
            game.Move(this, whiteGoodMove);
            Assert.IsTrue(whiteGoodMove.Moved);

            game.Move(this, new MoveEventArgs(7, 3, 3, 7));
            game.Move(this, new MoveEventArgs(4, 1, 4, 0));
            game.Move(this, new MoveEventArgs(5, 6, 5, 5));
            game.Move(this, new MoveEventArgs(3, 0, 7, 4));

            Assert.IsTrue(game.Board.CheckForCheck(PieceColor.Black));

            var blackBadMove = new MoveEventArgs(0, 6, 0, 4);
            game.Move(this, blackBadMove);
            Assert.IsFalse(blackBadMove.Moved);

            var blackGoodMove = new MoveEventArgs(4, 7, 4, 6);
            game.Move(this, blackGoodMove);
            Assert.IsTrue(blackGoodMove.Moved);
        }

        [TestMethod]
        public void CannotMoveBlankSquares()
        {
            var game = new GameHandler();

            var move = new MoveEventArgs(4, 4, 4, 1);
            game.Move(this, move);
            Assert.IsFalse(move.Moved);
        }

        [TestMethod]
        public void WhiteWon()
        {
            var game = new GameHandler();

            var move1 = new MoveEventArgs(4, 1, 4, 3);
            var move2 = new MoveEventArgs(4, 6, 4, 4);
            var move3 = new MoveEventArgs(5, 0, 2, 3);
            var move4 = new MoveEventArgs(1, 7, 2, 5);
            var move5 = new MoveEventArgs(3, 0, 5, 2);
            var move6 = new MoveEventArgs(3, 6, 3, 5);
            var move7 = new MoveEventArgs(5, 2, 5, 6);

            game.Move(this, move1);
            game.Move(this, move2);
            game.Move(this, move3);
            game.Move(this, move4);
            game.Move(this, move5);
            game.Move(this, move6);
            game.Move(this, move7);

            Assert.IsTrue(move1.Moved);
            Assert.IsTrue(move2.Moved);
            Assert.IsTrue(move3.Moved);
            Assert.IsTrue(move4.Moved);
            Assert.IsTrue(move4.Moved);
            Assert.IsTrue(move5.Moved);
            Assert.IsTrue(move6.Moved);
            Assert.IsTrue(move7.Moved);

            Debug.WriteLine(game.Board);

            Assert.IsTrue(game.WhiteWon);
            Assert.IsFalse(game.IsOnStalemate);
            Assert.IsFalse(game.BlackWon);
        }

        [TestMethod]
        public void BlackWon()
        {
            var game = new GameHandler();

            var move1 = new MoveEventArgs(5, 1, 5, 2);
            var move2 = new MoveEventArgs(4, 6, 4, 4);
            var move3 = new MoveEventArgs(6, 1, 6, 3);
            var move4 = new MoveEventArgs(3, 7, 7, 3);

            game.Move(this, move1);
            game.Move(this, move2);
            game.Move(this, move3);
            game.Move(this, move4);

            Assert.IsTrue(move1.Moved);
            Assert.IsTrue(move2.Moved);
            Assert.IsTrue(move3.Moved);
            Assert.IsTrue(move4.Moved);

            Debug.WriteLine(game.Board);

            Assert.IsTrue(game.BlackWon);
            Assert.IsFalse(game.IsOnStalemate);
            Assert.IsFalse(game.WhiteWon);
        }

        [TestMethod]
        public void GotStalemate()
        {
            var game = new GameHandler();

            game.Move(this, new MoveEventArgs(4, 1, 4, 2));
            game.Move(this, new MoveEventArgs(0, 6, 0, 4));
            game.Move(this, new MoveEventArgs(3, 0, 7, 4));
            game.Move(this, new MoveEventArgs(0, 7, 0, 5));
            game.Move(this, new MoveEventArgs(7, 4, 0, 4));
            game.Move(this, new MoveEventArgs(7, 6, 7, 4));
            game.Move(this, new MoveEventArgs(7, 1, 7, 3));
            game.Move(this, new MoveEventArgs(0, 5, 7, 5));
            game.Move(this, new MoveEventArgs(0, 4, 2, 6));
            game.Move(this, new MoveEventArgs(5, 6, 5, 5));
            game.Move(this, new MoveEventArgs(2, 6, 3, 6));
            game.Move(this, new MoveEventArgs(4, 7, 5, 6));
            game.Move(this, new MoveEventArgs(3, 6, 1, 6));
            game.Move(this, new MoveEventArgs(3, 7, 3, 2));
            game.Move(this, new MoveEventArgs(1, 6, 1, 7));
            game.Move(this, new MoveEventArgs(3, 2, 7, 6));
            game.Move(this, new MoveEventArgs(1, 7, 2, 7));
            game.Move(this, new MoveEventArgs(5, 6, 6, 5));
            game.Move(this, new MoveEventArgs(2, 7, 4, 5));

            Assert.IsTrue(game.IsOnStalemate);
            Assert.IsFalse(game.WhiteWon);
            Assert.IsFalse(game.BlackWon);
        }
    }
}
