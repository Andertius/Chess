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
            Offer.Yessed += DrawAccepted;
            Offer.Noed += DrawRejected;

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

        public bool GameFinished { get; set; }

        public bool DrawOffered { get; set; }

        public event EventHandler Moved;

        private void EndGame()
        {
            if (Game.Winner is not null)
            {
                if (!GameFinished)
                {
                    whiteTimer?.Stop();
                    blackTimer?.Stop();
                    RenderBoardOnly(Game.Board);
                    MessageBox.Show($"{Enum.GetName(typeof(PieceColor), Game.Winner)} won!");
                    GameFinished = true;
                }                
            }

            switch (Game.Draw)
            {
                case DrawBy.Stalemate:
                    whiteTimer?.Stop();
                    blackTimer?.Stop();
                    RenderBoardOnly(Game.Board);
                    MessageBox.Show($"Stalemate.");
                    GameFinished = true;
                    break;

                case DrawBy.FiftyMoveRule:
                    whiteTimer?.Stop();
                    blackTimer?.Stop();
                    RenderBoardOnly(Game.Board);
                    MessageBox.Show("Draw by y'all being boring.");
                    GameFinished = true;
                    break;

                case DrawBy.MutualAgreement:
                    whiteTimer?.Stop();
                    blackTimer?.Stop();
                    RenderBoardOnly(Game.Board);
                    MessageBox.Show("Draw by agreement.");
                    GameFinished = true;
                    break;

                case DrawBy.InsuficientMaterial:
                    whiteTimer?.Stop();
                    blackTimer?.Stop();
                    RenderBoardOnly(Game.Board);
                    MessageBox.Show("Draw by insuficient material.");
                    GameFinished = true;
                    break;

                case DrawBy.Repetition:
                    whiteTimer?.Stop();
                    blackTimer?.Stop();
                    RenderBoardOnly(Game.Board);
                    MessageBox.Show("Draw by repetition.");
                    GameFinished = true;
                    break;

                default:
                    break;
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings.ReadFile();
            Settings.Visibility = Visibility.Visible;
        }

        private void Settings_Close(object sender, EventArgs e)
        {
            RenderBoardOnly(Game.Board);
        }

        private void WhiteResign_Click(object sender, RoutedEventArgs e)
        {
            if (Game.Turn == PieceColor.White)
            {
                if (WhiteConfirmation.Visibility == Visibility.Visible)
                {
                    WhiteConfirmation.Visibility = Visibility.Collapsed;
                }
                else if (Settings.ResignConfirmation.IsChecked == false)
                {
                    Game.Winner = PieceColor.Black;
                    EndGame();
                }
                else
                {
                    WhiteConfirmation.Visibility = Visibility.Visible;
                }
            }
        }

        private void WhiteYes_Click(object sender, RoutedEventArgs e)
        {
            Game.Winner = PieceColor.Black;
            WhiteConfirmation.Visibility = Visibility.Collapsed;

            EndGame();
        }

        private void WhiteNo_Click(object sender, RoutedEventArgs e)
        {
            WhiteConfirmation.Visibility = Visibility.Collapsed;
        }

        private void BlackResign_Click(object sender, RoutedEventArgs e)
        {
            if (Game.Turn == PieceColor.Black)
            {
                if (BlackConfirmation.Visibility == Visibility.Visible)
                {
                    BlackConfirmation.Visibility = Visibility.Collapsed;
                }
                else if (Settings.ResignConfirmation.IsChecked == false)
                {
                    Game.Winner = PieceColor.White;
                    EndGame();
                }
                else
                {
                    BlackConfirmation.Visibility = Visibility.Visible;
                }
            }
        }

        private void BlackYes_Click(object sender, RoutedEventArgs e)
        {
            Game.Winner = PieceColor.White;
            BlackConfirmation.Visibility = Visibility.Collapsed;

            EndGame();
        }

        private void BlackNo_Click(object sender, RoutedEventArgs e)
        {
            BlackConfirmation.Visibility = Visibility.Collapsed;
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            Offer.Visibility = Visibility.Visible;
        }

        private void DrawRejected(object sender, EventArgs e)
        {
            Offer.Visibility = Visibility.Collapsed;
        }

        private void DrawAccepted(object sender, EventArgs e)
        {
            Game.Draw = DrawBy.MutualAgreement;
            Offer.Visibility = Visibility.Collapsed;

            EndGame();
        }
    }
}
