using CanTemplate.GameManaging;
using CanTemplate.ReactionText;
using CollapseShrinkDemo.Board.GamePieceN;
using UnityEngine;

namespace CollapseShrinkDemo.Board.Handlers
{
    public class BoardDeadlockHandler : MonoBehaviour
    {
        private Board _board;

        [SerializeField] private ReactionTextInfo deadlockTextInfo;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        public bool CheckForDeadlock(GamePiece[,] gamePieces, int minPiecesToExplode)
        {
            foreach (var piece in gamePieces)
            {
                if (piece is null) continue;

                var neighbours = _board.BoardNeighbourHelper.FindAllNeighboursRecursive(gamePieces, piece);

                if (neighbours.Count < minPiecesToExplode) continue;

                GameManager.gameStatus = GameStatus.Play;
                return false;
            }

            GameManager.gameStatus = GameStatus.Deadlocked;

            ReactionTextManager.CreateText(deadlockTextInfo);

            return true;
        }
    }
}