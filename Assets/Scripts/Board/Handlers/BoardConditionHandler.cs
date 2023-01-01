using System;
using System.Collections.Generic;
using System.Linq;
using CollapseShrinkDemo.Board.GamePieceN;
using DG.DeExtensions;
using UnityEngine;

namespace CollapseShrinkDemo.Board.Handlers
{
    public class BoardConditionHandler : MonoBehaviour
    {
        [SerializeField] private Condition[] conditions;
        private Board _board;

        private Condition _maxCondition;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        private void Start()
        {
            _maxCondition = conditions.OrderByDescending(condition => condition.conditionRange.y).First();
        }

        public void CheckConditions(GamePiece[,] gamePieces)
        {
            var alreadyVisitedPieces = new List<GamePiece>();

            foreach (var item in gamePieces)
            {
                if (item is null) continue;

                var neighbours = _board.BoardNeighbourHelper.FindAllNeighboursRecursive(gamePieces, item);
                if (alreadyVisitedPieces.Contains(item)) continue;
                for (int i = conditions.Length - 1; i > -1; i--)
                {
                    if (neighbours.Count >= _maxCondition.conditionRange.y)
                    {
                        ChangePiecesIcon(_maxCondition);
                        break;
                    }

                    if (MeetsCondition(conditions[i], neighbours.Count))
                    {
                        ChangePiecesIcon(conditions[i]);
                        break;
                    }

                    void ChangePiecesIcon(Condition condition)
                    {
                        foreach (var piece in neighbours)
                        {
                            piece.ChangeMyIcon(Resources.Load<Sprite>("GamePieceSprites/Conditions/" + Enum.GetName(typeof(MatchValue), item.MatchValue) + "_" + condition.spriteText));
                        }

                        alreadyVisitedPieces.AddRange(neighbours);
                    }

                    if (i != 0) continue;

                    foreach (var piece in neighbours)
                    {
                        piece.SetDefaultSprite();
                    }
                }
            }
        }

        private bool MeetsCondition(Condition condition, int amount) => amount.IsWithinRange(condition.conditionRange.x, condition.conditionRange.y, false);

        [Serializable]
        private class Condition
        {
            [MinMaxSlider(0, 100)] public Vector2Int conditionRange;

            public string spriteText;
        }
    }
}