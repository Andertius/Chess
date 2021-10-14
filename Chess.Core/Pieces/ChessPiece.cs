using System;

namespace Chess.Core.Pieces
{
    /// <summary>
    /// An abstract base class for the chess pieces.
    /// </summary>
    public abstract class ChessPiece : IEquatable<ChessPiece>, IComparable<ChessPiece>
    {
        /// <summary>
        /// Initializes a new piece with the given coordinates, color, and type.
        /// </summary>
        /// <param name="x">The file in which the piece is located.</param>
        /// <param name="y">The rank in which the piece is located.</param>
        /// <param name="color">The color of the piece.</param>
        /// <param name="value">The value of the piece.</param>
        /// <param name="piece">The type of the piece.</param>
        public ChessPiece(int x, int y, PieceColor color, int value, Piece piece)
        {
            X = x;
            Y = y;
            Color = color;
            Value = value;
            Piece = piece;
        }

        /// <summary>
        /// Gets the value of the <see cref="King"/> piece.
        /// </summary>
        protected static int KingValue => 0;

        /// <summary>
        /// Gets or sets the value that indicates whether the <see cref="King"/> piece just castled with the queenside <see cref="Rook"/>.
        /// </summary>
        public bool JustLongCastled { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the <see cref="King"/> piece just castled with the kingside <see cref="Rook"/>.
        /// </summary>
        public bool JustShortCastled { get; set; }

        /// <summary>
        /// Gets the value of the file in which the piece is located.
        /// </summary>
        public int X { get; protected set; }

        /// <summary>
        /// Gets the value of the rank in which the piece is located.
        /// </summary>
        public int Y { get; protected set; }

        /// <summary>
        /// Gets the color value of the piece.
        /// </summary>
        public PieceColor Color { get; }

        /// <summary>
        /// Gets the piece type.
        /// </summary>
        public Piece Piece { get; }

        /// <summary>
        /// Gets the value of the piece.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets or sets the value indicating whether the piece is a promoted pawn.
        /// </summary>
        public bool PromotedFromPawn { get; set; }

        /// <summary>
        /// Moves the <see cref="ChessPiece"/> if it is a valid move, freeing up the previous <see cref="Square"/>.
        /// </summary>
        /// <param name="newX">The destination file.</param>
        /// <param name="newY">The destination rank.</param>
        /// <param name="board">The board in which the move is executed.</param>
        /// <param name="capturedPiece">A captured piece. <see langword="null"/> if nothing was captured.</param>
        /// <param name="isMock">A <see cref="Boolean"/> value indicating whether the board is a mocked board.</param>
        /// <returns><see langword="true"/> if the move is valid; otherwise, <see langword="false"/>.</returns>
        public virtual bool Move(int newX, int newY, Board board, out ChessPiece capturedPiece, bool isMock)
        {
            if ((newX != X || newY != Y) && CheckIfIsValidMove(newX, newY, board))
            {
                if (!isMock && CheckForChecksAfterMove(newX, newY, board))
                {
                    capturedPiece = null;
                    return false;
                }

                JustLongCastled = false;
                JustShortCastled = false;

                capturedPiece = board[newX, newY]?.OccupiedBy;

                Board.Occupy(board[newX, newY], board[X, Y].OccupiedBy);
                Board.Occupy(board[X, Y], null);

                X = newX;
                Y = newY;

                board.CheckForProtection();
                return true;
            }

            capturedPiece = null;
            return false;
        }

        /// <summary>
        /// Checks whether the given move is valid.
        /// </summary>
        /// <param name="newX">The file of the move.</param>
        /// <param name="newY">The rank of the move.</param>
        /// <param name="board">The board in which the move is executed.</param>
        /// <returns><see langword="true"/> if the move is valid; otherwise, <see langword="false"/>.</returns>
        public virtual bool CheckIfIsValidMove(int newX, int newY, Board board) => true;

        /// <summary>
        /// Checks whether the piece portects the given square.
        /// </summary>
        /// <param name="x">The file of the square.</param>
        /// <param name="y">The rank of the square.</param>
        /// <param name="board">The board in which the square should be located.</param>
        /// <returns><see langword="true"/> if the square is protected; otherwise, <see langword="false"/>.</returns>
        public virtual bool Protects(int x, int y, Board board) => true;

        /// <summary>
        /// Checks whether the piece has any valid moves.
        /// </summary>
        /// <param name="board">The board in which the moves should be searched.</param>
        /// <returns><see langword="true"/> if the piece has valid moves; otherwise, <see langword="false"/>.</returns>
        public bool HasValidMoves(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (CheckIfIsValidMove(i, j, board) && !CheckForChecksAfterMove(i, j, board))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ChessPiece chess && Equals(chess);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value, Color);
        }

        /// <inheritdoc/>
        public bool Equals(ChessPiece other)
        {
            return !(other is null) && X == other.X && Y == other.Y && Value == other.Value && Color == other.Color && Piece == other.Piece;
        }

        /// <summary>
        /// Checks if the piece is pinned by checking if the move is opening a check to the ally king.
        /// </summary>
        /// <param name="newX">The file onto which the <see cref="ChessPiece"/> should move.</param>
        /// <param name="newY">The rank onto which the <see cref="ChessPiece"/> should move.</param>
        /// <param name="board">The board in which the move should be executed.</param>
        /// <returns><see langword="true"/> if the move is opening a check to the ally king; otherwise, <see langword="false"/>.</returns>
        public bool CheckForChecksAfterMove(int newX, int newY, Board board)
        {
            var mockBoard = new Board(board);
            mockBoard[X, Y].Move(newX, newY, mockBoard, out _, true);

            return mockBoard.CheckForCheck(Color);
        }

        public int CompareTo(ChessPiece other)
        {
            return Value < other.Value ? -1 : Value > other.Value ? 1 : 0;
        }
    }
}
