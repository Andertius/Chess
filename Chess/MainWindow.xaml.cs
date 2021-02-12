using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer whiteTimer;
        private TimeSpan whiteTime;

        private DispatcherTimer blackTimer;
        private TimeSpan blackTime;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Game = new GameHandler();
            GameHandler.PromotionRequested += Promote;
            Board = new List<List<Rectangle>>();

            RenderBoard();

            Moved += Move;
        }

        public GameHandler Game { get; }

        public List<List<Rectangle>> Board { get; }

        public (int X, int Y) Start { get; private set; } = (-1, -1);

        public (int X, int Y) End { get; private set; }

        public bool IsPromotingPawn { get; set; }

        public bool PieceChosen { get; set; }

        public ChessPiece PromotedPiece { get; set; }

        public int PromotedPawnX { get; set; }

        public bool ToRenderOrNotToRender { get; set; }

        public bool IsHolding { get; set; }

        public bool JustPickedUp { get; set; }

        public (PieceColor Color, int MoveIndex) SelectedMove { get; set; }

        public event EventHandler Moved;

        public void Board_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var rect = (Rectangle)sender;
            string coordinates = (string)rect.Tag;
            JustPickedUp = !(Start.X == Int32.Parse($"{coordinates[0]}") && Start.Y == Int32.Parse($"{coordinates[1]}"));

            if (Start.X != -1 && Game.Board[Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}")].OccupiedBy is null &&
                (!Game.Board[Start.X, Start.Y].OccupiedBy?.CheckIfIsValidMove(Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"), Game.Board) ?? false))
            {
                Start = (-1, -1);
                JustPickedUp = false;
                RenderBoard();
                return;
            }

            if (!IsPromotingPawn && !(Game.Board[Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}")].OccupiedBy is null) && (Start.X == -1 ||
                (!Game.Board[Start.X, Start.Y].OccupiedBy?.CheckIfIsValidMove(Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"), Game.Board) ?? false) ||
                Game.Board[Start.X, Start.Y].OccupiedBy?.Color != Game.Turn))
            {
                Start = (Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"));
                ToRenderOrNotToRender = true;
                IsHolding = true;

                RenderBoard();
                SelectSquare();
            }
        }

        public void Board_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            Start = (-1, -1);
            ToRenderOrNotToRender = false;
            IsHolding = false;
            JustPickedUp = false;

            RenderBoard();
            return;
        }

        public void Board_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            var rect = (Rectangle)sender;
            string coordinates = (string)rect.Tag;
            MouseCanvas.Children.Clear();

            if (Int32.Parse($"{coordinates[0]}") == Start.X && Int32.Parse($"{coordinates[1]}") == Start.Y && !JustPickedUp)
            {
                Start = (-1, -1);
                JustPickedUp = false;
                RenderBoard();
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
            RenderBoard();

            if (Start.X != -1)
            {
                SelectSquare();
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            MouseCanvas.Children.Clear();

            if (Start.X != -1 && IsHolding)
            {
                if (ToRenderOrNotToRender)
                {
                    RenderBoard();
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

        private void Move(object sender, EventArgs e)
        {
            if (Game.Move(Start.X, Start.Y, End.X, End.Y))
            {
                PlaySoundAfterMove();

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

                Start = (-1, -1);
                JustPickedUp = false;

                AddMove();

                if (whiteTimer is null)
                {
                    whiteTime = TimeSpan.FromSeconds(600);
                    blackTime = TimeSpan.FromSeconds(600);

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

                        if (blackTime == TimeSpan.Zero)
                        {
                            blackTimer.Stop();
                            Game.Winner = PieceColor.White;
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

                        if (whiteTime == TimeSpan.Zero)
                        {
                            whiteTimer.Stop();
                            Game.Winner = PieceColor.Black;
                            EndGame();
                        }
                        else if (!(Game.Winner is null))
                        {
                            whiteTimer.Stop();
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

        private void PlaySoundAfterMove()
        {
            var turn = Game.Turn == PieceColor.White ? "Black" : "White";
            var lastMove = Game.BoardStates[turn][^1];

            if (lastMove.IsCheck)
            {
                PlayAudio("Check.wav");
            }
            else if (lastMove.IsMate)
            {
                PlayAudio("Mate.wav");
            }
            else if (lastMove.IsCapturing)
            {
                PlayAudio("Capture.wav");
            }            
            else if (lastMove.IsLongCastle || lastMove.IsShortCastle)
            {
                PlayAudio("Castle.wav");
            }
            else if (!(lastMove.PawnPromotion is null))
            {
                PlayAudio("Promote.wav");
            }
            else
            {
                PlayAudio("Move.wav");
            }
        }

        private static void PlayAudio(string fileName)
        {
            var player = new SoundPlayer(fileName);
            player.Play();
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
                        RenderTracingModel(i, j);
                    }
                    else if (!IsHolding)
                    {
                        RenderTracingModel(i, j);
                    }
                }
            }

            if (Start.X != -1 && Game.Board[Start.X, Start.Y].OccupiedBy?.Color == Game.Turn)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (Game.Board[Start.X, Start.Y].OccupiedBy.CheckIfIsValidMove(j, 7 - i, Game.Board) && !Game.Board[Start.X, Start.Y].OccupiedBy.CheckForChecksAfterMove(j, 7 - i, Game.Board))
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

        private void RenderTracingModel(int i, int j)
        {
            var piece = Game.Board[j, 7 - i].OccupiedBy;

            if (!(piece is null))
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

        private void EndGame()
        {
            if (!(Game.Winner is null))
            {
                MessageBox.Show($"{Enum.GetName(typeof(PieceColor), Game.Winner)} won!");
            }

            switch (Game.Draw)
            {
                case DrawBy.Stalemate:
                    MessageBox.Show($"Stalemate.");
                    break;

                case DrawBy.FiftyMoveRule:
                    MessageBox.Show("Draw by y'all being boring.");
                    break;

                case DrawBy.InsuficientMaterial:
                    MessageBox.Show("Draw by insuficient material.");
                    break;

                case DrawBy.Repetition:
                    MessageBox.Show("Draw by repetition.");
                    break;

                default:
                    break;
            }
        }

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
            };

            MoveHistoryGrid.Children.Add(moveText);
            Grid.SetRow(moveText, Game.BoardStates[color].Count - 1);

            SelectedMove = (Game.Turn == PieceColor.White ? PieceColor.Black : PieceColor.White, Game.BoardStates[color].Count - 1);
            //UpdateSelectedMove();
        }

        private void UpdateSelectedMove()
        {
            int index = 0;

            foreach (var state in Game.BoardStates["White"])
            {
                if (SelectedMove.Color != PieceColor.White || index != SelectedMove.MoveIndex)
                {
                    //TODO Continue this shit
                }
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
