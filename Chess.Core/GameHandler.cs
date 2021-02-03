using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class GameHandler
    {
        public GameHandler()
        {
            WhiteCaptured = new ObservableCollection<ChessPiece>();
            BlackCaptured = new ObservableCollection<ChessPiece>();
            Board = new Board();
            Turn = PieceColor.White;
            MoveHistory = new List<(MoveDisplay, MoveDisplay)>();
            Loser = null;
        }

        public ObservableCollection<ChessPiece> WhiteCaptured { get; }

        public ObservableCollection<ChessPiece> BlackCaptured { get; }

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

        public PieceColor? Loser { get; private set; }

        public List<(MoveDisplay, MoveDisplay)> MoveHistory { get; }

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
                var move = new MoveDisplay(Board[e.X, e.Y].OccupiedBy, null, false, false, false,
                    false, false, false, false, (e.X, e.Y), (e.NewX, e.NewY));

                if (Board[e.X, e.Y].Move(e.NewX, e.NewY, Board, out var capturedPiece, false))
                {
                    if (Board[e.NewX, e.NewY].OccupiedBy.JustLongCastled)
                    {
                        move.IsLongCastle = true;
                    }
                    else if (Board[e.NewX, e.NewY].OccupiedBy.JustShortCastled)
                    {
                        move.IsShortCastle = true;
                    }

                    if (Board[e.NewX, e.NewY].OccupiedBy != move.Piece)
                    {
                        move.PawnPromotion = Board[e.NewX, e.NewY].OccupiedBy;

                        if (Turn == PieceColor.White)
                        {
                            BlackCaptured.Add(new Pawn(-1, -1, PieceColor.White));
                        }
                        else
                        {
                            WhiteCaptured.Add(new Pawn(-1, -1, PieceColor.Black));
                        }
                    }

                    if (!(capturedPiece is null) && Turn == PieceColor.White)
                    {
                        WhiteCaptured.Add(capturedPiece);
                        move.IsCapturing = true;

                        var canAlsoCapture = Board.CanAlsoCapture(e.NewX, e.NewY, Turn, Board[e.NewX, e.NewY].OccupiedBy.Piece);
                        move.CouldAnotherPieceCaptureSameFile = canAlsoCapture?.X == e.NewX;
                        move.CouldAnotherPieceCapture = !(canAlsoCapture is null) && !move.CouldAnotherPieceCaptureSameFile;
                    }
                    else if (!(capturedPiece is null) && Turn == PieceColor.Black)
                    {
                        BlackCaptured.Add(capturedPiece);
                        move.IsCapturing = true;

                        var canAlsoCapture = Board.CanAlsoCapture(e.NewX, e.NewY, Turn, Board[e.NewX, e.NewY].OccupiedBy.Piece);
                        move.CouldAnotherPieceCaptureSameFile = canAlsoCapture?.X == e.NewX;
                        move.CouldAnotherPieceCapture = !(canAlsoCapture is null) && !move.CouldAnotherPieceCaptureSameFile;
                    }

                    e.Moved = true;
                    Turn = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;

                    if (Board.CheckForCheck(Turn))
                    {
                        if (!Board.CheckIfHasValidMoves(Turn))
                        {
                            Loser = Turn;
                            move.IsMate = true;
                        }
                        else
                        {
                            move.IsCheck = true;
                        }
                    }
                    else if (!Board.CheckIfHasValidMoves(Turn))
                    {
                        IsOnStalemate = true;
                    }

                    Board.UnEnPassantAllPawns();

                    if (Turn == PieceColor.Black)
                    {
                        MoveHistory.Add((move, null));
                    }
                    else
                    {
                        MoveHistory[^1] = (MoveHistory[^1].Item1, move);
                    }
                }
            }
        }
    }
}
