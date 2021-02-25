using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
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
            GameEnd.NewGamed += GameEnd_NewGamed;

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

        public GameHandler Game { get; private set; }

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

        public bool GameStarted { get; set; }

        public bool GameFinished { get; set; }

        public bool DrawOffered { get; set; }

        public event EventHandler Moved;

        private void EndGame()
        {
            string message = String.Empty;

            if (Game.Winner is not null)
            {
                if (!GameFinished)
                {
                    message = $"{Enum.GetName(typeof(PieceColor), Game.Winner)} won!";
                }                
            }

            switch (Game.Draw)
            {
                case DrawBy.Stalemate:
                    message = $"Stalemate.";
                    break;

                case DrawBy.FiftyMoveRule:
                    message = "Draw by y'all being boring.";
                    break;

                case DrawBy.MutualAgreement:
                    message = "Draw by agreement.";
                    break;

                case DrawBy.InsuficientMaterial:
                    message = "Draw by insuficient material.";
                    break;

                case DrawBy.Repetition:
                    message = "Draw by repetition.";
                    
                    break;

                default:
                    break;
            }

            if (message != String.Empty)
            {
                whiteTimer?.Stop();
                blackTimer?.Stop();
                RenderBoardOnly(Game.Board);
                GameEnd.Message.Text = message;
                GameEnd.Visibility = Visibility.Visible;
                GameFinished = true;
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
    }
}
