using System;
using System.Collections.Generic;
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
            MoveButtons = new List<Button>();
            LastSound = new Dictionary<string, List<string>>
            {
                { "White", new List<string>() },
                { "Black", new List<string>() },
            };

            RenderBoardAfterMove();

            Moved += Move;
        }

        public GameHandler Game { get; }

        public List<List<Rectangle>> Board { get; }

        public (int X, int Y) Start { get; private set; } = (-1, -1);

        public (int X, int Y) End { get; private set; }

        public ((int X, int Y) Start, (int X, int Y) End) LastMove { get; set; } = ((-1, -1), (-1, -1));

        public bool IsPromotingPawn { get; set; }

        public bool PieceChosen { get; set; }

        public ChessPiece PromotedPiece { get; set; }

        public int PromotedPawnX { get; set; }

        public bool ToRenderOrNotToRender { get; set; }

        public bool IsHolding { get; set; }

        public bool JustPickedUp { get; set; }

        public (PieceColor Color, int MoveIndex) SelectedMove { get; set; }

        public List<Button> MoveButtons { get; }

        public Dictionary<string, List<string>> LastSound { get; set; }

        public bool IsOnLastMove { get; set; } = true;

        public event EventHandler Moved;

        public void Board_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (IsOnLastMove)
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
            if (IsOnLastMove)
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
            if (IsOnLastMove)
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

            if (Start.X != -1 && IsHolding)
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

                LastMove = ((Start.X, Start.Y), (End.X, End.Y));
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

                        if (blackTime <= TimeSpan.Zero)
                        {
                            Game.Winner = PieceColor.White;
                            EndGame();
                            whiteTimer.Stop();
                            blackTimer.Stop();
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
                            whiteTimer.Stop();
                            blackTimer.Stop();
                        }
                        else if (!(Game.Winner is null))
                        {
                            EndGame();
                            whiteTimer.Stop();
                            blackTimer.Stop();
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
                LastSound[turn].Add("Check.wav");
            }
            else if (lastMove.IsMate)
            {
                PlayAudio("Mate.wav");
                LastSound[turn].Add("Mate.wav");
            }
            else if (lastMove.IsCapturing)
            {
                PlayAudio("Capture.wav");
                LastSound[turn].Add("Capture.wav");
            }            
            else if (lastMove.IsLongCastle || lastMove.IsShortCastle)
            {
                PlayAudio("Castle.wav");
                LastSound[turn].Add("Castle.wav");
            }
            else if (lastMove.PawnPromotion is not null)
            {
                PlayAudio("Promote.wav");
                LastSound[turn].Add("Promote.wav");
            }
            else
            {
                PlayAudio("Move.wav");
                LastSound[turn].Add("Move.wav");
            }
        }

        private static void PlayAudio(string fileName)
        {
            var player = new SoundPlayer(fileName);
            player.Play();
        }

        private void RenderBoardAfterMove()
        {
            RenderBoardOnly(Game.Board);

            if (Start.X != -1 && Game.Board[Start.X, Start.Y].OccupiedBy?.Color == Game.Turn && (Game.Draw is null || Game.Winner is null))
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

            if (LastMove != ((-1, -1), (-1, -1)))
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
            if ((blackTimer?.IsEnabled ?? false) || (whiteTimer?.IsEnabled ?? false))
            { 
                if (!(Game.Winner is null))
                {
                    whiteTimer.Stop();
                    blackTimer.Stop();
                    MessageBox.Show($"{Enum.GetName(typeof(PieceColor), Game.Winner)} won!");
                }

                switch (Game.Draw)
                {
                    case DrawBy.Stalemate:
                        whiteTimer.Stop();
                        blackTimer.Stop();
                        MessageBox.Show($"Stalemate.");
                        break;

                    case DrawBy.FiftyMoveRule:
                        whiteTimer.Stop();
                        blackTimer.Stop();
                        MessageBox.Show("Draw by y'all being boring.");
                        break;

                    case DrawBy.InsuficientMaterial:
                        whiteTimer.Stop();
                        blackTimer.Stop();
                        MessageBox.Show("Draw by insuficient material.");
                        break;

                    case DrawBy.Repetition:
                        whiteTimer.Stop();
                        blackTimer.Stop();
                        MessageBox.Show("Draw by repetition.");
                        break;

                    default:
                        break;
                }
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
            PlayAudio(LastSound[$"{Enum.GetName(typeof(PieceColor), SelectedMove.Color)}"][SelectedMove.MoveIndex]);
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void FirstMove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMove.Color != PieceColor.White || SelectedMove.MoveIndex != 0)
            {
                SelectedMove = (PieceColor.White, 0);
                LastMove = (Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].Start,
                    Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].End);

                IsOnLastMove = false;
                UpdateSelectedMove();
                UpdateBoard();
            }
        }

        private void PreviousMove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMove.Color != PieceColor.White || SelectedMove.MoveIndex != 0)
            {
                SelectedMove = SelectedMove.Color == PieceColor.Black ? (PieceColor.White, SelectedMove.MoveIndex) : (PieceColor.Black, SelectedMove.MoveIndex - 1);
                LastMove = (Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].Start,
                    Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].End);

                IsOnLastMove = false;
                UpdateSelectedMove();
                UpdateBoard();
            }
        }

        private void NextMove_Click(object sender, RoutedEventArgs e)
        {
            if (!IsOnLastMove)
            {
                SelectedMove = SelectedMove.Color == PieceColor.White ? (PieceColor.Black, SelectedMove.MoveIndex) : (PieceColor.White, SelectedMove.MoveIndex + 1);
                LastMove = (Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].Start,
                    Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].End);

                if ((Game.BoardStates["White"].Count == Game.BoardStates["Black"].Count && SelectedMove.Color == PieceColor.Black &&
                    SelectedMove.MoveIndex == Game.BoardStates["Black"].Count - 1) ||
                    (Game.BoardStates["White"].Count != Game.BoardStates["Black"].Count && SelectedMove.Color == PieceColor.White &&
                    SelectedMove.MoveIndex == Game.BoardStates["White"].Count - 1))
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

        private void LastMove_Click(object sender, RoutedEventArgs e)
        {
            if (!IsOnLastMove)
            {
                if (Game.BoardStates["White"].Count == Game.BoardStates["Black"].Count)
                {
                    SelectedMove = (PieceColor.Black, Game.BoardStates["Black"].Count - 1);
                    LastMove = (Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].Start,
                        Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].End);
                }
                else
                {
                    SelectedMove = (PieceColor.White, Game.BoardStates["White"].Count - 1);
                    LastMove = (Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].Start,
                        Game.BoardStates[Enum.GetName(typeof(PieceColor), SelectedMove.Color)][SelectedMove.MoveIndex].End);
                }

                IsOnLastMove = true;

                UpdateSelectedMove();
                UpdateBoard();
            }
        }
    }
}
