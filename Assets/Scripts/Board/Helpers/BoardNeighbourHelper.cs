using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollapseShrinkDemo.Board.GamePieceN;
using UnityEngine;

namespace CollapseShrinkDemo.Board.Helpers
{
    public class BoardNeighbourHelper : MonoBehaviour
    {
        private Board _board;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        private bool IsWithinBounds(int x, int y) => x < _board.width && x >= 0 && y >= 0 && y < _board.height;

        public List<GamePiece> FindAllNeighboursRecursive(GamePiece[,] gamePieces, GamePiece gamePiece, List<GamePiece> result = null)
        {
            var clickedNeighbours = FindNeighbours(gamePieces, gamePiece.X, gamePiece.Y);

            if (clickedNeighbours == null) return new List<GamePiece>();

            result ??= new List<GamePiece>();

            if (!result.Contains(gamePiece)) result.Add(gamePiece);

            var newPieces = new List<GamePiece>();
            foreach (var piece in clickedNeighbours.Where(piece => !result.Contains(piece)))
            {
                result.Add(piece);
                newPieces.Add(piece);
            }

            foreach (var piece in newPieces)
            {
                FindAllNeighboursRecursive(gamePieces, piece, result);
            }

            return result;
        }

        private List<GamePiece> FindNeighbours(GamePiece[,] gamePieces, int x, int y)
        {
            if (gamePieces[x, y] is null) return new List<GamePiece>();

            List<Tuple<int, int>> potentialNeighs = new List<Tuple<int, int>>
            {
                Tuple.Create(x, y + 1),
                Tuple.Create(x, y - 1),
                Tuple.Create(x - 1, y),
                Tuple.Create(x + 1, y)
            };

            return potentialNeighs.Where(neigh =>
            {
                var potXVal = neigh.Item1;
                var potYVal = neigh.Item2;
                return IsWithinBounds(potXVal, potYVal) && gamePieces[potXVal, potYVal] is not null && gamePieces[potXVal, potYVal].MatchValue == gamePieces[x, y].MatchValue;
            }).Select(x => gamePieces[x.Item1, x.Item2]).ToList();
        }
    }
}