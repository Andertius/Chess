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

            Assert.IsTrue(game.Move(1, 0, 2, 2));
            Assert.IsFalse(game.Move(0, 0, 1, 0));

            Assert.IsTrue(game.Move(7, 6, 7, 5));
            Assert.IsFalse(game.Move(7, 7, 7, 6));
        }

        [TestMethod]
        public void PieceIsPinned()
        {
            var game = new GameHandler();

            game.Move(4, 1, 4, 2);
            game.Move(4, 6, 4, 4);

            game.Move(7, 1, 7, 2);
            game.Move(3, 7, 7, 3);

            game.Move(5, 1, 5, 2);
            Assert.IsTrue(game.Board[5, 1].OccupiedBy is Pawn);
            game.Move(3, 0, 7, 4);

            game.Move(5, 6, 5, 5);
            Assert.IsTrue(game.Board[5, 6].OccupiedBy is Pawn);
        }

        [TestMethod]
        public void CannotMovePieceUnlessPreventsCheck()
        {
            var game = new GameHandler();

            game.Move(4, 1, 4, 3);
            game.Move(4, 6, 4, 4);
            game.Move(5, 1, 5, 3);
            game.Move(3, 7, 7, 3);

            Assert.IsTrue(game.Board.CheckForCheck(PieceColor.White));
            Assert.IsFalse(game.Move(0, 1, 0, 2));
            Assert.IsTrue(game.Move(4, 0, 4, 1));

            game.Move(7, 3, 3, 7);
            game.Move(4, 1, 4, 0);
            game.Move(5, 6, 5, 5);
            game.Move(3, 0, 7, 4);

            Assert.IsTrue(game.Board.CheckForCheck(PieceColor.Black));
            Assert.IsFalse(game.Move(0, 6, 0, 4));
            Assert.IsTrue(game.Move(4, 7, 4, 6));
        }

        [TestMethod]
        public void CannotMoveBlankSquares()
        {
            var game = new GameHandler();
            Assert.IsFalse(game.Move(4, 4, 4, 1));
        }

        [TestMethod]
        public void WhiteWon()
        {
            var game = new GameHandler();

            Assert.IsTrue(game.Move(4, 1, 4, 3));
            Assert.IsTrue(game.Move(4, 6, 4, 4));
            Assert.IsTrue(game.Move(5, 0, 2, 3));
            Assert.IsTrue(game.Move(1, 7, 2, 5));
            Assert.IsTrue(game.Move(3, 0, 5, 2));
            Assert.IsTrue(game.Move(3, 6, 3, 5));
            Assert.IsTrue(game.Move(5, 2, 5, 6));

            Debug.WriteLine(game.Board);

            Assert.IsTrue(game.Winner == PieceColor.White);
            Assert.IsNull(game.Draw);
        }

        [TestMethod]
        public void BlackWon()
        {
            var game = new GameHandler();

            Assert.IsTrue(game.Move(5, 1, 5, 2));
            Assert.IsTrue(game.Move(4, 6, 4, 4));
            Assert.IsTrue(game.Move(6, 1, 6, 3));
            Assert.IsTrue(game.Move(3, 7, 7, 3));

            Debug.WriteLine(game.Board);

            Assert.IsTrue(game.Winner == PieceColor.Black);
            Assert.IsNull(game.Draw);
        }

        [TestMethod]
        public void GotStalemate()
        {
            var game = new GameHandler();

            game.Move(4, 1, 4, 2);
            game.Move(0, 6, 0, 4);
            game.Move(3, 0, 7, 4);
            game.Move(0, 7, 0, 5);
            game.Move(7, 4, 0, 4);
            game.Move(7, 6, 7, 4);
            game.Move(7, 1, 7, 3);
            game.Move(0, 5, 7, 5);
            game.Move(0, 4, 2, 6);
            game.Move(5, 6, 5, 5);
            game.Move(2, 6, 3, 6);
            game.Move(4, 7, 5, 6);
            game.Move(3, 6, 1, 6);
            game.Move(3, 7, 3, 2);
            game.Move(1, 6, 1, 7);
            game.Move(3, 2, 7, 6);
            game.Move(1, 7, 2, 7);
            game.Move(5, 6, 6, 5);
            game.Move(2, 7, 4, 5);

            Assert.IsTrue(game.Draw == DrawBy.Stalemate);
            Assert.IsNull(game.Winner);
        }

        [TestMethod]
        public void MovesAreGettingLogged()
        {
            var game = new GameHandler();

            game.Move(4, 1, 4, 3);
            game.Move(4, 6, 4, 4);
            game.Move(1, 0, 2, 2);
            game.Move(6, 7, 5, 5);
            game.Move(3, 0, 5, 2);
            game.Move(5, 7, 2, 4);
            game.Move(3, 1, 3, 2);
            game.Move(4, 7, 6, 7);
            game.Move(2, 0, 6, 4);
            game.Move(7, 6, 7, 5);
            game.Move(4, 0, 2, 0);
            game.Move(7, 5, 6, 4);
            game.Move(5, 2, 5, 5);
            game.Move(0, 6, 0, 4);
            game.Move(5, 5, 6, 6);
            game.Move(6, 7, 6, 6);
            game.Move(6, 0, 5, 2);
            game.Move(2, 4, 5, 1);
            game.Move(2, 2, 1, 4);
            game.Move(6, 4, 6, 3);
            game.Move(1, 4, 0, 6);
            game.Move(6, 3, 6, 2);
            game.Move(0, 6, 2, 5);
            game.Move(6, 2, 7, 1);
            // game.Move(5, 2, 4, 4);
            // game.Move(2, 5, 4, 4);
            game.Move(7, 0, 7, 1);
            game.Move(1, 7, 0, 5);
            game.Move(7, 1, 7, 5);
            game.Move(0, 5, 2, 4);
            game.Move(7, 5, 4, 5);
            game.Move(2, 4, 4, 3);
            game.Move(3, 0, 4, 0);
            game.Move(4, 3, 6, 2);
            game.Move(4, 3, 6, 2);
            game.Move(4, 0, 4, 4);

            Assert.AreEqual("e4",    game.BoardStates["White"][0].ToString());
            Assert.AreEqual("e5",    game.BoardStates["Black"][0].ToString());
            Assert.AreEqual("Nc3",   game.BoardStates["White"][1].ToString()); 
            Assert.AreEqual("Nf6",   game.BoardStates["Black"][1].ToString());
            Assert.AreEqual("Qf3",   game.BoardStates["White"][2].ToString());
            Assert.AreEqual("Bc5",   game.BoardStates["Black"][2].ToString());
            Assert.AreEqual("d3",    game.BoardStates["White"][3].ToString());
            Assert.AreEqual("O-O",   game.BoardStates["Black"][3].ToString());
            Assert.AreEqual("Bg5",   game.BoardStates["White"][4].ToString());
            Assert.AreEqual("h6",    game.BoardStates["Black"][4].ToString());
            Assert.AreEqual("O-O-O", game.BoardStates["White"][5].ToString());
            Assert.AreEqual("hxg5",  game.BoardStates["Black"][5].ToString());
            Assert.AreEqual("Qxf6",  game.BoardStates["White"][6].ToString());
            Assert.AreEqual("a5",    game.BoardStates["Black"][6].ToString());
            Assert.AreEqual("Qxg7+", game.BoardStates["White"][7].ToString());
            Assert.AreEqual("Kxg7",  game.BoardStates["Black"][7].ToString());
            Assert.AreEqual("Nf3",   game.BoardStates["White"][8].ToString());
            Assert.AreEqual("Bxf2",  game.BoardStates["Black"][8].ToString());
            Assert.AreEqual("Nb5",   game.BoardStates["White"][9].ToString());
            Assert.AreEqual("g4",    game.BoardStates["Black"][9].ToString());
            Assert.AreEqual("Na7",   game.BoardStates["White"][10].ToString());
            Assert.AreEqual("g3",    game.BoardStates["Black"][10].ToString());
            Assert.AreEqual("Nc6",   game.BoardStates["White"][11].ToString());
            Assert.AreEqual("gxh2",  game.BoardStates["Black"][11].ToString());
            // Assert.AreEqual("Nfxe5", game.MoveHistory[12].WhiteMove.ToString());
            // Assert.AreEqual("Ncxe5", game.MoveHistory[12].WhiteMove.ToString());
            Assert.AreEqual("R1xe5", game.BoardStates["White"][16].ToString());
        }

        [TestMethod]
        public void ThreefoldRepetitionStalemate()
        {
            var game = new GameHandler();

            for (int i = 0; i < 5; i++)
            {
                game.Move(1, 0, 2, 2);
                game.Move(1, 7, 2, 5);
                game.Move(2, 2, 1, 0);
                game.Move(2, 5, 1, 7);
            }

            Assert.IsTrue(game.Draw == DrawBy.Repetition);
        }
    }
}
