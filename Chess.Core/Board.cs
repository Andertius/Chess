using System;
using System.Collections.Generic;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class Board
    {
        public Board()
        {
            GameBoard = new List<List<Square>>();

            for (int i = 0; i < 8; i++)
            {
                GameBoard.Add(new List<Square>());
            }

            FillWhite();
            FillBlanks();
            FillBlack();
        }

        public List<List<Square>> GameBoard { get; }

        public Square this [int index, int jndex]
            => GameBoard[index][jndex];

        private void FillWhite()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    GameBoard[i].Add(new Square(PieceColor.White));
                }
            }
        }

        private void FillBlanks()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 2; j < 6; j++)
                {
                    GameBoard[i].Add(new Square(null));
                }
            }
        }

        private void FillBlack()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 6; j < 8; j++)
                {
                    GameBoard[i].Add(new Square(PieceColor.Black));
                }
            }
        }
    }
}
