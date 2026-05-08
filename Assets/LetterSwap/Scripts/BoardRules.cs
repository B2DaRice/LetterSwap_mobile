using System;

namespace LetterSwap
{
    public static class BoardRules
    {
        public static bool AreAdjacent(BoardCoordinate first, BoardCoordinate second)
        {
            var rowDistance = Math.Abs(first.Row - second.Row);
            var columnDistance = Math.Abs(first.Column - second.Column);
            return rowDistance + columnDistance == 1;
        }
    }
}
