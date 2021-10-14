using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
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

            Settings.SettingsClosed += Settings_Close;
            GameEnd.NewGamed += GameEnd_NewGamed;

            WhiteOffer.Yessed += DrawAccepted;
            WhiteOffer.Noed += DrawRejected;

            BlackOffer.Yessed += DrawAccepted;
            BlackOffer.Noed += DrawRejected;

            Game = new GameHandler();

            Board = new List<List<Rectangle>>();
            MoveButtons = new List<Button>();
            LastSound = new Dictionary<string, List<string>>
            {
                { "White", new List<string>() },
                { "Black", new List<string>() },
            };

            RenderBoard();
            RenderModels(Game.Board);

            Moved += Move;
        }

        public GameHandler Game { get; private set; }

        public List<List<Rectangle>> Board { get; }

        public (int X, int Y) Start { get; private set; } = (-1, -1);

        public (int X, int Y) End { get; private set; }

        public ((int X, int Y) Start, (int X, int Y) End) LastMove { get; set; } = ((-1, -1), (-1, -1));

        public bool IsPromotingPawn { get; set; }

        public int PromotedPawnX { get; set; }

        public bool ToRenderOrNotToRender { get; set; }

        public bool IsHolding { get; set; }

        public bool JustPickedUp { get; set; }

        public (PieceColor Color, int MoveIndex) SelectedMove { get; set; }

        public List<Button> MoveButtons { get; }

        public Dictionary<string, List<string>> LastSound { get; set; }

        public bool IsOnLastMove { get; set; } = true;

        public bool GameStarted { get; set; }

        public bool GameFinished { get; set; }

        public bool DrawOffered { get; set; }

        public event EventHandler Moved;

        private void HandleSounds()
        {
            string turn = Game.Turn == PieceColor.White ? "Black" : "White";
            BoardState lastMove = Game.BoardStates[turn][^1];
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings.ReadFile();
            Settings.Visibility = Visibility.Visible;
        }

        private void Settings_Close(object sender, EventArgs e)
        {
            RenderModels(Game.Board);
        }

        private void RenderCapturedPieces()
        {
            CapturedByWhite.Children.Clear();
            CapturedByBlack.Children.Clear();

            foreach (var piece in Game.Captured["White"])
            {
                var pieceImage = new Image() { Width = 30 };
                pieceImage.Source = new BitmapImage(
                    new Uri(System.IO.Path
                        .Combine("pack://application:,,,/Chess;component",
                            "Models",
                            "Black",
                            $"{Enum.GetName(typeof(Piece), piece.Piece)}.png")));
                CapturedByWhite.Children.Add(pieceImage);
            }

            foreach (var piece in Game.Captured["Black"])
            {
                var pieceImage = new Image() { Width = 30 };
                pieceImage.Source = new BitmapImage(
                    new Uri(System.IO.Path
                    .Combine("pack://application:,,,/Chess;component",
                        "Models",
                        "White",
                        $"{Enum.GetName(typeof(Piece), piece.Piece)}.png")));
                CapturedByBlack.Children.Add(pieceImage);
            }

            var capturedByWhiteValue = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
            };

            var capturedByBlackValue = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
            };

            int white = Game.WhitePieces.Select(x => x.Value).Sum();
            int black = Game.BlackPieces.Select(x => x.Value).Sum();

            if (white > black)
            {
                capturedByWhiteValue.Text = $"+{white - black}";
                CapturedByWhite.Children.Add(capturedByWhiteValue);
            }
            else if (white < black)
            {
                capturedByBlackValue.Text = $"+{black - white}";
                CapturedByBlack.Children.Add(capturedByBlackValue);
            }
        }
    }
}
