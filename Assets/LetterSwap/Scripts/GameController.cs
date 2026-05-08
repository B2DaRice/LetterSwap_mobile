using UnityEngine;

namespace LetterSwap
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private int rows = 8;
        [SerializeField] private int columns = 8;
        [SerializeField] private int randomSeed = 1208;
        [SerializeField] private string letterSource = "EEEEEEEEEEEEAAAAAAAAAIIIIIIIIOOOOOOOONNNNNNRRRRRRTTTTTTLLLLSSSSUUUUDDDDGGGBBCCMMPPFFHHVVWWYYKJXQZ";

        private BoardModel board;

        private void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            board = new BoardModel(rows, columns);
            board.Fill(new RandomLetterGenerator(randomSeed, letterSource));

            if (boardView == null)
            {
                boardView = FindFirstObjectByType<BoardView>();
            }

            if (boardView == null)
            {
                Debug.LogError("GameController needs a BoardView to render the board.");
                return;
            }

            boardView.Render(board);
        }

        public void SetBoardView(BoardView view)
        {
            boardView = view;
        }
    }
}
