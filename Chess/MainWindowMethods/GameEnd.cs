using System;
using System.Collections.Generic;
using System.Windows;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void WhiteResign_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted)
            {
                if (WhiteConfirmation.Visibility == Visibility.Visible)
                {
                    WhiteConfirmation.Visibility = Visibility.Collapsed;
                }
                else if (Settings.ResignConfirmation.IsChecked == false)
                {
                    Game.Winner = PieceColor.Black;
                    EndGame();
                }
                else
                {
                    WhiteConfirmation.Visibility = Visibility.Visible;
                }
            }
        }

        private void WhiteYes_Click(object sender, RoutedEventArgs e)
        {
            Game.Winner = PieceColor.Black;
            WhiteConfirmation.Visibility = Visibility.Collapsed;

            EndGame();
        }

        private void WhiteNo_Click(object sender, RoutedEventArgs e)
        {
            WhiteConfirmation.Visibility = Visibility.Collapsed;
        }

        private void BlackResign_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted)
            {
                if (BlackConfirmation.Visibility == Visibility.Visible)
                {
                    BlackConfirmation.Visibility = Visibility.Collapsed;
                }
                else if (Settings.ResignConfirmation.IsChecked == false)
                {
                    Game.Winner = PieceColor.White;
                    EndGame();
                }
                else
                {
                    BlackConfirmation.Visibility = Visibility.Visible;
                }
            }
        }

        private void BlackYes_Click(object sender, RoutedEventArgs e)
        {
            Game.Winner = PieceColor.White;
            BlackConfirmation.Visibility = Visibility.Collapsed;

            EndGame();
        }

        private void BlackNo_Click(object sender, RoutedEventArgs e)
        {
            BlackConfirmation.Visibility = Visibility.Collapsed;
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted)
            {
                Offer.Visibility = Visibility.Visible;
            }
        }

        private void DrawRejected(object sender, EventArgs e)
        {
            Offer.Visibility = Visibility.Collapsed;
        }

        private void DrawAccepted(object sender, EventArgs e)
        {
            Game.Draw = DrawBy.MutualAgreement;
            Offer.Visibility = Visibility.Collapsed;

            EndGame();
        }

        private void GameEnd_NewGamed(object sender, EventArgs e)
        {
            GameStarted = false;
            GameFinished = false;
            GameEnd.Visibility = Visibility.Collapsed;
            Game = new GameHandler();

            MoveHistoryGrid.Children.Clear();
            MoveHistoryGrid.ColumnDefinitions.Clear();
            MoveHistoryGrid.RowDefinitions.Clear();

            LastMove = ((-1, -1), (-1, -1));
            MoveButtons.Clear();
            LastSound.Clear();
            LastSound.Add("White", new List<string>());
            LastSound.Add("Black", new List<string>());

            whiteTimer = null;
            blackTimer = null;

            WhiteTimeTextBlock.Text = "";
            BlackTimeTextBlock.Text = "";

            RenderBoardOnly(Game.Board);
        }
    }
}
