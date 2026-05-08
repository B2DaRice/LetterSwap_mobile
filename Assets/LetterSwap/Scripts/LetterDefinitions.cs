using System;
using System.Collections.Generic;

namespace LetterSwap
{
    public static class LetterDefinitions
    {
        public static readonly IReadOnlyList<LetterDefinition> All = new[]
        {
            new LetterDefinition('A', 1, 9),
            new LetterDefinition('B', 3, 2),
            new LetterDefinition('C', 3, 2),
            new LetterDefinition('D', 2, 4),
            new LetterDefinition('E', 1, 12),
            new LetterDefinition('F', 4, 2),
            new LetterDefinition('G', 2, 3),
            new LetterDefinition('H', 4, 2),
            new LetterDefinition('I', 1, 9),
            new LetterDefinition('J', 8, 1),
            new LetterDefinition('K', 5, 1),
            new LetterDefinition('L', 1, 4),
            new LetterDefinition('M', 3, 2),
            new LetterDefinition('N', 1, 6),
            new LetterDefinition('O', 1, 8),
            new LetterDefinition('P', 3, 2),
            new LetterDefinition('Q', 10, 1),
            new LetterDefinition('R', 1, 6),
            new LetterDefinition('S', 1, 4),
            new LetterDefinition('T', 1, 6),
            new LetterDefinition('U', 1, 4),
            new LetterDefinition('V', 4, 2),
            new LetterDefinition('W', 4, 2),
            new LetterDefinition('X', 8, 1),
            new LetterDefinition('Y', 4, 2),
            new LetterDefinition('Z', 10, 1),
        };

        private static readonly Dictionary<char, LetterDefinition> ByLetter = CreateLookup();

        public static LetterDefinition Get(char letter)
        {
            var normalizedLetter = char.ToUpperInvariant(letter);
            if (!ByLetter.TryGetValue(normalizedLetter, out var definition))
            {
                throw new ArgumentOutOfRangeException(nameof(letter), $"No definition exists for letter '{letter}'.");
            }

            return definition;
        }

        public static int GetScore(char letter)
        {
            return Get(letter).Score;
        }

        private static Dictionary<char, LetterDefinition> CreateLookup()
        {
            var lookup = new Dictionary<char, LetterDefinition>();
            foreach (var definition in All)
            {
                lookup.Add(definition.Letter, definition);
            }

            return lookup;
        }
    }
}
