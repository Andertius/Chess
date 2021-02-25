using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        public void Board_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (!GameFinished && IsOnLastMove && Settings.Visibility == Visibility.Collapsed)
            {
                var rect = (Rectangle)sender;
                string coordinates = (string)rect.Tag;
                JustPickedUp = !(Start.X == Int32.Parse($"{coordinates[0]}") && Start.Y == Int32.Parse($"{coordinates[1]}"));

                if (Start.X != -1 && Game.Board[Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}")].OccupiedBy is null &&
                    (!Game.Board[Start.X, Start.Y].OccupiedBy?.CheckIfIsValidMove(Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"), Game.Board) ?? false))
                {
                    Start = (-1, -1);
                    JustPickedUp = false;
                    RenderBoardAfterMove();
                    return;
                }

                if (!IsPromotingPawn && !(Game.Board[Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}")].OccupiedBy is null) && (Start.X == -1 ||
                    (!Game.Board[Start.X, Start.Y].OccupiedBy?.CheckIfIsValidMove(Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"), Game.Board) ?? false) ||
                    Game.Board[Start.X, Start.Y].OccupiedBy?.Color != Game.Turn))
                {
                    Start = (Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"));
                    ToRenderOrNotToRender = true;
                    IsHolding = true;

                    RenderBoardAfterMove();
                    SelectSquare();
                }
            }
        }

        public void Board_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (!GameFinished && IsOnLastMove && Settings.Visibility == Visibility.Collapsed)
            {
                Start = (-1, -1);
                ToRenderOrNotToRender = false;
                IsHolding = false;
                JustPickedUp = false;

                RenderBoardAfterMove();
            }
        }

        public void Board_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (Settings.Visibility != Visibility.Collapsed)
            {
                Settings.Exit(this, new RoutedEventArgs());
            }
            else if (!GameFinished && IsOnLastMove)
            {
                var rect = (Rectangle)sender;
                string coordinates = (string)rect.Tag;
                MouseCanvas.Children.Clear();

                if (Int32.Parse($"{coordinates[0]}") == Start.X && Int32.Parse($"{coordinates[1]}") == Start.Y && !JustPickedUp)
                {
                    Start = (-1, -1);
                    JustPickedUp = false;
                    RenderBoardAfterMove();
                    return;
                }

                if (!IsPromotingPawn && Start.X != -1)
                {
                    End = (Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"));

                    if (Start.X != End.X || Start.Y != End.Y)
                    {
                        Moved?.Invoke(this, EventArgs.Empty);
                    }
                }

                ToRenderOrNotToRender = false;
                IsHolding = false;
                RenderBoardAfterMove();

                if (Start.X != -1)
                {
                    SelectSquare();
                }

                EndGame();
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            MouseCanvas.Children.Clear();

            if (Start.X != -1 && IsHolding && !GameFinished)
            {
                if (ToRenderOrNotToRender)
                {
                    RenderBoardAfterMove();
                    SelectSquare();

                    ToRenderOrNotToRender = false;
                }

                Point position = e.GetPosition(this);
                var piece = Game.Board[Start.X, Start.Y].OccupiedBy; string color = Enum.GetName(typeof(PieceColor), piece.Color);

                string pieceType = Enum.GetName(typeof(Piece), piece.Piece);
                string path = System.IO.Path.Combine(@"pack://application:,,,/Chess;component", "Models", color, $"{pieceType}.png");
                var bitmap = new BitmapImage(new Uri(path));

                var image = new Image()
                {
                    Source = bitmap,
                    IsHitTestVisible = false,
                    Width = 100,
                };

                MouseCanvas.Children.Add(image);
                Canvas.SetLeft(image, position.X - LeftColumn.ActualWidth - 50);
                Canvas.SetTop(image, position.Y - TopRow.ActualHeight - AlmostTopRow.ActualHeight - 50);
            }
        }
    }
}
