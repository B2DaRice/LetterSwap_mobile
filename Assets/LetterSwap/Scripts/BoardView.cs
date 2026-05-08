using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LetterSwap
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform boardRoot;
        [SerializeField] private InputController inputController;
        [SerializeField] private Color tileColor = new Color(0.93f, 0.89f, 0.78f);
        [SerializeField] private Color alternateTileColor = new Color(0.86f, 0.91f, 0.88f);
        [SerializeField] private Color textColor = new Color(0.13f, 0.15f, 0.18f);
        [SerializeField] private Color selectionColor = new Color(0.18f, 0.58f, 0.62f);

        private BoardModel currentBoard;

        private void Update()
        {
            HandlePointerPress();
        }

        public void Render(BoardModel board)
        {
            if (board == null)
            {
                Debug.LogError("BoardView cannot render a null board.");
                return;
            }

            currentBoard = board;
            EnsureBoardRoot();
            EnsureInputController();
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

        public void SetSelectedCoordinate(BoardCoordinate coordinate)
        {
            foreach (var tileView in GetComponentsInChildren<TileView>())
            {
                tileView.SetSelected(tileView.Coordinate.Equals(coordinate));
            }
        }

        public void ClearSelection()
        {
            foreach (var tileView in GetComponentsInChildren<TileView>())
            {
                tileView.SetSelected(false);
            }
        }

        public void SetBoardRoot(RectTransform root)
        {
            boardRoot = root;
        }

        public void SetInputController(InputController controller)
        {
            inputController = controller;
        }

        private void EnsureBoardRoot()
        {
            if (boardRoot == null)
            {
                boardRoot = transform as RectTransform;
            }
        }

        private void EnsureInputController()
        {
            if (inputController == null)
            {
                inputController = FindFirstObjectByType<InputController>();
            }
        }

        private void HandlePointerPress()
        {
            if (currentBoard == null || boardRoot == null || Pointer.current == null)
            {
                return;
            }

            if (!Pointer.current.press.wasPressedThisFrame)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    boardRoot,
                    Pointer.current.position.ReadValue(),
                    null,
                    out var localPoint))
            {
                return;
            }

            if (TryGetCoordinateFromLocalPoint(localPoint, out var coordinate))
            {
                EnsureInputController();
                inputController?.HandleCoordinateTapped(coordinate);
            }
        }

        private bool TryGetCoordinateFromLocalPoint(Vector2 localPoint, out BoardCoordinate coordinate)
        {
            coordinate = default;

            var grid = boardRoot.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                return false;
            }

            var rect = boardRoot.rect;
            var x = localPoint.x - rect.xMin - grid.padding.left;
            var y = rect.yMax - localPoint.y - grid.padding.top;

            if (x < 0f || y < 0f)
            {
                return false;
            }

            var columnPitch = grid.cellSize.x + grid.spacing.x;
            var rowPitch = grid.cellSize.y + grid.spacing.y;
            var column = Mathf.FloorToInt(x / columnPitch);
            var row = Mathf.FloorToInt(y / rowPitch);
            var xInsideSlot = x - column * columnPitch;
            var yInsideSlot = y - row * rowPitch;

            if (xInsideSlot > grid.cellSize.x || yInsideSlot > grid.cellSize.y)
            {
                return false;
            }

            var tappedCoordinate = new BoardCoordinate(row, column);
            if (!currentBoard.IsInBounds(tappedCoordinate))
            {
                return false;
            }

            coordinate = tappedCoordinate;
            return true;
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

            var outlineObject = new GameObject("SelectionOutline");
            outlineObject.transform.SetParent(tileObject.transform, false);

            var outlineRect = outlineObject.AddComponent<RectTransform>();
            outlineRect.anchorMin = Vector2.zero;
            outlineRect.anchorMax = Vector2.one;
            outlineRect.offsetMin = new Vector2(6f, 6f);
            outlineRect.offsetMax = new Vector2(-6f, -6f);

            var outline = outlineObject.AddComponent<Image>();
            outline.color = selectionColor;
            outline.raycastTarget = false;
            outline.enabled = false;

            var letterObject = new GameObject("Letter");
            letterObject.transform.SetParent(tileObject.transform, false);

            var letterRect = letterObject.AddComponent<RectTransform>();
            letterRect.anchorMin = Vector2.zero;
            letterRect.anchorMax = Vector2.one;
            letterRect.offsetMin = Vector2.zero;
            letterRect.offsetMax = Vector2.zero;

            var letterText = letterObject.AddComponent<Text>();
            letterText.alignment = TextAnchor.MiddleCenter;
            letterText.raycastTarget = false;
            letterText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            letterText.fontSize = 54;
            letterText.resizeTextForBestFit = true;
            letterText.resizeTextMinSize = 24;
            letterText.resizeTextMaxSize = 54;

            tileView.Bind(background, letterText, outline);
            tileView.Configure(coordinate, letter, isAlternate ? alternateTileColor : tileColor, textColor);
        }
    }
}
