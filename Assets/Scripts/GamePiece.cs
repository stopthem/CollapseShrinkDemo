using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void ChangeMyIcon(Sprite sprite) => _spriteRenderer.sprite = sprite;

    public void Move(int x, int y, float speed)
    {
        LerpManager.LerpOverTime(transform.position, new Vector2(x, y), speed, x => transform.position = x, overrideCurve: _variables.pieceMoveCurve
        , normalAction: () =>
        {
            this.x = x;
            this.y = y;
            Board.Instance.PlaceGamePieceAt(x, y, this);
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