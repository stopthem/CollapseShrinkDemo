using System;
using CanTemplate.Extensions;
using CanTemplate.Pooling;
using DG.Tweening;
using UnityEngine;

namespace CollapseShrinkDemo.Board.GamePieceN
{
    public class GamePiece : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public MatchValue MatchValue { get; private set; }

        public Poolable Poolable { get; private set; }

        [SerializeField] private GamePieceVariables gamePieceVariables;

        public Tween MovingTween { get; private set; }

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            Poolable = GetComponent<Poolable>();
        }

        public void Init(int x, int y, MatchValue wantedMatchValue)
        {
            X = x;
            Y = y;
            SetDefaultSprite(wantedMatchValue);
            MatchValue = wantedMatchValue;
        }

        public void Init(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetDefaultSprite(MatchValue? wantedMatchValue = null)
        {
            _spriteRenderer.sprite = Resources.Load<Sprite>($"GamePieceSprites/Default/{Enum.GetName(typeof(MatchValue), wantedMatchValue ?? MatchValue)}_Default");
        }

        public void ChangeMyIcon(Sprite sprite) => _spriteRenderer.sprite = sprite;

        public void Move(int x, int y, float duration)
        {
            name = $"GamePiece X:{x} Y:{y}";

            transform.DOKill();

            var toPos = new Vector2(x, y).ToV3();
            MovingTween = transform.DOPath(new[]
            {
                toPos,
                toPos.WithY(gamePieceVariables.movingJumpHeightRange.GetRandom(), true),
                toPos
            }, duration, PathType.Linear, PathMode.Sidescroller2D).SetEase(gamePieceVariables.pieceMoveEase);
        }
    }

    public enum MatchValue
    {
        Yellow,
        Red,
        Purple,
        Pink,
        Green,
        Blue
    }
}