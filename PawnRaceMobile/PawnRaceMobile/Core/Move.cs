using System;

namespace PawnRaceMobile.Core
{
    public struct Move
    {
        public Square From
        {
            get; private set;
        }

        public Square To
        {
            get; private set;
        }

        public bool IsCapture
        {
            get; private set;
        }

        public bool IsEpCapture
        {
            get; private set;
        }

        public string SAN => IsCapture || IsEpCapture
            ? From.Notation.Substring(0, 1) + 'x' + To.Notation
            : To.Notation;

        public Move(Square from, Square to, bool isCapture, bool isEpCapture)
        {
            From = from;
            To = to;
            IsCapture = isCapture;
            IsEpCapture = isEpCapture;
        }

        public Move(Square from, Square to) : this(from, to, false, false)
        {
        }

        public bool IsLong() => Math.Abs(From.Y - To.Y) > 1;
    }
}