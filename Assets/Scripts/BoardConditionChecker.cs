using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseShrinkCore.Helpers
{
    public class BoardConditionChecker : MonoBehaviour
    {
        [SerializeField] private Condition[] conditions;
        private Board _board;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        public void CheckConditions(GamePiece[,] gamePieces)
        {
            List<GamePiece> alreadyVisitedPieces = new List<GamePiece>();
            foreach (var item in gamePieces)
            {
                if (item == null) continue;

                var neighbours = _board.boardNeighbourHelper.FindAllNeighboursRecursive(gamePieces, item);
                if (alreadyVisitedPieces.Contains(item)) continue;
                for (int i = conditions.Length - 1; i > -1; i--)
                {
                    if (MeetsCondition(conditions[i], neighbours.Count))
                    {
                        foreach (var piece in neighbours)
                        {
                            piece.ChangeMyIcon(Resources.Load<Sprite>("ConditionalSprites/" + Enum.GetName(typeof(MatchValue), item.matchValue) + "_" + conditions[i].spriteText));
                        }

                        alreadyVisitedPieces.AddRange(neighbours);
                        break;
                    }

                    if (i == 0)
                    {
                        foreach (var piece in neighbours)
                        {
                            piece.ChangeMyIcon(_board.gamePieceDefaultIcons[(int)item.matchValue]);
                        }
                    }
                }
            }
        }

        private bool MeetsCondition(Condition condition, int amount) => amount > condition.moreThan && amount < condition.lessThan;

        [System.Serializable]
        private class Condition
        {
            public int moreThan;
            public int lessThan;
            public string spriteText;
        }
    }
}