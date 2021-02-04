using System;
using System.Collections.Generic;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    /// <summary>
    /// Represents the game itself.
    /// </summary>
    public class GameHandler
    {
        private int fiftyRuleCounter;

        /// <summary>
        /// Initializes the <see cref="GameHandler"/> class.
        /// </summary>
        public GameHandler()
        {
            Captured = new Dictionary<string, List<ChessPiece>>
            {
                { "White", new List<ChessPiece>() },
                { "Black", new List<ChessPiece>() }
            };

            BoardStates = new Dictionary<string, List<BoardState>>()
            {
                { "White", new List<BoardState>() },
                { "Black", new List<BoardState>() }
            };

            Board = new Board();
            Turn = PieceColor.White;
            Winner = null;
        }

        /// <summary>
        /// Gets the <see cref="Dictionary{TKey, TValue}"/> of all the moves taken from the beginning of the game.
        /// </summary>
        public Dictionary<string, List<ChessPiece>> Captured { get; }

        /// <summary>
        /// Gets the <see cref="Core.Board"/> object in which the game is taking place.
        /// </summary>
        public Board Board { get; }

        /// <summary>
        /// Gets total value the white player has on the board.
        /// </summary>
        public int TotalWhiteValue => Board.GameBoard
            .SelectMany(x => x)
            .Where(x => x.OccupiedBy?.Color == PieceColor.White)
            .Select(x => x.OccupiedBy.Value).Sum();

        /// <summary>
        /// Gets total value the black player has on the board.
        /// </summary>
        public int TotalBlackValue => Board.GameBoard
            .SelectMany(x => x)
            .Where(x => x.OccupiedBy?.Color == PieceColor.White)
            .Select(x => x.OccupiedBy.Value).Sum();

        /// <summary>
        /// Gets or sets the color of the current turn.
        /// </summary>
        public PieceColor Turn { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether the white player is under check.
        /// </summary>
        public bool IsWhiteUnderCheck { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether the black player is under check.
        /// </summary>
        public bool IsBlackUnderCheck { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether the game is in stalemate.
        /// </summary>
        public bool IsInStalemate { get; private set; }

        /// <summary>
        /// Gets the winner of the game.
        /// </summary>
        public PieceColor? Winner { get; private set; }

        /// <summary>
        /// Gets all the previous board states.
        /// </summary>
        public Dictionary<string, List<BoardState>> BoardStates { get; }

        /// <summary>
        /// An event that handles the <see cref="Pawn"/> promotion.
        /// </summary>
        public static event EventHandler<PawnPromotionEventArgs> PromotionRequested;

        /// <summary>
        /// Requests the <see cref="ChessPiece"/> into which the <see cref="Pawn"/> should promote.
        /// </summary>
        /// <returns>The <see cref="ChessPiece"/> that the <see cref="Pawn"/> should promote to.</returns>
        /// <exception cref="ArgumentException"/>
        public static ChessPiece RequestPromotion(object sender, int x, PieceColor color)
        {
            var e = new PawnPromotionEventArgs(x, color);
            PromotionRequested?.Invoke(sender, e);

            if (e.Piece.Piece == Piece.King)
            {
                throw new ArgumentException("The pawn cannot be promoted into a king");
            }
            else if (e.Piece.Piece == Piece.Pawn)
            {
                throw new ArgumentException("The pawn cannot be promoted into another pawn");
            }
            else if (e.Piece is null)
            {
                throw new ArgumentException("The chess piece cannot be null");
            }

            return e.Piece;
        }

        /// <summary>
        /// Moves the <see cref="ChessPiece"/> in the given <see cref="Square"/> if it is a valid move,
        /// while switching turns and checking for checks, mates and stalemates.
        /// </summary>
        public void Move(object sender, MoveEventArgs e)
        {
            if (Board[e.X, e.Y].OccupiedBy?.Color == Turn)
            {
                var state = new BoardState(Board[e.X, e.Y].OccupiedBy, null, (e.X, e.Y), (e.NewX, e.NewY));

                if (Winner is null && !IsInStalemate && Board[e.X, e.Y].Move(e.NewX, e.NewY, Board, out var capturedPiece, false))
                {
                    ManageGame(state, e, capturedPiece);
                }
            }
        }

        private void ManageGame(BoardState state, MoveEventArgs e, ChessPiece capturedPiece)
        {
            state.Board = new Board(Board);

            if (Board[e.NewX, e.NewY].OccupiedBy.JustLongCastled)
            {
                state.IsLongCastle = true;
            }
            else if (Board[e.NewX, e.NewY].OccupiedBy.JustShortCastled)
            {
                state.IsShortCastle = true;
            }

            if (Board[e.NewX, e.NewY].OccupiedBy != state.CurrentPiece)
            {
                state.PawnPromotion = Board[e.NewX, e.NewY].OccupiedBy;
                Captured[Enum.GetName(typeof(PieceColor), Turn)].Add(new Pawn(-1, -1, Turn));
            }

            if (!(capturedPiece is null))
            {
                Captured[Enum.GetName(typeof(PieceColor), Turn)].Add(capturedPiece);
                state.IsCapturing = true;

                var canAlsoCapture = Board.CanAlsoCapture(e.NewX, e.NewY, Turn, Board[e.NewX, e.NewY].OccupiedBy.Piece);
                state.CouldAnotherPieceCaptureSameFile = canAlsoCapture?.X == e.NewX;
                state.CouldAnotherPieceCapture = !(canAlsoCapture is null) && !state.CouldAnotherPieceCaptureSameFile;
            }

            if (!state.IsCapturing && state.PawnPromotion is null && state.CurrentPiece.Piece != Piece.Pawn)
            {
                fiftyRuleCounter++;
            }
            else
            {
                fiftyRuleCounter = 0;
            }

            var oppositeColor = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            if (Board.CheckForCheck(oppositeColor))
            {
                if (!Board.CheckIfHasValidMoves(oppositeColor))
                {
                    Winner = Turn;
                    state.IsMate = true;
                }
                else
                {
                    state.IsCheck = true;
                }
            }
            else if (fiftyRuleCounter == 100 || !Board.CheckIfHasValidMoves(oppositeColor) || CheckForThreefoldRepetition(state))
            {
                IsInStalemate = true;
            }

            Board.UnEnPassantAllPawns();
            BoardStates[Enum.GetName(typeof(PieceColor), Turn)].Add(state);

            Turn = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;
            e.Moved = true;
        }

        private bool CheckForThreefoldRepetition(BoardState state)
        {
            BoardState.CheckForRepeatingStates(BoardStates, state, Turn);

            var white = BoardStates["White"].Where(x => x.TimesRepeated == 3).ToList();
            var black = BoardStates["Black"].Where(x => x.TimesRepeated == 3).ToList();

            return white.Count != 0 || black.Count != 0;
        }
    }
}
