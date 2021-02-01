using System.Collections.Generic;

using Chess.Core.Pieces;

namespace Chess.Core
{
    public class Board
    {
        public Board()
        {
            GameBoard = new List<List<ChessPiece>>();

            for (int i = 0; i < 8; i++)
            {
                GameBoard.Add(new List<ChessPiece>());
            }

            FillWhite();
            FillBlanks();
            FillBlack();
        }

        public List<List<ChessPiece>> GameBoard { get; }

        public ChessPiece this [int index, int jndex]
            => GameBoard[index][jndex];

        public void Occupy(int x, int y, ChessPiece piece)
        {
            GameBoard[x][y] = piece;
        }

        private void FillWhite()
        {
            GameBoard[0].Add(new Rook(0, 0, PieceColor.White));
            GameBoard[7].Add(new Rook(0, 7, PieceColor.White));

            GameBoard[1].Add(new Knight(0, 1, PieceColor.White));
            GameBoard[6].Add(new Knight(0, 6, PieceColor.White));

            GameBoard[2].Add(new Bishop(0, 2, PieceColor.White));
            GameBoard[5].Add(new Bishop(0, 5, PieceColor.White));

            GameBoard[3].Add(new Queen(0, 3, PieceColor.White));
            GameBoard[4].Add(new King(0, 4, PieceColor.White));

            GameBoard[0].Add(new Pawn(1, 0, PieceColor.White));
            GameBoard[1].Add(new Pawn(1, 1, PieceColor.White));
            GameBoard[2].Add(new Pawn(1, 2, PieceColor.White));
            GameBoard[3].Add(new Pawn(1, 3, PieceColor.White));
            GameBoard[4].Add(new Pawn(1, 4, PieceColor.White));
            GameBoard[5].Add(new Pawn(1, 5, PieceColor.White));
            GameBoard[6].Add(new Pawn(1, 6, PieceColor.White));
            GameBoard[7].Add(new Pawn(1, 7, PieceColor.White));
        }

        private void FillBlanks()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 2; j < 6; j++)
                {
                    GameBoard[i].Add(null);
                }
            }
        }

        private void FillBlack()
        {
            GameBoard[0].Add(new Pawn(6, 0, PieceColor.Black));
            GameBoard[1].Add(new Pawn(6, 1, PieceColor.Black));
            GameBoard[2].Add(new Pawn(6, 2, PieceColor.Black));
            GameBoard[3].Add(new Pawn(6, 3, PieceColor.Black));
            GameBoard[4].Add(new Pawn(6, 4, PieceColor.Black));
            GameBoard[5].Add(new Pawn(6, 5, PieceColor.Black));
            GameBoard[6].Add(new Pawn(6, 6, PieceColor.Black));
            GameBoard[7].Add(new Pawn(6, 7, PieceColor.Black));

            GameBoard[0].Add(new Rook(7, 0, PieceColor.Black));
            GameBoard[7].Add(new Rook(7, 7, PieceColor.Black));

            GameBoard[1].Add(new Knight(7, 1, PieceColor.Black));
            GameBoard[6].Add(new Knight(7, 6, PieceColor.Black));

            GameBoard[2].Add(new Bishop(7, 2, PieceColor.Black));
            GameBoard[5].Add(new Bishop(7, 5, PieceColor.Black));

            GameBoard[4].Add(new King(7, 3, PieceColor.Black));
            GameBoard[3].Add(new Queen(7, 4, PieceColor.Black));
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var output = GameBoard[i][j]?.ToString() ?? "0";
                    result += $"{output} ";
                }

                result += "\n";
            }

            return result;
        }
    }
}
