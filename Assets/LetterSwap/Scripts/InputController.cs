using UnityEngine;

namespace LetterSwap
{
    public sealed class InputController : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private BoardView boardView;

        private BoardCoordinate? selectedCoordinate;

        public void HandleTileTapped(TileView tileView)
        {
            if (tileView == null)
            {
                return;
            }

            HandleCoordinateTapped(tileView.Coordinate);
        }

        public void HandleCoordinateTapped(BoardCoordinate tappedCoordinate)
        {
            Debug.Log($"Tile tapped: {tappedCoordinate}");

            if (selectedCoordinate == null)
            {
                Select(tappedCoordinate);
                return;
            }

            var firstCoordinate = selectedCoordinate.Value;
            if (firstCoordinate.Equals(tappedCoordinate))
            {
                ClearSelection();
                return;
            }

            if (!BoardRules.AreAdjacent(firstCoordinate, tappedCoordinate))
            {
                Select(tappedCoordinate);
                return;
            }

            ClearSelection();
            ResolveReferences();
            gameController?.HandleAdjacentTilesSelected(firstCoordinate, tappedCoordinate);
        }

        public void SetReferences(GameController controller, BoardView view)
        {
            gameController = controller;
            boardView = view;
        }

        private void Select(BoardCoordinate coordinate)
        {
            selectedCoordinate = coordinate;
            ResolveReferences();
            if (boardView == null)
            {
                Debug.LogError("InputController cannot select a tile because BoardView is missing.");
                return;
            }

            boardView.SetSelectedCoordinate(coordinate);
        }

        private void ClearSelection()
        {
            selectedCoordinate = null;
            ResolveReferences();
            boardView?.ClearSelection();
        }

        private void ResolveReferences()
        {
            if (gameController == null)
            {
                gameController = FindFirstObjectByType<GameController>();
            }

            if (boardView == null)
            {
                boardView = FindFirstObjectByType<BoardView>();
            }
        }
    }
}
