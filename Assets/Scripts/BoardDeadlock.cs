using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseShrinkCore.Helpers
{
    public class BoardDeadlock : MonoBehaviour
    {
        private Board _board;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        public bool CheckForDeadlock(GamePiece[,] gamePieces, int minPiecesToExplode)
        {
            foreach (var piece in gamePieces)
            {
                if (piece == null) continue;
                var neighbours = _board.boardNeighbourHelper.FindAllNeighboursRecursive(gamePieces, piece);
                if (neighbours.Count >= minPiecesToExplode) return false;
            }
            return true;
        }
    }
}