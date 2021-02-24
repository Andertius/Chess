using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void Move(object sender, EventArgs e)
        {
            if (Game.Move(Start.X, Start.Y, End.X, End.Y))
            {
                HandleSounds();
                WhiteConfirmation.Visibility = Visibility.Collapsed;
                BlackConfirmation.Visibility = Visibility.Collapsed;

                if (Game.Turn == PieceColor.White)
                {
                    WhiteClockBorder.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    BlackClockBorder.Background = new SolidColorBrush(Color.FromRgb(44, 39, 35));

                    WhiteTimeTextBlock.Foreground = Brushes.Black;
                    BlackTimeTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(152, 150, 149));
                }
                else
                {
                    WhiteClockBorder.Background = new SolidColorBrush(Color.FromRgb(152, 150, 149));
                    BlackClockBorder.Background = new SolidColorBrush(Color.FromRgb(38, 33, 27));

                    WhiteTimeTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(57, 53, 49));
                    BlackTimeTextBlock.Foreground = Brushes.White;
                }

                LastMove = ((Start.X, Start.Y), (End.X, End.Y));
                Start = (-1, -1);
                JustPickedUp = false;

                AddMove();

                if (whiteTimer is null && Settings.TimedGames.IsChecked == true)
                {
                    StartTimers();
                }
            }
        }

        private void StartTimers()
        {
            whiteTime = TimeSpan.FromSeconds(Double.Parse(Settings.WhiteTimeTextBox.Text) - 0.1);
            blackTime = TimeSpan.FromSeconds(Double.Parse(Settings.BlackTimeTextBox.Text));

            blackTimer = new DispatcherTimer(new TimeSpan(ticks: 10000), DispatcherPriority.Normal, delegate
            {
                string minutes = $"{blackTime.Hours * 60 + blackTime.Minutes}";
                string seconds;

                if (blackTime.Seconds / 10 == 0)
                {
                    seconds = $"0{blackTime.Seconds}";
                }
                else
                {
                    seconds = $"{blackTime.Seconds}";
                }

                BlackTimeTextBlock.Text = minutes + ":" + seconds;

                if (blackTime <= TimeSpan.Zero)
                {
                    Game.Winner = PieceColor.White;
                    EndGame();
                }
                else if (Game.Winner is not null)
                {
                    EndGame();
                }

                if (Game.Turn == PieceColor.Black)
                {
                    blackTime = blackTime.Add(TimeSpan.FromMilliseconds(-15));
                }
            }, Application.Current.Dispatcher);

            whiteTimer = new DispatcherTimer(new TimeSpan(ticks: 10000), DispatcherPriority.Normal, delegate
            {
                string minutes = $"{whiteTime.Hours * 60 + whiteTime.Minutes}";
                string seconds;

                if (whiteTime.Seconds / 10 == 0)
                {
                    seconds = $"0{whiteTime.Seconds}";
                }
                else
                {
                    seconds = $"{whiteTime.Seconds}";
                }

                WhiteTimeTextBlock.Text = minutes + ":" + seconds;

                if (whiteTime <= TimeSpan.Zero)
                {
                    Game.Winner = PieceColor.Black;
                    EndGame();
                }
                else if (Game.Winner is not null)
                {
                    EndGame();
                }

                if (Game.Turn == PieceColor.White)
                {
                    whiteTime = whiteTime.Add(TimeSpan.FromMilliseconds(-15));
                }
            }, Application.Current.Dispatcher);

            whiteTimer.Start();
            blackTimer.Start();
        }
    }
}
