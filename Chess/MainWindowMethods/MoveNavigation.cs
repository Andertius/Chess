using System;
using System.Windows;

using Chess.Core.Pieces;

namespace Chess
{
    public partial class MainWindow : Window
    {
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
