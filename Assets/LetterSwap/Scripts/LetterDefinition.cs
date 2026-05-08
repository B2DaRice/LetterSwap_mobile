namespace LetterSwap
{
    public readonly struct LetterDefinition
    {
        public LetterDefinition(char letter, int score, int frequencyWeight)
        {
            Letter = char.ToUpperInvariant(letter);
            Score = score;
            FrequencyWeight = frequencyWeight;
        }

        public char Letter { get; }
        public int Score { get; }
        public int FrequencyWeight { get; }
    }
}
