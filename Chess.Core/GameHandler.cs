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
        /// Gets the <see cref="Dictionary{TKey, TValue}"/> of all the pieces captured from the beginning of the game.
        /// </summary>
        public Dictionary<string, List<ChessPiece>> Captured { get; }

        /// <summary>
        /// Gets the <see cref="Core.Board"/> object in which the game is taking place.
        /// </summary>
        public Board Board { get; }

        /// <summary>
        /// Gets all the pieces the white player has on the board.
        /// </summary>
        public List<ChessPiece> WhitePieces => Board.GameBoard
            .SelectMany(x => x)
            .Where(x => x.OccupiedBy?.Color == PieceColor.White)
            .Select(x => x.OccupiedBy)
            .ToList();

        /// <summary>
        /// Gets all the pieces the black player has on the board.
        /// </summary>
        public List<ChessPiece> BlackPieces => Board.GameBoard
            .SelectMany(x => x)
            .Where(x => x.OccupiedBy?.Color == PieceColor.Black)
            .Select(x => x.OccupiedBy)
            .ToList();

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
        /// Gets or sets the type of a draw the players got.
        /// </summary>
        public DrawBy? Draw { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how the player won.
        /// </summary>
        public WonBy? Win { get; set; }

        /// <summary>
        /// Gets the winner of the game.
        /// </summary>
        public PieceColor? Winner { get; set; }

        /// <summary>
        /// Gets the <see cref="Dictionary{TKey, TValue}"/> of all the moves taken from the beginning of the game.
        /// </summary>
        public Dictionary<string, List<BoardState>> BoardStates { get; }

        /// <summary>
        /// Gets or sets the value indicating whether the plyare has to promote.
        /// </summary>
        public static bool HasToPromote { get; set; }

        /// <summary>
        /// Gets or sets the piece that was captured after a move.
        /// </summary>
        /// <remarks>
        /// If none were captured, the the value is null.
        /// </remarks>
        public ChessPiece CapturedPiece { get; set; }

        /// <summary>
        /// Moves the <see cref="ChessPiece"/> in the given <see cref="Square"/> if it is a valid move,
        /// while switching turns and checking for checks, mates and stalemates.
        /// </summary>
        /// <returns><see langword="true"/> if the move is valid; otherwise, <see langword="false"/>.</returns>
        public bool Move(int x, int y, int newX, int newY)
        {
            if (Board[x, y].OccupiedBy?.Color == Turn)
            {
                var state = new BoardState(Board[x, y].OccupiedBy, null, (x, y), (newX,newY));

                if (Winner is null && Draw is null && Board[x, y].Move(newX, newY, Board, out var capturedPiece, false))
                {
                    CapturedPiece = capturedPiece;

                    if (!HasToPromote)
                    {
                        ManageGame(state, x, newX, newY, capturedPiece);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the last board state after a move or pawn promotion.
        /// </summary>
        /// <param name="state">The board state to update.</param>
        /// <param name="x">The starting file position.</param>
        /// <param name="newX">The ending file posotion.</param>
        /// <param name="newY">The ending rank position.</param>
        /// <param name="capturedPiece">A piece that was captured during the move.</param>
        public void ManageGame(BoardState state, int x, int newX, int newY, ChessPiece capturedPiece)
        {
            state.Board = new Board(Board);

            if (Board[newX, newY].OccupiedBy.JustLongCastled)
            {
                state.IsLongCastle = true;
            }
            else if (Board[newX, newY].OccupiedBy.JustShortCastled)
            {
                state.IsShortCastle = true;
            }

            if (HasToPromote)
            {
                Captured[Enum.GetName(typeof(PieceColor), ~Turn)].Add(new Pawn(-1, -1, Turn));
                Captured[Enum.GetName(typeof(PieceColor), ~Turn)].Sort();
            }

            if (capturedPiece is not null)
            {
                if (!capturedPiece.PromotedFormPawn)
                {
                    Captured[Enum.GetName(typeof(PieceColor), Turn)].Add(capturedPiece);
                    Captured[Enum.GetName(typeof(PieceColor), Turn)].Sort();
                }

                state.IsCapturing = true;

                var canAlsoCapture = Board.CanAlsoCapture(newX, newY, Turn, Board[newX, newY].OccupiedBy.Piece);
                state.CouldAnotherPieceCaptureSameFile = canAlsoCapture?.X == x;
                state.CouldAnotherPieceCapture = canAlsoCapture is not null && !state.CouldAnotherPieceCaptureSameFile;

                if (capturedPiece.Y != newY)
                {
                    Board[capturedPiece.X, capturedPiece.Y].Occupy(null);
                }
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
                    Win = WonBy.Checkmate;
                }
                else
                {
                    state.IsCheck = true;
                }
            }
            else
            {
                CheckForStalemate(oppositeColor, state);
            }

            Board.UnEnPassantAllPawns();
            BoardStates[Enum.GetName(typeof(PieceColor), Turn)].Add(state);

            Turn = ~Turn;
        }

        private bool CheckForStalemate(PieceColor oppositeColor, BoardState state)
        {
            if (CheckForInsufficientMaterial())
            {
                Draw = DrawBy.InsuficientMaterial;
            }
            else if (fiftyRuleCounter == 100)
            {
                Draw = DrawBy.FiftyMoveRule;
            }
            else if (!Board.CheckIfHasValidMoves(oppositeColor))
            {
                Draw = DrawBy.Stalemate;
            }
            else if (CheckForThreefoldRepetition(state))
            {
                Draw = DrawBy.Repetition;
            }

            return false;
        }

        private bool CheckForInsufficientMaterial()
        {
            return ((WhitePieces.Count == 1 || CheckForOnlyPiece(WhitePieces, Piece.Bishop) || CheckForOnlyPiece(WhitePieces, Piece.Knight)) &&
                (BlackPieces.Count == 1 || CheckForOnlyPiece(BlackPieces, Piece.Bishop) || CheckForOnlyPiece(BlackPieces, Piece.Knight))) ||
                WhitePieces.Count == 1 && CheckForTwoKnights(BlackPieces) || BlackPieces.Count == 1 && CheckForTwoKnights(WhitePieces);
        }

        private static bool CheckForOnlyPiece(IEnumerable<ChessPiece> pieces, Piece piece)
        {
            if (pieces.Count() != 2)
            {
                return false;
            }

            foreach (var item in pieces)
            {
                if (item.Piece == piece)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckForTwoKnights(IEnumerable<ChessPiece> pieces)
        {
            if (pieces.Count() != 3)
            {
                return false;
            }

            int counter = 0;

            foreach (var item in pieces)
            {
                if (item.Piece == Piece.Knight)
                {
                    counter++;
                }
            }

            return counter == 2;
        }

        private bool CheckForThreefoldRepetition(BoardState state)
        {
            BoardState.CheckForRepeatingStates(BoardStates, state, Turn);

            var white = BoardStates["White"].Where(x => x.TimesRepeated == 3);
            var black = BoardStates["Black"].Where(x => x.TimesRepeated == 3);

            return white.Any() || black.Any();
        }
    }
}
