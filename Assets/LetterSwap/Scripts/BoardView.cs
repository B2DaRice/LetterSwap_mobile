using UnityEngine;
using UnityEngine.UI;

namespace LetterSwap
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform boardRoot;
        [SerializeField] private Color tileColor = new Color(0.93f, 0.89f, 0.78f);
        [SerializeField] private Color alternateTileColor = new Color(0.86f, 0.91f, 0.88f);
        [SerializeField] private Color textColor = new Color(0.13f, 0.15f, 0.18f);

        public void Render(BoardModel board)
        {
            if (board == null)
            {
                Debug.LogError("BoardView cannot render a null board.");
                return;
            }

            EnsureBoardRoot();
            ClearTiles();
            ConfigureGrid(board.Columns);

            for (var row = 0; row < board.Rows; row++)
            {
                for (var column = 0; column < board.Columns; column++)
                {
                    var coordinate = new BoardCoordinate(row, column);
                    CreateTile(coordinate, board.GetLetter(coordinate));
                }
            }
        }

        public void SetBoardRoot(RectTransform root)
        {
            boardRoot = root;
        }

        private void EnsureBoardRoot()
        {
            if (boardRoot == null)
            {
                boardRoot = transform as RectTransform;
            }
        }

        private void ClearTiles()
        {
            for (var i = boardRoot.childCount - 1; i >= 0; i--)
            {
                var child = boardRoot.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        private void ConfigureGrid(int columns)
        {
            var grid = boardRoot.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                grid = boardRoot.gameObject.AddComponent<GridLayoutGroup>();
            }

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.cellSize = new Vector2(112f, 112f);
            grid.spacing = new Vector2(8f, 8f);
            grid.padding = new RectOffset(14, 14, 14, 14);
        }

        private void CreateTile(BoardCoordinate coordinate, char letter)
        {
            var tileObject = new GameObject($"Tile_{coordinate.Row}_{coordinate.Column}");
            tileObject.transform.SetParent(boardRoot, false);

            var rectTransform = tileObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(112f, 112f);

            var background = tileObject.AddComponent<Image>();
            var isAlternate = (coordinate.Row + coordinate.Column) % 2 != 0;

            var tileView = tileObject.AddComponent<TileView>();

            var letterObject = new GameObject("Letter");
            letterObject.transform.SetParent(tileObject.transform, false);

            var letterRect = letterObject.AddComponent<RectTransform>();
            letterRect.anchorMin = Vector2.zero;
            letterRect.anchorMax = Vector2.one;
            letterRect.offsetMin = Vector2.zero;
            letterRect.offsetMax = Vector2.zero;

            var letterText = letterObject.AddComponent<Text>();
            letterText.alignment = TextAnchor.MiddleCenter;
            letterText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            letterText.fontSize = 54;
            letterText.resizeTextForBestFit = true;
            letterText.resizeTextMinSize = 24;
            letterText.resizeTextMaxSize = 54;

            tileView.Bind(background, letterText);
            tileView.Configure(coordinate, letter, isAlternate ? alternateTileColor : tileColor, textColor);
        }
    }
}
