using System.Collections;
using System.Collections.Generic;
using CanTemplate.Extensions;
using DG.Tweening;
using UnityEngine;

namespace CollapseShrinkCore
{
    public class GamePiece : MonoBehaviour
    {
        [HideInInspector] public int x, y;
        [HideInInspector] public MatchValue matchValue;

        [SerializeField] private GamePieceVariables _variables;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Init(int x, int y, Sprite sprite, MatchValue matchValue)
        {
            this.x = x;
            this.y = y;
            _spriteRenderer.sprite = sprite;
            this.matchValue = matchValue;
        }

        public void Init(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void ChangeMyIcon(Sprite sprite) => _spriteRenderer.sprite = sprite;

        public void Move(int x, int y, float duration)
        {
            transform.DOKill();
            transform.DOMove(new Vector2(x, y), duration).SetEase(_variables.pieceMoveEase)
                .OnComplete(() => name = "GamePiece " + "x" + x + " y" + y);
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