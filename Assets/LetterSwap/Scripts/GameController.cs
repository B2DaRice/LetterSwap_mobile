using UnityEngine;

namespace LetterSwap
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private InputController inputController;
        [SerializeField] private int rows = 8;
        [SerializeField] private int columns = 8;
        [SerializeField] private int randomSeed = 1208;

        private BoardModel board;

        private void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            board = new BoardModel(rows, columns);
            board.Fill(new RandomLetterGenerator(randomSeed));

            if (boardView == null)
            {
                boardView = FindFirstObjectByType<BoardView>();
            }

            if (boardView == null)
            {
                Debug.LogError("GameController needs a BoardView to render the board.");
                return;
            }

            if (inputController == null)
            {
                inputController = FindFirstObjectByType<InputController>();
            }

            if (inputController == null)
            {
                inputController = gameObject.AddComponent<InputController>();
            }

            inputController?.SetReferences(this, boardView);
            boardView.SetInputController(inputController);
            boardView.Render(board);
        }

        public void SetBoardView(BoardView view)
        {
            boardView = view;
        }

        public void SetInputController(InputController controller)
        {
            inputController = controller;
        }

        public void HandleAdjacentTilesSelected(BoardCoordinate first, BoardCoordinate second)
        {
            if (board == null)
            {
                Debug.LogError("Cannot swap tiles because the board has not been created.");
                return;
            }

            if (!BoardRules.AreAdjacent(first, second))
            {
                Debug.LogWarning($"Ignoring non-adjacent tile pair: {first} and {second}");
                return;
            }

            board.SwapLetters(first, second);
            boardView.Render(board);
            Debug.Log($"Swapped adjacent tile pair: {first} and {second}");
        }
    }
}
