using UnityEngine;
using UnityEngine.UI;

namespace LetterSwap
{
    public sealed class TileView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Text letterText;
        [SerializeField] private Image selectionOutline;

        private Color normalBackgroundColor;
        private readonly Color selectedBackgroundColor = new Color(0.18f, 0.86f, 0.92f);
        public BoardCoordinate Coordinate { get; private set; }

        public void Configure(BoardCoordinate coordinate, char letter, Color backgroundColor, Color textColor)
        {
            Coordinate = coordinate;

            if (background != null)
            {
                normalBackgroundColor = backgroundColor;
                background.color = backgroundColor;
            }

            if (letterText != null)
            {
                letterText.text = letter.ToString();
                letterText.color = textColor;
            }
        }

        public void Bind(Image tileBackground, Text tileLetterText, Image tileSelectionOutline)
        {
            background = tileBackground;
            letterText = tileLetterText;
            selectionOutline = tileSelectionOutline;
        }

        public void SetSelected(bool selected)
        {
            if (selectionOutline != null)
            {
                selectionOutline.enabled = selected;
            }

            if (background != null)
            {
                background.color = selected ? selectedBackgroundColor : normalBackgroundColor;
            }

            if (letterText != null)
            {
                letterText.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal;
            }
        }
    }
}
