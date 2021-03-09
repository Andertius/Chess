using System;
using System.Collections.Generic;
using System.Windows;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void EndGame()
        {
            string message = String.Empty;

            if (Game.Winner is not null)
            {
                if (!GameFinished)
                {
                    message = $"{Enum.GetName(typeof(PieceColor), Game.Winner)} won!";

                    switch (Game.Win)
                    {
                        case WonBy.Checkmate:
                            GameEnd.Reason.Text = "Checkmate.";
                            break;

                        case WonBy.Resignation:
                            GameEnd.Reason.Text = "Resignation.";
                            break;

                        case WonBy.Timeout:
                            GameEnd.Reason.Text = "Timeout.";
                            break;
                    }
                }
            }

            switch (Game.Draw)
            {
                case DrawBy.Stalemate:
                    message = $"Draw";
                    GameEnd.Reason.Text = "Stalemate.";
                    break;

                case DrawBy.FiftyMoveRule:
                    message = "Draw";
                    GameEnd.Reason.Text = "by y'all being boring.";
                    break;

                case DrawBy.MutualAgreement:
                    message = "Draw";
                    GameEnd.Reason.Text = "Agreement.";
                    break;

                case DrawBy.InsuficientMaterial:
                    message = "Draw";
                    GameEnd.Reason.Text = "Insuficient material.";
                    break;

                case DrawBy.Repetition:
                    message = "Draw";
                    GameEnd.Reason.Text = "Repetition.";
                    break;

                default:
                    break;
            }

            if (message != String.Empty)
            {
                whiteTimer?.Stop();
                blackTimer?.Stop();
                RenderModels(Game.Board);
                GameEnd.Message.Text = message;
                GameEnd.Visibility = Visibility.Visible;
                GameFinished = true;
            }
        }

        private void WhiteResign_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted && !DrawOffered)
            {
                if (WhiteConfirmation.Visibility == Visibility.Visible)
                {
                    WhiteConfirmation.Visibility = Visibility.Collapsed;
                }
                else if (Settings.ResignConfirmation.IsChecked == false)
                {
                    Game.Winner = PieceColor.Black;
                    Game.Win = WonBy.Resignation;
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
            Game.Win = WonBy.Resignation;

            EndGame();
        }

        private void WhiteNo_Click(object sender, RoutedEventArgs e)
        {
            WhiteConfirmation.Visibility = Visibility.Collapsed;
        }

        private void BlackResign_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted && !DrawOffered)
            {
                if (BlackConfirmation.Visibility == Visibility.Visible)
                {
                    BlackConfirmation.Visibility = Visibility.Collapsed;
                }
                else if (Settings.ResignConfirmation.IsChecked == false)
                {
                    Game.Winner = PieceColor.White;
                    Game.Win = WonBy.Resignation;
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
            Game.Win = WonBy.Resignation;

            EndGame();
        }

        private void BlackNo_Click(object sender, RoutedEventArgs e)
        {
            BlackConfirmation.Visibility = Visibility.Collapsed;
        }

        private void DrawFromWhite_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted)
            {
                if (DrawOffered)
                {
                    DrawAccepted(this, e);
                    return;
                }

                DrawOffered = true;
                WhiteOffer.Visibility = Visibility.Visible;
                WhiteConfirmation.Visibility = Visibility.Collapsed;
                BlackConfirmation.Visibility = Visibility.Collapsed;
            }
        }

        private void DrawFromBlack_Click(object sender, RoutedEventArgs e)
        {
            if (GameStarted)
            {
                if (DrawOffered)
                {
                    DrawAccepted(this, e);
                    return;
                }

                DrawOffered = true;
                BlackOffer.Visibility = Visibility.Visible;
                WhiteConfirmation.Visibility = Visibility.Collapsed;
                BlackConfirmation.Visibility = Visibility.Collapsed;
            }
        }

        private void DrawRejected(object sender, EventArgs e)
        {
            DrawOffered = false;
            WhiteOffer.Visibility = Visibility.Collapsed;
            BlackOffer.Visibility = Visibility.Collapsed;
        }

        private void DrawAccepted(object sender, EventArgs e)
        {
            DrawOffered = false;
            Game.Draw = DrawBy.MutualAgreement;
            WhiteOffer.Visibility = Visibility.Collapsed;
            BlackOffer.Visibility = Visibility.Collapsed;

            EndGame();
        }

        private void GameEnd_NewGamed(object sender, EventArgs e)
        {
            GameStarted = false;
            GameFinished = false;
            GameEnd.Visibility = Visibility.Collapsed;
            WhiteOffer.Visibility = Visibility.Collapsed;
            BlackOffer.Visibility = Visibility.Collapsed;
            WhiteConfirmation.Visibility = Visibility.Collapsed;
            BlackConfirmation.Visibility = Visibility.Collapsed;
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

            RenderModels(Game.Board);
        }
    }
}
