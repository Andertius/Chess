using System.Windows;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void Promote(object sender, PawnPromotionEventArgs e)
        {
            var pawn = (Pawn)sender;
            IsPromotingPawn = true;
            PromotedPawnX = pawn.X;

            //PawnPromotionBorder.Visibility = Visibility.Visible;

            //if (pawn.Color == PieceColor.White)
            //{
            //    WhiteStackPanel.Visibility = Visibility.Visible;
            //    BlackStackPanel.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    WhiteStackPanel.Visibility = Visibility.Collapsed;
            //    BlackStackPanel.Visibility = Visibility.Visible;
            //}

            //while(!PieceChosen)
            //{ }

            e.Piece = new Queen(pawn.X, Game.Turn == PieceColor.White ? 7 : 0, Game.Turn);

            //PawnPromotionBorder.Visibility = Visibility.Collapsed;
            IsPromotingPawn = false;
            PromotedPiece = null;
            PieceChosen = false;
        }

        private void RookChosen(object sender, RoutedEventArgs e)
        {
            PromotedPiece = WhiteStackPanel.Visibility == Visibility.Visible ?
                new Rook(PromotedPawnX, 7, PieceColor.White) :
                new Rook(PromotedPawnX, 0, PieceColor.Black);
            PieceChosen = true;
        }

        private void BishopChosen(object sender, RoutedEventArgs e)
        {
            PromotedPiece = WhiteStackPanel.Visibility == Visibility.Visible ?
                new Bishop(PromotedPawnX, 7, PieceColor.White) :
                new Bishop(PromotedPawnX, 0, PieceColor.Black);
            PieceChosen = true;
        }

        private void KnightChosen(object sender, RoutedEventArgs e)
        {
            PromotedPiece = WhiteStackPanel.Visibility == Visibility.Visible ?
                new Knight(PromotedPawnX, 7, PieceColor.White) :
                new Knight(PromotedPawnX, 0, PieceColor.Black);
            PieceChosen = true;
        }

        private void QueenChosen(object sender, RoutedEventArgs e)
        {
            PromotedPiece = WhiteStackPanel.Visibility == Visibility.Visible ?
                new Queen(PromotedPawnX, 7, PieceColor.White) :
                new Queen(PromotedPawnX, 0, PieceColor.Black);
            PieceChosen = true;
        }
    }
}
