using System;

namespace LetterSwap
{
    public sealed class BoardModel
    {
        private readonly char[,] letters;

        public BoardModel(int rows, int columns)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Board rows must be greater than zero.");
            }

            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), "Board columns must be greater than zero.");
            }

            Rows = rows;
            Columns = columns;
            letters = new char[rows, columns];
        }

        public int Rows { get; }
        public int Columns { get; }

        public char GetLetter(BoardCoordinate coordinate)
        {
            EnsureInBounds(coordinate);
            return letters[coordinate.Row, coordinate.Column];
        }

        public void SetLetter(BoardCoordinate coordinate, char letter)
        {
            EnsureInBounds(coordinate);
            letters[coordinate.Row, coordinate.Column] = char.ToUpperInvariant(letter);
        }

        public void Fill(ILetterGenerator letterGenerator)
        {
            if (letterGenerator == null)
            {
                throw new ArgumentNullException(nameof(letterGenerator));
            }

            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    SetLetter(new BoardCoordinate(row, column), letterGenerator.NextLetter());
                }
            }
        }

        public void SwapLetters(BoardCoordinate first, BoardCoordinate second)
        {
            EnsureInBounds(first);
            EnsureInBounds(second);

            var firstLetter = letters[first.Row, first.Column];
            letters[first.Row, first.Column] = letters[second.Row, second.Column];
            letters[second.Row, second.Column] = firstLetter;
        }

        public bool IsInBounds(BoardCoordinate coordinate)
        {
            return coordinate.Row >= 0
                && coordinate.Row < Rows
                && coordinate.Column >= 0
                && coordinate.Column < Columns;
        }

        private void EnsureInBounds(BoardCoordinate coordinate)
        {
            if (!IsInBounds(coordinate))
            {
                throw new ArgumentOutOfRangeException(nameof(coordinate), $"Coordinate {coordinate} is outside the board.");
            }
        }
    }
}
