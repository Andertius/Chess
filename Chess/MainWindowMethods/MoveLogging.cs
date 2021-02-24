using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void AddMove()
        {
            string color = Game.Turn == PieceColor.Black ? "White" : "Black";

            if (Game.Turn == PieceColor.Black)
            {
                var rect = new Rectangle()
                {
                    Height = 30,
                    Width = 310,
                };

                if (Game.BoardStates[color].Count % 2 == 1)
                {
                    rect.Fill = new SolidColorBrush(Color.FromRgb(35, 33, 31));
                }
                else
                {
                    rect.Fill = new SolidColorBrush(Color.FromRgb(43, 41, 38));
                }

                var row = new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) };
                MoveHistoryGrid.RowDefinitions.Add(row);

                MoveHistoryGrid.Children.Add(rect);
                Grid.SetRow(rect, Game.BoardStates[color].Count - 1);

                var moveNumTextBlock = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Text = $"{Game.BoardStates[color].Count}.",
                    Margin = new Thickness(5, 2, 0, 0),
                    Foreground = Brushes.LightGray,
                };

                MoveHistoryGrid.Children.Add(moveNumTextBlock);
                Grid.SetRow(moveNumTextBlock, Game.BoardStates[color].Count - 1);
            }

            var moveText = new Button()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(30, 0, 0, 0),
                Tag = $"{color} {Game.BoardStates[color].Count - 1}",
            };

            moveText.Margin = Game.Turn == PieceColor.Black ? new Thickness(30, 0, 0, 0) : new Thickness(125, 0, 0, 0);

            moveText.Content = new TextBlock()
            {
                Text = $"{Game.BoardStates[color][^1]}",
                Foreground = new SolidColorBrush(Color.FromRgb(195, 194, 193)),
                FontWeight = FontWeights.Black,
                FontSize = 14,
                Padding = new Thickness(5, 0, 5, 0),
            };

            moveText.Click += MoveButton_Click;
            MoveHistoryGrid.Children.Add(moveText);
            Grid.SetRow(moveText, Game.BoardStates[color].Count - 1);
            MoveButtons.Add(moveText);

            SelectedMove = (Game.Turn == PieceColor.White ? PieceColor.Black : PieceColor.White, Game.BoardStates[color].Count - 1);
            UpdateSelectedMove();
            MoveHistoryScrollViewer.ScrollToEnd();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string info = (string)button.Tag;

            (PieceColor Color, int MoveIndex) selectedMove = (info.Substring(0, 5) == "White" ? PieceColor.White : PieceColor.Black, Int32.Parse(info[6..]));

            if (selectedMove != SelectedMove)
            {
                SelectedMove = selectedMove;
                LastMove = (Game.BoardStates[info.Substring(0, 5)][SelectedMove.MoveIndex].Start, Game.BoardStates[info.Substring(0, 5)][SelectedMove.MoveIndex].End);

                if ((Game.BoardStates["White"].Count == Game.BoardStates["Black"].Count && selectedMove.Color == PieceColor.Black &&
                    selectedMove.MoveIndex == Game.BoardStates["Black"].Count - 1) ||
                    (Game.BoardStates["White"].Count != Game.BoardStates["Black"].Count && selectedMove.Color == PieceColor.White &&
                    selectedMove.MoveIndex == Game.BoardStates["White"].Count - 1))
                {
                    IsOnLastMove = true;
                }
                else
                {
                    IsOnLastMove = false;
                }

                UpdateSelectedMove();
                UpdateBoard();
            }
        }

        private void UpdateSelectedMove()
        {
            foreach (var button in MoveButtons)
            {
                if ((string)button.Tag == $"{Enum.GetName(typeof(PieceColor), SelectedMove.Color)} {SelectedMove.MoveIndex}")
                {
                    button.Background = new SolidColorBrush(Color.FromRgb(82, 81, 78));
                }
                else
                {
                    button.Background = Brushes.Transparent;
                }
            }
        }

        private void UpdateBoard()
        {
            var board = Game.BoardStates[$"{Enum.GetName(typeof(PieceColor), SelectedMove.Color)}"][SelectedMove.MoveIndex].Board;
            RenderBoardOnly(board);

            if (Settings.PlaySounds.IsChecked == true)
            {
                PlayAudio(LastSound[$"{Enum.GetName(typeof(PieceColor), SelectedMove.Color)}"][SelectedMove.MoveIndex]);
            }
        }
    }
}
