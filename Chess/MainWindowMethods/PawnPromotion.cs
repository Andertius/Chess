using System;
using System.Windows;
using System.Windows.Controls;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void HandlePromotion()
        {
            if (Settings.AutoQueen.IsChecked == true)
            {
                var queen = WhiteStackPanel.Visibility == Visibility.Visible ?
                    new Queen(End.X, 7, PieceColor.White) :
                    new Queen(End.X, 0, PieceColor.Black);

                queen.PromotedFormPawn = true;
                FinishMove(queen);
                return;
            }

            IsPromotingPawn = true;

            PawnPromotionBorder.Visibility = Visibility.Visible;

            if (Game.Turn == PieceColor.White)
            {
                WhiteStackPanel.Visibility = Visibility.Visible;
                BlackStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                WhiteStackPanel.Visibility = Visibility.Collapsed;
                BlackStackPanel.Visibility = Visibility.Visible;
            }
        }

        private void PieceChosen(object sender, RoutedEventArgs e)
        {
            string tag = (string)((Button)sender).Tag;
            ChessPiece promotedPiece = tag switch
            {
                nameof(Queen) => WhiteStackPanel.Visibility == Visibility.Visible ?
                    new Queen(End.X, 7, PieceColor.White) :
                    new Queen(End.X, 0, PieceColor.Black),

                nameof(Rook) => WhiteStackPanel.Visibility == Visibility.Visible ?
                    new Rook(End.X, 7, PieceColor.White) :
                    new Rook(End.X, 0, PieceColor.Black),

                nameof(Knight) => WhiteStackPanel.Visibility == Visibility.Visible ?
                    new Knight(End.X, 7, PieceColor.White) :
                    new Knight(End.X, 0, PieceColor.Black),

                nameof(Bishop) => WhiteStackPanel.Visibility == Visibility.Visible ?
                    new Bishop(End.X, 7, PieceColor.White) :
                    new Bishop(End.X, 0, PieceColor.Black),

                _ => throw new ArgumentException("Invald piece."),
            };

            promotedPiece.PromotedFormPawn = true;
            FinishMove(promotedPiece);
        }

        private void FinishMove(ChessPiece piece)
        {
            WhiteStackPanel.Visibility = Visibility.Collapsed;
            BlackStackPanel.Visibility = Visibility.Collapsed;
            PawnPromotionBorder.Visibility = Visibility.Collapsed;

            Game.Board[End.X, End.Y].Occupy(piece);

            var state = new BoardState(new Pawn(Start.X, Start.Y, piece.Color), piece, (Start.X, Start.Y), (End.X, End.Y));
            Game.ManageGame(state, Start.X, End.X, End.Y, Game.CapturedPiece);

            HandleAfterMoveLogic();
            RenderModels(Game.Board);

            GameHandler.HasToPromote = false;
            IsPromotingPawn = false;
        }
    }
}
