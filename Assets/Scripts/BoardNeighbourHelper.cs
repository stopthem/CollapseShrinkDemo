using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollapseShrinkCore.Helpers
{
    public class BoardNeighbourHelper : MonoBehaviour
    {
        private Board _board;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        private bool IsWithinBounds(int x, int y) => (x < _board.width && x >= 0) && (y >= 0 && y < _board.height);

        public List<GamePiece> FindAllNeighboursRecursive(GamePiece[,] gamePieces, GamePiece gamePiece, List<GamePiece> result = null)
        {
            List<GamePiece> clickedNeighbours = FindNeighbours(gamePieces, gamePiece.x, gamePiece.y);

            if (clickedNeighbours == null) return new List<GamePiece>();

            if (result == null)
            {
                result = new List<GamePiece>();
            }
            if (!result.Contains(gamePiece)) result.Add(gamePiece);

            var newPieces = new List<GamePiece>();
            foreach (var piece in clickedNeighbours)
            {
                if (!result.Contains(piece))
                {
                    result.Add(piece);
                    newPieces.Add(piece);
                }
            }

            foreach (var piece in newPieces)
            {
                FindAllNeighboursRecursive(gamePieces, piece, result);
            }

            return result;
        }

        private List<GamePiece> FindNeighbours(GamePiece[,] gamePieces, int x, int y)
        {
            if (gamePieces[x, y] == null) return new List<GamePiece>();

            List<Tuple<int, int>> potentialNeighs = new List<Tuple<int, int>>();

            potentialNeighs.Add(Tuple.Create(x, y + 1));
            potentialNeighs.Add(Tuple.Create(x, y - 1));
            potentialNeighs.Add(Tuple.Create(x - 1, y));
            potentialNeighs.Add(Tuple.Create(x + 1, y));

            return potentialNeighs.Where(neigh =>
            {
                int potXVal = neigh.Item1;
                int potYVal = neigh.Item2;
                return IsWithinBounds(potXVal, potYVal) && gamePieces[potXVal, potYVal] != null && gamePieces[potXVal, potYVal].matchValue == gamePieces[x, y].matchValue;
            }).Select(x => gamePieces[x.Item1, x.Item2]).ToList();
        }
    }
}
