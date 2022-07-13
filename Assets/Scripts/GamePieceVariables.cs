using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CollapseShrinkCore
{
    [CreateAssetMenu(menuName = "Game Piece Variables (Core)")]
    public class GamePieceVariables : ScriptableObject
    {
        public EaseSelection pieceMoveEase;
    }
}