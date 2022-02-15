using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseShrinkCore
{
    [CreateAssetMenu(menuName = "Game Piece Variables (Core)")]
    public class GamePieceVariables : ScriptableObject
    {
        public AnimationCurve pieceMoveCurve;
    }
}