using System;

namespace LetterSwap
{
    [Serializable]
    public readonly struct BoardCoordinate : IEquatable<BoardCoordinate>
    {
        public BoardCoordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }
        public int Column { get; }

        public bool Equals(BoardCoordinate other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}
