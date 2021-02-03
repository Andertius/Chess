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
        }

        public ObservableCollection<ChessPiece> WhiteCaptured { get; }

        public ObservableCollection<ChessPiece> BlackCaptured { get; }

        public Board Board { get; }

        public int TotalWhiteValue => WhiteCaptured.Select(x => x.Value).Sum();

        public int TotalBlackValue => BlackCaptured.Select(x => x.Value).Sum();

        public PieceColor Turn { get; private set; }

        public bool IsWhiteUnderCheck { get; private set; }

        public bool IsBlackUnderCheck { get; private set; }

        public bool IsOnStalemate { get; private set; }

        public bool WhiteWon { get; private set; }

        public bool BlackWon { get; private set; }

        public void Move(object sender, MoveEventArgs e)
        {
            if (Board[e.X, e.Y].OccupiedBy?.Color == Turn)
            {
                if (Board[e.X, e.Y].Move(e.NewX, e.NewY, Board, out var capturedPiece, false))
                {
                    if (!(capturedPiece is null))
                    {
                        WhiteCaptured.Add(capturedPiece);
                    }

                    e.Moved = true;
                    Turn = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;

                    if (Board.CheckForCheck(Turn) && !Board.CheckIfHasValidMoves(Turn))
                    {
                        if (Turn == PieceColor.Black)
                        {
                            WhiteWon = true;
                        }
                        else
                        {
                            BlackWon = true;
                        }
                    }
                    else if (!Board.CheckIfHasValidMoves(Turn))
                    {
                        IsOnStalemate = true;
                    }
                }
            }
        }
    }
}
