using System.Collections.ObjectModel;
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
            IsWhitesTurn = true;
        }

        public ObservableCollection<ChessPiece> WhiteCaptured { get; }

        public ObservableCollection<ChessPiece> BlackCaptured { get; }

        public Board Board { get; }

        public int TotalWhiteValue => WhiteCaptured.Select(x => x.Value).Sum();

        public int TotalBlackValue => BlackCaptured.Select(x => x.Value).Sum();

        public bool IsWhitesTurn { get; private set; }

        public bool IsWhiteUnderCheck { get; private set; }

        public bool IsBlackUnderCheck { get; private set; }

        public void Move(object sender, MoveEventArgs e)
        {
            if (IsWhitesTurn && Board[e.X, e.Y].OccupiedBy?.Color == PieceColor.White)
            {
                if (Board[e.X, e.Y].Move(e.NewX, e.NewY, Board, out var capturedPiece, false))
                {
                    if (!(capturedPiece is null))
                    {
                        WhiteCaptured.Add(capturedPiece);
                    }

                    IsWhitesTurn = !IsWhitesTurn;
                    e.Moved = true;
                }
            }
            else if (!IsWhitesTurn && Board[e.X, e.Y].OccupiedBy?.Color == PieceColor.Black)
            {
                if (Board[e.X, e.Y].Move(e.NewX, e.NewY, Board, out var capturedPiece, false))
                {
                    if (!(capturedPiece is null))
                    {
                        BlackCaptured.Add(capturedPiece); 
                    }

                    IsWhitesTurn = !IsWhitesTurn;
                    e.Moved = true;
                }
            }
        }
    }
}
