using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CanTemplate.Camera;
using CanTemplate.Extensions;
using CanTemplate.GameManaging;
using CanTemplate.Pooling;
using CanTemplate.UI;
using CanTemplate.Utilities;
using CollapseShrinkDemo.Board.GamePieceN;
using CollapseShrinkDemo.Board.Helpers;
using CollapseShrinkDemo.Board.Handlers;
using DG.Tweening;

namespace CollapseShrinkDemo.Board
{
    [RequireComponent(typeof(BoardConditionHandler)), RequireComponent(typeof(BoardNeighbourHelper)), RequireComponent(typeof(BoardDeadlockHandler)), RequireComponent(typeof(BoardInputHandler))]
    public class Board : MonoBehaviour
    {
        public BoardConditionHandler BoardConditionHandler { get; private set; }
        public BoardNeighbourHelper BoardNeighbourHelper { get; private set; }
        public BoardDeadlockHandler BoardDeadlockHandler { get; private set; }

        #region General

        [Header("General"), Space(5)] [Range(2, 10)]
        public int width = 10;

        [Range(2, 10)] public int height = 10;

        [SerializeField, Range(2, 6)] private int availableColorCount;

        #endregion

        #region Game Piece Related

        [Header("Game Piece Related"), Space(5)] [SerializeField]
        private PoolerInfoWithTag gamePiecePoolerInfoWithTag;

        [SerializeField] private PooledObjInfoWithTag starVFXPooledObjInfo;
        [SerializeField, Space(10)] private int minPiecesToExplode = 2;

        [SerializeField, Space(10)] private float piecesMoveDuration = .5f;
        [SerializeField] private float timeBetweenFillPieces = .025f;
        [SerializeField, Space(10)] private float fillPieceSpawnYOffset = 10;
        private List<int> _selectedColors;

        private GamePiece[,] _gamePieces;

        private float _camTopPointY;

        #endregion

        #region Tile

        [Header("Tile"), Space(5)] [SerializeField]
        private GameObject tilePf;

        private Tile[,] _tiles;

        #endregion

        private void Awake()
        {
            BoardConditionHandler = GetComponent<BoardConditionHandler>();
            BoardDeadlockHandler = GetComponent<BoardDeadlockHandler>();
            BoardNeighbourHelper = GetComponent<BoardNeighbourHelper>();
        }

        private void Start()
        {
            SelectColors();

            CreateTiles();

            UIManager.Instance.onPanelShowed.AddListener(gameStatus =>
            {
                if (gameStatus is not GameStatus.Menu) return;
                
                gamePiecePoolerInfoWithTag.SetPooler();
                FillBoard();
            });
        }

        private void IterateInTiles(Action<int, int> tileIndex)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileIndex.Invoke(x, y);
                }
            }
        }

        private void CreateTiles()
        {
            _gamePieces = new GamePiece[width, height];
            _tiles = new Tile[width, height];

            IterateInTiles((x, y) =>
            {
                var tileObj = Instantiate(tilePf, new Vector2(x, y), Quaternion.identity, transform);
                var tile = tileObj.GetComponent<Tile>();
                tile.Init(x, y);
                _tiles[x, y] = tile;

                tile.name = $"Tile X:{x} Y:{y}";
            });

            var currentCinemachineTransform = CinemachineManager.CurrentCinemachineInfo.cinemachineVirtualCamera.transform;

            currentCinemachineTransform.position = new Vector3((float)width / 2 - .5f, (float)height / 2, currentCinemachineTransform.position.z);

            WaitUtilities.WaitForAFrame(() => _camTopPointY = CinemachineManager.MainCam.ScreenToWorldPoint(new Vector3(0, Screen.height, currentCinemachineTransform.position.z * -1)).y);
        }

        private void SelectColors()
        {
            var colorValues = Enumerable.Range(0, Enum.GetNames(typeof(MatchValue)).Length).ToArray();
            _selectedColors = colorValues.OrderBy(x => UnityEngine.Random.Range(0, colorValues.Length)).Take(availableColorCount).ToList();
        }

        private GamePiece MakeFillPiece(int x, int y)
        {
            var obj = gamePiecePoolerInfoWithTag.pooler.Get<GamePiece>();
            obj.transform.position = new Vector2(x, y + fillPieceSpawnYOffset);
            obj.name = $"GamePiece X:{x} Y:{y}";

            var selectedColor = UnityEngine.Random.Range(0, _selectedColors.Count);
            var gamePiece = obj.GetComponent<GamePiece>();
            gamePiece.Init(x, y, (MatchValue)selectedColor);
            _gamePieces[x, y] = gamePiece;
            return gamePiece;
        }

        public void CheckForExplodable(GamePiece clickedPiece)
        {
            if (_gamePieces[clickedPiece.X, clickedPiece.Y] == null) return;
            var explodableList = BoardNeighbourHelper.FindAllNeighboursRecursive(_gamePieces, clickedPiece);

            if (explodableList.Count < minPiecesToExplode) return;

            foreach (var piece in explodableList)
            {
                Explode(piece);
                StartCoroutine(WaitForFillBoard(CollapseColumn(piece.X)));
            }

            WaitAndCheckDeadlock();
        }

        private void CheckConditions() => BoardConditionHandler.CheckConditions(_gamePieces);

        private void WaitForPiecesMoveEnd(Action action) => DOVirtual.DelayedCall(piecesMoveDuration, () => WaitUtilities.WaitForAFrame(action));

        private void Explode(GamePiece piece)
        {
            _gamePieces[piece.X, piece.Y] = null;
            ParticleUtilities.PlayParticle(starVFXPooledObjInfo, new ParticleUtilities.ParticlePlayOptions(piece.transform));
            ClearPiece(piece);
        }

        private List<GamePiece> CollapseColumn(int column)
        {
            var movingPieces = new List<GamePiece>();

            for (int i = 0; i < height - 1; i++)
            {
                if (_gamePieces[column, i] is not null) continue;

                for (int j = i + 1; j < height; j++)
                {
                    if (_gamePieces[column, j] is null) continue;

                    _gamePieces[column, j].Move(column, i, piecesMoveDuration);
                    _gamePieces[column, i] = _gamePieces[column, j];
                    _gamePieces[column, i].Init(column, i);
                    if (!movingPieces.Contains(_gamePieces[column, i])) movingPieces.Add(_gamePieces[column, i]);
                    _gamePieces[column, j] = null;
                    break;
                }
            }

            return movingPieces;
        }

        private bool IsCollapsed(List<GamePiece> gamePieces) => gamePieces.Where(piece => piece is not null).All(piece => !piece.MovingTween.IsActiveNPlaying());

        private IEnumerator WaitForFillBoard(List<GamePiece> movingPieces)
        {
            if (movingPieces.Count == 0)
            {
                WaitForPiecesMoveEnd(() => FillBoard());
                yield break;
            }

            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            FillBoard();
        }

        private void FillBoard(bool cleanBoard = false)
        {
            if (cleanBoard) ClearBoard();

            var fillableTiles = new List<Tile>();

            IterateInTiles((x, y) =>
            {
                if (_gamePieces[x, y] is null) fillableTiles.Add(_tiles[x, y]);
            });

            var fillPieces = fillableTiles.Select(tile => MakeFillPiece(tile.x, tile.y)).ToList();
            foreach (var fillPiece in fillPieces)
            {
                fillPiece.gameObject.SetActive(false);
            }

            var columnGroups = fillPieces.GroupBy(x => x.X);

            foreach (var column in columnGroups)
            {
                DelayedFill(column.ToList());
            }
        }

        private void DelayedFill(List<GamePiece> gamePieces)
        {
            DOTweenUtilities.LoopWait(gamePieces, timeBetweenFillPieces,
                (gamePiece, i, _) =>
                {
                    gamePiece.gameObject.SetActive(true);

                    gamePiece.transform.position = new Vector2(gamePiece.X, _camTopPointY + i);
                    gamePiece.Move(gamePiece.X, gamePiece.Y, piecesMoveDuration);
                }, WaitAndCheckDeadlock);
        }

        private void ClearBoard()
        {
            foreach (var piece in _gamePieces)
            {
                if (piece is null) continue;
                ClearPiece(piece);
            }

            Array.Clear(_gamePieces, 0, _gamePieces.Length);
            SelectColors();
        }

        private void ClearPiece(GamePiece piece) => piece.Poolable.ReturnToPool();

        private void WaitAndCheckDeadlock()
        {
            CheckConditions();

            WaitForPiecesMoveEnd(() =>
            {
                if (!BoardDeadlockHandler.CheckForDeadlock(_gamePieces, minPiecesToExplode)) return;

                FillBoard(true);
            });
        }
    }
}