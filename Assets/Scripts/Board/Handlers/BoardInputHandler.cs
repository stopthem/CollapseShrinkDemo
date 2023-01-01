using System;
using CanTemplate.Camera;
using CanTemplate.GameManaging;
using CanTemplate.Input;
using CollapseShrinkDemo.Board.GamePieceN;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CollapseShrinkDemo.Board.Handlers
{
    public class BoardInputHandler : MonoBehaviour
    {
        private Board _board;

        [SerializeField] private LayerMask gamePieceLayerMask;

        private void Awake()
        {
            _board = GetComponent<Board>();
        }

        private void OnEnable()
        {
            InputManager.onTouchTap += OnTouchTap;
        }

        private void OnDisable()
        {
            InputManager.onTouchTap -= OnTouchTap;
        }

        private void OnTouchTap(InputAction.CallbackContext obj)
        {
            if (GameManager.gameStatus is not GameStatus.Play) return;

            var ray = CinemachineManager.CamToTouchRay;
            var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, gamePieceLayerMask);

            if (!hit) return;

            _board.CheckForExplodable(hit.collider.GetComponent<GamePiece>());
        }
    }
}