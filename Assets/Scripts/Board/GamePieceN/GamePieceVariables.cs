using CanTemplate.Extensions;
using UnityEngine;

namespace CollapseShrinkDemo.Board.GamePieceN
{
    [CreateAssetMenu(menuName = "Game Piece Variables (Core)")]
    public class GamePieceVariables : ScriptableObject
    {
        public EaseSelection pieceMoveEase;
        [MinMaxSlider(0, 100)] public Vector2 movingJumpHeightRange = new(.15f, .25f);
    }
}