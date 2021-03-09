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
        private void RenderModels(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        Board[i][j].Fill = new SolidColorBrush(Color.FromRgb(238, 238, 210));
                    }
                    else
                    {
                        Board[i][j].Fill = new SolidColorBrush(Color.FromRgb(118, 150, 86));
                    }
                }
            }

            ModelGrid.Children.Clear();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (j != Start.X || 7 - i != Start.Y)
                    {
                        RenderModel(i, j, board);
                    }
                    else if (!IsHolding)
                    {
                        RenderModel(i, j, board);
                    }
                }
            }

            if (LastMove != ((-1, -1), (-1, -1)) && Settings.HighlightMoves.IsChecked == true)
            {
                RenderLastMove();
            }

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

                                ModelGrid.Children.Add(circle);

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

                                ModelGrid.Children.Add(ring);

                                Grid.SetRow(ring, i);
                                Grid.SetColumn(ring, j);
                            }
                        }
                    }
                }
            }
        }

        private void RenderBoard()
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

                    if ((i + j) % 2 == 0)
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 210));
                    }
                    else
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(118, 150, 86));
                    }

                    Board[i].Add(rect);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                }
            }

            RenderCoordinates();
        }

        private void SelectSquare()
        {
            int index = 7 - Start.Y;
            int jndex = Start.X;

            var sq = Board[7 - Start.Y][Start.X];

            if ((index + jndex) % 2 == 0)
            {
                sq.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
            else
            {
                sq.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
        }

        private void RenderLastMove()
        {
            int startIndex = 7 - LastMove.Start.Y;
            int startJndex = LastMove.Start.X;
            int endIndex = 7 - LastMove.End.Y;
            int endJndex = LastMove.End.X;

            var sqStart = Board[startIndex][startJndex];
            var sqEnd = Board[endIndex][endJndex];

            if ((startIndex + startJndex) % 2 == 0)
            {
                sqStart.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
            else
            {
                sqStart.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }

            if ((endIndex + endJndex) % 2 == 0)
            {
                sqEnd.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
            }
            else
            {
                sqEnd.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
            }
        }

        private void RenderCoordinates()
        {
            for (int i = 0; i < 8; i++)
            {
                Viewbox vb = new Viewbox()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    RenderTransformOrigin = new Point(0.85, 1),
                };

                TextBlock coord = new TextBlock()
                {
                    FontWeight = FontWeights.Bold,
                    Text = $"{(char)('a' + i)}",
                };

                vb.RenderTransform = new ScaleTransform() { ScaleX = 0.25, ScaleY = 0.25 };

                if (i % 2 == 0)
                {
                    coord.Foreground = new SolidColorBrush(Color.FromRgb(238, 238, 210));
                }
                else
                {
                    coord.Foreground = new SolidColorBrush(Color.FromRgb(118, 150, 86));
                }

                vb.Child = coord;
                BoardGrid.Children.Add(vb);
                Grid.SetColumn(vb, i);
                Grid.SetRow(vb, 7);
            }

            for (int i = 0; i < 8; i++)
            {
                Viewbox vb = new Viewbox()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    RenderTransformOrigin = new Point(0.05, 0),
                };

                TextBlock coord = new TextBlock()
                {
                    FontWeight = FontWeights.Bold,
                    Text = $"{i + 1}",
                };

                vb.RenderTransform = new ScaleTransform() { ScaleX = 0.25, ScaleY = 0.25 };

                if (i % 2 == 0)
                {
                    coord.Foreground = new SolidColorBrush(Color.FromRgb(238, 238, 210));
                }
                else
                {
                    coord.Foreground = new SolidColorBrush(Color.FromRgb(118, 150, 86));
                }

                vb.Child = coord;
                BoardGrid.Children.Add(vb);
                Grid.SetColumn(vb, 0);
                Grid.SetRow(vb, 7 - i);
            }
        }

        private void RenderModel(int i, int j, Board board)
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

                ModelGrid.Children.Add(image);
                Grid.SetRow(image, i);
                Grid.SetColumn(image, j);
            }
        }
    }
}
