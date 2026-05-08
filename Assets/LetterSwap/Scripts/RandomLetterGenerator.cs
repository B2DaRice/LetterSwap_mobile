using System;
using System.Collections.Generic;
using System.Linq;

namespace LetterSwap
{
    public sealed class RandomLetterGenerator : ILetterGenerator
    {
        private readonly Random random;
        private readonly List<LetterDefinition> letterDefinitions;
        private readonly int totalFrequencyWeight;

        public RandomLetterGenerator(int seed)
            : this(seed, LetterDefinitions.All)
        {
        }

        public RandomLetterGenerator(int seed, IEnumerable<LetterDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            letterDefinitions = definitions.ToList();
            if (letterDefinitions.Count == 0)
            {
                throw new ArgumentException("At least one letter definition is required.", nameof(definitions));
            }

            totalFrequencyWeight = letterDefinitions.Sum(definition => definition.FrequencyWeight);
            if (totalFrequencyWeight <= 0)
            {
                throw new ArgumentException("Total frequency weight must be greater than zero.", nameof(definitions));
            }

            random = new Random(seed);
        }

        public char NextLetter()
        {
            var roll = random.Next(1, totalFrequencyWeight + 1);
            var runningWeight = 0;

            foreach (var definition in letterDefinitions)
            {
                runningWeight += definition.FrequencyWeight;
                if (roll <= runningWeight)
                {
                    return definition.Letter;
                }
            }

            return letterDefinitions[letterDefinitions.Count - 1].Letter;
        }
    }
}
