using System;
using System.Collections.Generic;
using System.Linq;

using Chess.Core.Pieces;

namespace Chess.Core
{
    /// <summary>
    /// Represents a chess board.
    /// </summary>
    public class Board : IEquatable<Board>
    {
        /// <summary>
        /// Initializes a new <see cref="Board"/> class with the dafault piece positions.
        /// </summary>
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

        /// <summary>
        /// Initializes a new <see cref="Board"/> class with the piece positions that are taken from the given <paramref name="board"/>.
        /// </summary>
        /// <param name="board">The board from which the pieces are copied.</param>
        public Board(Board board)
        {
            GameBoard = new List<List<Square>>();

            for (int i = 0; i < 8; i++)
            {
                GameBoard.Add(new List<Square>());

                for (int j = 0; j < 8; j++)
                {
                    GameBoard[i].Add(new Square(board.GameBoard[i][j]));
                }
            }
        }

        /// <summary>
        /// Gets the board itself.
        /// </summary>
        public List<List<Square>> GameBoard { get; }

        /// <summary>
        /// Gets the <see cref="Square"/> with the given coordinates.
        /// </summary>
        /// <returns></returns>
        public Square this[int index, int jndex]
            => GameBoard[index][jndex];

        /// <summary>
        /// Occupies the given <see cref="Square"/> with the given <see cref="ChessPiece"/>.
        /// </summary>
        public static void Occupy(Square square, ChessPiece piece)
        {
            square.Occupy(piece);
        }

        /// <summary>
        /// Checks the entire board, to determine the squares that are protected.
        /// </summary>
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

        /// <summary>
        /// Checks whether either of the kings are in check.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if one of the kings are in check; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CheckForCheck(PieceColor color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (GameBoard[i][j].OccupiedBy?.Color == color && GameBoard[i][j].OccupiedBy.Piece == Piece.King)
                    {
                        if (color == PieceColor.White && GameBoard[i][j].IsBlackProtected)
                        {
                            return true;
                        }
                        else if (color == PieceColor.Black && GameBoard[i][j].IsWhiteProtected)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Makes all pawns that were not captured by an en passant move not available for an en passant capture.
        /// </summary>
        public void UnEnPassantAllPawns()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 1; j < 7; j++)
                {
                    if (GameBoard[i][j].OccupiedBy is Pawn pawn)
                    {
                        pawn.CanBeEnPassanted = false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a player with the given color has any valid moves.
        /// </summary>
        /// <param name="color">The color of the player.</param>
        /// <returns>
        /// <see langword="true"/> if the player has at least one valid move; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CheckIfHasValidMoves(PieceColor color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (GameBoard[i][j].OccupiedBy?.Color == color && GameBoard[i][j].OccupiedBy.HasValidMoves(this))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if another same <see cref="ChessPiece"/> could capture the given <see cref="Square"/>.
        /// </summary>
        /// <param name="x">The file of the square.</param>
        /// <param name="y">The rank of the square.</param>
        /// <param name="color">The color of the pieces to check.</param>
        /// <param name="piece">The pieces that should be checked.</param>
        /// <returns>
        /// The <see cref="ChessPiece"/> that could make a capture; <see langword="null"/> if none of the pieces could capture.
        /// </returns>
        public ChessPiece CanAlsoCapture(int x, int y, PieceColor color, Piece piece)
        {
            var mockBoard = new Board(this);
            mockBoard[x, y].Occupy(null);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (mockBoard.GameBoard[i][j].OccupiedBy?.Color == color && mockBoard.GameBoard[i][j].OccupiedBy.Piece == piece &&
                        mockBoard.GameBoard[i][j].OccupiedBy.CheckIfIsValidMove(x, y, mockBoard))
                    {
                        return GameBoard[i][j].OccupiedBy;
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Board board && Equals(board);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(GameBoard);
        }

        /// <inheritdoc/>
        public bool Equals(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!GameBoard[i].SequenceEqual(board.GameBoard[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(Board left, Board right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Board left, Board right)
        {
            return !(left == right);
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
                    GameBoard[i].Add(new Square(new Pawn(-1, -1, PieceColor.Black)));
                    GameBoard[i][j].Occupy(null);
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

            GameBoard[3].Add(new Square(new Queen(3, 7, PieceColor.Black)));
            GameBoard[4].Add(new Square(new King(4, 7, PieceColor.Black)));
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
