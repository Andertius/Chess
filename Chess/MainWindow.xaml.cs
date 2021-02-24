using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            Settings.SettingsClosed += Settings_Close;

            Game = new GameHandler();
            GameHandler.PromotionRequested += Promote;

            Board = new List<List<Rectangle>>();
            MoveButtons = new List<Button>();
            LastSound = new Dictionary<string, List<string>>
            {
                { "White", new List<string>() },
                { "Black", new List<string>() },
            };

            RenderBoardOnly(Game.Board);

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

        private void Move(object sender, EventArgs e)
        {
            if (Game.Move(Start.X, Start.Y, End.X, End.Y))
            {
                HandleSounds();

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

        private void HandleSounds()
        {
            var turn = Game.Turn == PieceColor.White ? "Black" : "White";
            var lastMove = Game.BoardStates[turn][^1];
            string soundName;

            if (lastMove.IsCheck)
            {
                soundName = "Check.wav";
            }
            else if (lastMove.IsMate)
            {
                soundName = "Mate.wav";
            }
            else if (lastMove.IsCapturing)
            {
                soundName = "Capture.wav";
            }            
            else if (lastMove.IsLongCastle || lastMove.IsShortCastle)
            {
                soundName = "Castle.wav";
            }
            else if (lastMove.PawnPromotion is not null)
            {
                soundName = "Promote.wav";
            }
            else
            {
                soundName = "Move.wav";
            }

            if (Settings.PlaySounds.IsChecked == true)
            {
                PlayAudio(soundName);
            }

            LastSound[turn].Add(soundName);
        }

        private static void PlayAudio(string fileName)
        {
            var player = new SoundPlayer(fileName);
            player.Play();
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

            if (Settings.PlaySounds.IsChecked == true)
            {
                PlayAudio(LastSound[$"{Enum.GetName(typeof(PieceColor), SelectedMove.Color)}"][SelectedMove.MoveIndex]);
            }
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings.ReadFile();
            Settings.Visibility = Visibility.Visible;
        }

        private void Settings_Close(object sender, EventArgs e)
        {
            RenderBoardOnly(Game.Board);
        }
    }
}
