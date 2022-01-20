using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardConditionChecker : MonoBehaviour
{
    [SerializeField] private Condition[] conditions;

    public void CheckConditions(List<GamePiece> gamePieces)
    {
        List<GamePiece> alreadyVisitedPieces = new List<GamePiece>();
        foreach (var item in gamePieces)
        {
            if (item == null) continue;

            var neighbours = Board.Instance.FindAllNeighboursRecursive(item);
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
                else if (i == 0)
                {
                    foreach (var piece in neighbours)
                    {
                        piece.ChangeMyIcon(Board.Instance.gamePieceDefaultIcons[(int)item.matchValue]);
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
