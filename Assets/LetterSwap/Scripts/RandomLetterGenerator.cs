using System;

namespace LetterSwap
{
    public sealed class RandomLetterGenerator : ILetterGenerator
    {
        private const string DefaultLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly Random random;
        private readonly string letters;

        public RandomLetterGenerator(int seed, string letters = DefaultLetters)
        {
            if (string.IsNullOrWhiteSpace(letters))
            {
                throw new ArgumentException("Letter source cannot be empty.", nameof(letters));
            }

            random = new Random(seed);
            this.letters = letters.ToUpperInvariant();
        }

        public char NextLetter()
        {
            return letters[random.Next(letters.Length)];
        }
    }
}
