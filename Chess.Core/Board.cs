using System.Collections.Generic;

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

            CheckForProtection();
        }

        public List<List<Square>> GameBoard { get; }

        public Square this [int index, int jndex]
            => GameBoard[index][jndex];

        public static void Occupy(Square square, ChessPiece piece)
        {
            square.Occupy(piece);
        }

        public void CheckForProtection()
        {
            UnportectEverything();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (GameBoard[i][j].OccupiedBy?.Color == PieceColor.White)
                    {
                        foreach (var square in GameBoard[i][j].FindProtectedSquares(this))
                        {
                            square.IsWhiteProtected = true;
                        }
                    }
                    else if (GameBoard[i][j].OccupiedBy?.Color == PieceColor.Black)
                    {
                        foreach (var square in GameBoard[i][j].FindProtectedSquares(this))
                        {
                            square.IsBlackProtected = true;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string result = "";

            for (int j = 7; j > -1; j--)
            {
                for (int i = 0; i < 8; i++)
                {
                    var output = GameBoard[i][j]?.ToString() ?? "0";
                    result += $"{output} ";
                }

                result += "\n";
            }

            return result;
        }

        private void FillWhite()
        {
            GameBoard[0].Add(new Square(new Rook(0, 0, PieceColor.White)));
            GameBoard[7].Add(new Square(new Rook(7, 0, PieceColor.White)));

            GameBoard[1].Add(new Square(new Knight(1, 0, PieceColor.White)));
            GameBoard[6].Add(new Square(new Knight(6, 0, PieceColor.White)));

            GameBoard[2].Add(new Square(new Bishop(2, 0, PieceColor.White)));
            GameBoard[5].Add(new Square(new Bishop(5, 0, PieceColor.White)));

            GameBoard[3].Add(new Square(new Queen(3, 0, PieceColor.White)));
            GameBoard[4].Add(new Square(new King(4, 0, PieceColor.White)));

            GameBoard[0].Add(new Square(new Pawn(0, 1, PieceColor.White)));
            GameBoard[1].Add(new Square(new Pawn(1, 1, PieceColor.White)));
            GameBoard[2].Add(new Square(new Pawn(2, 1, PieceColor.White)));
            GameBoard[3].Add(new Square(new Pawn(3, 1, PieceColor.White)));
            GameBoard[4].Add(new Square(new Pawn(4, 1, PieceColor.White)));
            GameBoard[5].Add(new Square(new Pawn(5, 1, PieceColor.White)));
            GameBoard[6].Add(new Square(new Pawn(6, 1, PieceColor.White)));
            GameBoard[7].Add(new Square(new Pawn(7, 1, PieceColor.White)));
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
            GameBoard[0].Add(new Square(new Pawn(0, 6, PieceColor.Black)));
            GameBoard[1].Add(new Square(new Pawn(1, 6, PieceColor.Black)));
            GameBoard[2].Add(new Square(new Pawn(2, 6, PieceColor.Black)));
            GameBoard[3].Add(new Square(new Pawn(3, 6, PieceColor.Black)));
            GameBoard[4].Add(new Square(new Pawn(4, 6, PieceColor.Black)));
            GameBoard[5].Add(new Square(new Pawn(5, 6, PieceColor.Black)));
            GameBoard[6].Add(new Square(new Pawn(6, 6, PieceColor.Black)));
            GameBoard[7].Add(new Square(new Pawn(7, 6, PieceColor.Black)));

            GameBoard[0].Add(new Square(new Rook(0, 7, PieceColor.Black)));
            GameBoard[7].Add(new Square(new Rook(7, 7, PieceColor.Black)));

            GameBoard[1].Add(new Square(new Knight(1, 7, PieceColor.Black)));
            GameBoard[6].Add(new Square(new Knight(6, 7, PieceColor.Black)));

            GameBoard[2].Add(new Square(new Bishop(2, 7, PieceColor.Black)));
            GameBoard[5].Add(new Square(new Bishop(5, 7, PieceColor.Black)));

            GameBoard[4].Add(new Square(new King(3, 7, PieceColor.Black)));
            GameBoard[3].Add(new Square(new Queen(4, 7, PieceColor.Black)));
        }

        private void UnportectEverything()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameBoard[i][j].IsWhiteProtected = false;
                    GameBoard[i][j].IsBlackProtected = false;
                }
            }
        }
    }
}
