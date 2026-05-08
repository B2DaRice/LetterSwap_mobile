using UnityEngine;
using UnityEngine.UI;

namespace LetterSwap
{
    public sealed class TileView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Text letterText;

        public BoardCoordinate Coordinate { get; private set; }

        public void Configure(BoardCoordinate coordinate, char letter, Color backgroundColor, Color textColor)
        {
            Coordinate = coordinate;

            if (background != null)
            {
                background.color = backgroundColor;
            }

            if (letterText != null)
            {
                letterText.text = letter.ToString();
                letterText.color = textColor;
            }
        }

        public void Bind(Image tileBackground, Text tileLetterText)
        {
            background = tileBackground;
            letterText = tileLetterText;
        }
    }
}
