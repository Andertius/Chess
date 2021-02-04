using System;

namespace Chess.Core
{
    /// <summary>
    /// Represents a class that contains data for when someone takes a turn.
    /// </summary>
    public class MoveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes an instance of a <see cref="MoveEventArgs"/> class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public MoveEventArgs(int x, int y, int newX, int newY)
        {
            X = x;
            Y = y;

            NewX = newX;
            NewY = newY;
            Moved = false;
        }

        /// <summary>
        /// Gets the file from which the player took his turn.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the rank from which the player took his turn.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the file onto which the player took his turn.
        /// </summary>
        public int NewX { get; }

        /// <summary>
        /// Gets the rank onto which the player took his turn.
        /// </summary>
        public int NewY { get; }

        /// <summary>
        /// Gets or sets the value whether the move is valid.
        /// </summary>
        public bool Moved { get; set; }
    }
}
