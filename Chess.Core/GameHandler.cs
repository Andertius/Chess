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
        }

        public ObservableCollection<ChessPiece> White { get; }

        public ObservableCollection<ChessPiece> Black { get; }

        public ObservableCollection<ChessPiece> WhiteCaptured { get; }

        public ObservableCollection<ChessPiece> BlackCaptured { get; }

        public Board Board { get; }

        public int TotalWhiteValue => White.Select(x => x.Value).Sum();

        public int TotalBlackValue => Black.Select(x => x.Value).Sum();

        public bool IsWhitesTurn { get; private set; }
    }
}
