using System;

namespace Chess.Core
{
    public class MoveEventArgs : EventArgs
    {
        public MoveEventArgs(int x, int y, int newX, int newY)
        {
            X = x;
            Y = y;

            NewX = newX;
            NewY = newY;
            Moved = false;
        }

        public int X { get; }
        public int Y { get; }

        public int NewX { get; }
        public int NewY { get; }

        public bool Moved { get; set; }
    }
}
