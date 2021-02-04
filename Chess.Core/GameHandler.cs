using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class GameHandler
    {
        private int fiftyRuleCounter;

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

        public Dictionary<string, List<ChessPiece>> Captured { get; }

        public Board Board { get; }

        public int TotalWhiteValue => Board.GameBoard
            .SelectMany(x => x)
            .Where(x => x.OccupiedBy?.Color == PieceColor.White)
            .Select(x => x.OccupiedBy.Value).Sum();

        public int TotalBlackValue => Board.GameBoard
            .SelectMany(x => x)
            .Where(x => x.OccupiedBy?.Color == PieceColor.White)
            .Select(x => x.OccupiedBy.Value).Sum();

        public PieceColor Turn { get; private set; }

        public bool IsWhiteUnderCheck { get; private set; }

        public bool IsBlackUnderCheck { get; private set; }

        public bool IsOnStalemate { get; private set; }

        public PieceColor? Winner { get; private set; }

        public Dictionary<string, List<BoardState>> BoardStates { get; }

        public static event EventHandler<PawnPromotionEventArgs> PromotionRequested;

        public static ChessPiece RequestPromotion(object sender, int x, PieceColor color)
        {
            var e = new PawnPromotionEventArgs(x, color);
            PromotionRequested?.Invoke(sender, e);

            return e.Piece;
        }

        public void Move(object sender, MoveEventArgs e)
        {
            if (Board[e.X, e.Y].OccupiedBy?.Color == Turn)
            {
                var state = new BoardState(Board[e.X, e.Y].OccupiedBy, null, false, false, false,
                    false, false, false, false, (e.X, e.Y), (e.NewX, e.NewY));

                if (Winner is null && !IsOnStalemate && Board[e.X, e.Y].Move(e.NewX, e.NewY, Board, out var capturedPiece, false))
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
                        IsOnStalemate = true;
                    }

                    Board.UnEnPassantAllPawns();
                    BoardStates[Enum.GetName(typeof(PieceColor), Turn)].Add(state);

                    Turn = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    e.Moved = true;
                }
            }
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
