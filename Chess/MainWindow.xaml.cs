using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Chess.Core;
using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Game = new GameHandler();
            Board = new List<List<Rectangle>>();

            RenderBoard();

            Moved += Move;
        }

        public GameHandler Game { get; }

        public List<List<Rectangle>> Board { get; }

        public int StartX { get; private set; } = -1;

        public int StartY { get; private set; } = -1;

        public int EndX { get; private set; }

        public int EndY { get; private set; }

        public event EventHandler Moved;

        public void Board_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var rect = (Rectangle)sender;
            string coordinates = (string)rect.Tag;

            if ((StartX == -1 && StartY == -1) ||
                (!Game.Board[StartX, StartY].OccupiedBy?.IsValidMove(Int32.Parse($"{coordinates[0]}"), Int32.Parse($"{coordinates[1]}"), Game.Board) ?? false))
            {
                StartX = Int32.Parse($"{coordinates[0]}");
                StartY = Int32.Parse($"{coordinates[1]}");

                RenderBoard();
                var sq = Board[7 - StartY][StartX];

                if ((7 - StartY) % 2 == 0 && StartX % 2 == 0)
                {
                    sq.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
                }
                else if ((7 - StartY) % 2 == 0 && StartX % 2 == 1)
                {
                    sq.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
                }
                else if ((7 - StartY) % 2 == 1 && StartX % 2 == 0)
                {
                    sq.Fill = new SolidColorBrush(Color.FromRgb(186, 202, 43));
                }
                else if ((7 - StartY) % 2 == 1 && StartX % 2 == 1)
                {
                    sq.Fill = new SolidColorBrush(Color.FromRgb(246, 246, 105));
                }
            }
        }

        public void Board_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (StartX != -1 && StartY != -1)
            {
                var rect = (Rectangle)sender;
                string coordinates = (string)rect.Tag;
                EndX = Int32.Parse($"{coordinates[0]}");
                EndY = Int32.Parse($"{coordinates[1]}");

                //TODO print valid moves for the piece

                if (StartX != EndX || StartY != EndY)
                {
                    Moved?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Move(object sender, EventArgs e)
        {
            if (Game.Move(StartX, StartY, EndX, EndY))
            {
                RenderBoard();
                PlaySoundAfterMove(true);

                if (Game.Winner == PieceColor.White)
                {
                    MessageBox.Show("White won!");
                }
                else if (Game.Winner == PieceColor.Black)
                {
                    MessageBox.Show("Black won!");
                }

                if (Game.IsInStalemate)
                {
                    MessageBox.Show("Stalemate.");
                }
            }
            else
            {
                PlaySoundAfterMove(false);
            }

            StartX = -1;
            StartY = -1;
        }

        private void PlaySoundAfterMove(bool didMove)
        {
            if (!didMove)
            {
                return;
            }

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

                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle();
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
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = Game.Board[j, i].OccupiedBy;

                    if (!(piece is null))
                    {
                        string color = Enum.GetName(typeof(PieceColor), piece.Color);
                        string pieceType = Enum.GetName(typeof(Piece), piece.Piece);
                        string path = System.IO.Path.Combine(@"pack://application:,,,/Chess;component", "Models", color, $"{pieceType}.png");
                        var bitmap = new BitmapImage(new Uri(path));

                        var image = new Image()
                        {
                            Source = bitmap,
                        };

                        BoardGrid.Children.Add(image);
                        Grid.SetRow(image, 7 - i);
                        Grid.SetColumn(image, j);
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle()
                    {
                        Fill = Brushes.Transparent,
                        Tag = $"{j}{7 - i}",
                    };

                    BoardGrid.Children.Add(rect);

                    rect.MouseLeftButtonDown += Board_MouseLeftButtonDown;
                    rect.MouseLeftButtonUp += Board_MouseLeftButtonUp;

                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                }
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
