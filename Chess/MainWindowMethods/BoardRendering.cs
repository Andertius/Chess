using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private void RenderBoardAfterMove()
        {
            RenderBoardOnly(Game.Board);

            if (Settings.ShowLegalMoves.IsChecked == true &&
                Start.X != -1 &&
                Game.Board[Start.X, Start.Y].OccupiedBy?.Color == Game.Turn &&
                (Game.Draw is null || Game.Winner is null))
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (Game.Board[Start.X, Start.Y].OccupiedBy.CheckIfIsValidMove(j, 7 - i, Game.Board) &&
                            !Game.Board[Start.X, Start.Y].OccupiedBy.CheckForChecksAfterMove(j, 7 - i, Game.Board))
                        {
                            if (Game.Board[j, 7 - i].OccupiedBy is null)
                            {
                                var circle = new Ellipse()
                                {
                                    Width = 35,
                                    Height = 35,
                                    Margin = new Thickness(25),
                                    Fill = new SolidColorBrush(Color.FromArgb(50, 25, 25, 25)),
                                    IsHitTestVisible = false,
                                };

                                BoardGrid.Children.Add(circle);

                                Grid.SetRow(circle, i);
                                Grid.SetColumn(circle, j);
                            }
                            else
                            {
                                var ring = new Path()
                                {
                                    Fill = new SolidColorBrush(Color.FromArgb(50, 25, 25, 25)),
                                    IsHitTestVisible = false,
                                };

                                ring.Data = new CombinedGeometry(GeometryCombineMode.Xor,
                                    geometry1: new EllipseGeometry(radiusX: 50, radiusY: 50, center: new Point(50, 50)),
                                    geometry2: new EllipseGeometry(radiusX: 42, radiusY: 42, center: new Point(50, 50)));

                                BoardGrid.Children.Add(ring);

                                Grid.SetRow(ring, i);
                                Grid.SetColumn(ring, j);
                            }
                        }
                    }
                }
            }
        }

        private void RenderBoardOnly(Board board)
        {
            BoardGrid.Children.Clear();
            Board.Clear();

            for (int i = 0; i < 8; i++)
            {
                Board.Add(new List<Rectangle>());
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle()
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Tag = $"{j}{7 - i}",
                    };

                    rect.MouseLeftButtonDown += Board_MouseLeftButtonDown;
                    rect.MouseLeftButtonUp += Board_MouseLeftButtonUp;
                    rect.MouseRightButtonDown += Board_MouseRightButtonDown;

                    BoardGrid.Children.Add(rect);

                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 210));
                    }
                    else if (i % 2 == 0 && j % 2 == 1)
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(118, 150, 86));
                    }
                    else if (i % 2 == 1 && j % 2 == 0)
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(118, 150, 86));
                    }
                    else if (i % 2 == 1 && j % 2 == 1)
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 210));
                    }

                    Board[i].Add(rect);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);

                    if (j != Start.X || 7 - i != Start.Y)
                    {
                        RenderModels(i, j, board);
                    }
                    else if (!IsHolding)
                    {
                        RenderModels(i, j, board);
                    }
                }
            }

            if (LastMove != ((-1, -1), (-1, -1)) && Settings.HighlightMoves.IsChecked == true)
            {
                RenderLastMove();
            }
        }

        private void SelectSquare()
        {
            var sq = Board[7 - Start.Y][Start.X];

            if ((7 - Start.Y) % 2 == 0 && Start.X % 2 == 0)
            {
                sq.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
            else if ((7 - Start.Y) % 2 == 0 && Start.X % 2 == 1)
            {
                sq.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
            else if ((7 - Start.Y) % 2 == 1 && Start.X % 2 == 0)
            {
                sq.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
            else if ((7 - Start.Y) % 2 == 1 && Start.X % 2 == 1)
            {
                sq.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
        }

        private void RenderLastMove()
        {
            var sqStart = Board[7 - LastMove.Start.Y][LastMove.Start.X];
            var sqEnd = Board[7 - LastMove.End.Y][LastMove.End.X];

            if ((7 - LastMove.Start.Y) % 2 == 0 && LastMove.Start.X % 2 == 0)
            {
                sqStart.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
            else if ((7 - LastMove.Start.Y) % 2 == 0 && LastMove.Start.X % 2 == 1)
            {
                sqStart.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
            else if ((7 - LastMove.Start.Y) % 2 == 1 && LastMove.Start.X % 2 == 0)
            {
                sqStart.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
            else if ((7 - LastMove.Start.Y) % 2 == 1 && LastMove.Start.X % 2 == 1)
            {
                sqStart.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }

            if ((7 - LastMove.End.Y) % 2 == 0 && LastMove.End.X % 2 == 0)
            {
                sqEnd.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
            else if ((7 - LastMove.End.Y) % 2 == 0 && LastMove.End.X % 2 == 1)
            {
                sqEnd.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
            else if ((7 - LastMove.End.Y) % 2 == 1 && LastMove.End.X % 2 == 0)
            {
                sqEnd.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
            else if ((7 - LastMove.End.Y) % 2 == 1 && LastMove.End.X % 2 == 1)
            {
                sqEnd.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
        }

        private void RenderModels(int i, int j, Board board)
        {
            var piece = board[j, 7 - i].OccupiedBy;

            if (piece is not null)
            {
                string color = Enum.GetName(typeof(PieceColor), piece.Color);
                string pieceType = Enum.GetName(typeof(Piece), piece.Piece);
                string path = System.IO.Path.Combine(@"pack://application:,,,/Chess;component", "Models", color, $"{pieceType}.png");
                var bitmap = new BitmapImage(new Uri(path));

                var image = new Image()
                {
                    Source = bitmap,
                    IsHitTestVisible = false,
                };

                BoardGrid.Children.Add(image);
                Grid.SetRow(image, i);
                Grid.SetColumn(image, j);
            }
        }
    }
}
