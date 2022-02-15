using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseShrinkCore
{
    public class GamePiece : MonoBehaviour
    {
        [HideInInspector] public int x, y;
        [HideInInspector] public MatchValue matchValue;

        [SerializeField] private GamePieceVariables _variables;

        private IEnumerator _movingRoutine;

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

        public void Move(int x, int y, float speed)
        {
            if (_movingRoutine != null) LerpManager.Instance.StopCoroutine(_movingRoutine);
            LerpManager.LerpOverTime(transform.position, new Vector2(x, y), speed, x => transform.position = x, out _movingRoutine, overrideCurve: _variables.pieceMoveCurve
            , normalAction: () =>
              {
                  name = "GamePiece " + "x" + x + " y" + y;
              });
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