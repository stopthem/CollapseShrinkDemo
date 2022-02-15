using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CollapseShrinkCore.Helpers;

namespace CollapseShrinkCore
{
    public class Board : MonoBehaviour
    {
        [HideInInspector] public BoardConditionChecker boardConditionChecker;
        [HideInInspector] public BoardNeighbourHelper boardNeighbourHelper;
        [HideInInspector] public BoardDeadlock boardDeadlock;

        [Range(2, 10)] public int width = 10, height = 10;
        [SerializeField, Range(2, 6)] private int availableColorCount;
        private List<int> _selectedColors;
        public Sprite[] gamePieceDefaultIcons;

        private GamePiece[,] _gamePieces;

        private Tile[,] _tiles;
        [SerializeField] private GameObject tilePf;

        [SerializeField, Space(10)] private int minPiecesToExplode = 2;
        [SerializeField, Space(10)] private float piecesMoveTime = .5f;
        [SerializeField] private float timeBetweenFillPieces = .025f;
        private float PiecesMoveSpeed { get => 1 / piecesMoveTime; }

        [SerializeField, Space(10)] private float fillPieceSpawnYOffset = 10;

        private bool _canPlay;

        private void Awake()
        {
            boardConditionChecker = GetComponent<BoardConditionChecker>();
            boardDeadlock = GetComponent<BoardDeadlock>();
            boardNeighbourHelper = GetComponent<BoardNeighbourHelper>();
        }

        private void Start()
        {
            SelectColors();

            _gamePieces = new GamePiece[width, height];
            _tiles = new Tile[width, height];

            CreateTiles();
            FillBoard();
            WaitAndCheckDeadlock();
        }

        private void CreateTiles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject tileObj = Instantiate(tilePf, new Vector2(x, y), Quaternion.identity, transform);
                    Tile tile = tileObj.GetComponent<Tile>();
                    tile.Init(x, y);
                    _tiles[x, y] = tile;
                    tile.name = "tile " + "x" + x + " y" + y;
                }
            }
        }

        private void SelectColors()
        {
            int[] colorValues = Enumerable.Range(0, Enum.GetNames(typeof(MatchValue)).Length).ToArray();
            _selectedColors = colorValues.OrderBy(x => UnityEngine.Random.Range(0, colorValues.Length)).Take(availableColorCount).ToList();
        }

        private GamePiece MakeFillPiece(int x, int y)
        {
            var obj = PoolerHandler.ReturnPooler("GamePiecePooler").GetObject();
            obj.transform.position = new Vector2(x, y + fillPieceSpawnYOffset);
            obj.name = "GamePiece " + "x" + x + " y" + y;

            int selectedColor = UnityEngine.Random.Range(0, _selectedColors.Count);
            GamePiece gamePiece = obj.GetComponent<GamePiece>();
            gamePiece.Init(x, y, gamePieceDefaultIcons[selectedColor], (MatchValue)selectedColor);
            _gamePieces[x, y] = gamePiece;
            return gamePiece;
        }

        private void Update()
        {
            CheckForInput();
        }

        private void CheckForInput()
        {
            if (Input.GetMouseButtonDown(0) && _canPlay)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider) CheckForExplodable(hit.collider.GetComponent<GamePiece>());
            }
        }

        private void CheckForExplodable(GamePiece clickedPiece)
        {
            if (_gamePieces[clickedPiece.x, clickedPiece.y] == null) return;
            var explodableList = boardNeighbourHelper.FindAllNeighboursRecursive(_gamePieces, clickedPiece);

            if (explodableList.Count >= minPiecesToExplode)
            {
                foreach (var piece in explodableList)
                {
                    Explode(piece);
                    StartCoroutine(WaitForFillBoard(CollapseColumn(piece.x)));
                }
                WaitAndCheckDeadlock();
            }
        }

        private void CheckConditions() => boardConditionChecker.CheckConditions(_gamePieces);

        private void WaitForPiecesMoveEnd(System.Action action) => LerpManager.Wait(piecesMoveTime, () => LerpManager.WaitForFrames(1, action));

        private void Explode(GamePiece piece)
        {
            _gamePieces[piece.x, piece.y] = null;
            var particle = PoolerHandler.ReturnPooler("StartVFXPooler").GetObject();
            GameManager.PlayParticle(particle, transform, piece.transform.position);
            ClearPiece(piece);
        }

        private List<GamePiece> CollapseColumn(int column)
        {
            List<GamePiece> movingPieces = new List<GamePiece>();

            for (int i = 0; i < height - 1; i++)
            {
                if (_gamePieces[column, i] == null)
                {
                    for (int j = i + 1; j < height; j++)
                    {
                        if (_gamePieces[column, j] != null)
                        {
                            _gamePieces[column, j].Move(column, i, PiecesMoveSpeed);
                            _gamePieces[column, i] = _gamePieces[column, j];
                            _gamePieces[column, i].Init(column, i);
                            if (!movingPieces.Contains(_gamePieces[column, i])) movingPieces.Add(_gamePieces[column, i]);
                            _gamePieces[column, j] = null;
                            break;
                        }
                    }
                }
            }
            return movingPieces;
        }

        private bool IsCollapsed(List<GamePiece> gamePieces)
        {
            foreach (GamePiece piece in gamePieces)
            {
                if (piece != null)
                {
                    if (Mathf.Abs(piece.transform.position.y - (float)piece.y) > 0.001f)
                    {
                        return false;
                    }
                    if (Mathf.Abs(piece.transform.position.x - (float)piece.x) > 0.001f)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #region fillboard
        private IEnumerator WaitForFillBoard(List<GamePiece> movingPieces)
        {
            if (movingPieces.Count == 0)
            {
                WaitForPiecesMoveEnd(() =>
                {
                    FillBoard();
                });
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

            List<Tile> fillableTiles = new List<Tile>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_gamePieces[x, y] == null) fillableTiles.Add(_tiles[x, y]);
                }
            }

            List<GamePiece> fillPieces = new List<GamePiece>();

            foreach (var tile in fillableTiles)
            {
                fillPieces.Add(MakeFillPiece(tile.x, tile.y));
            }

            var columnGroups = fillPieces.GroupBy(x => x.x);

            foreach (var column in columnGroups)
            {
                DelayedFill(column.Cast<GamePiece>().ToList());
            }
        }

        private void DelayedFill(List<GamePiece> gamePieces)
        {
            LerpManager.LoopWait(gamePieces.Count, timeBetweenFillPieces,
            x =>
            {
                GamePiece gamePiece = gamePieces[x];
                gamePiece.transform.position = new Vector2(gamePiece.x, gamePiece.y + fillPieceSpawnYOffset);
                gamePiece.Move(gamePiece.x, gamePiece.y, PiecesMoveSpeed);
            }, () =>
            {
                WaitAndCheckDeadlock();
            });
        }
        #endregion

        private void ClearBoard()
        {
            foreach (var piece in _gamePieces)
            {
                if (piece == null) continue;
                ClearPiece(piece);
            }
            Array.Clear(_gamePieces, 0, _gamePieces.Length);
            SelectColors();
        }

        private void ClearPiece(GamePiece piece) => piece.GetComponent<Poolable>().ClearMe();

        private void WaitAndCheckDeadlock()
        {
            WaitForPiecesMoveEnd(() =>
            {
                if (!boardDeadlock.CheckForDeadlock(_gamePieces, minPiecesToExplode))
                {
                    _canPlay = true;
                    CheckConditions();
                }
                else
                {
                    _canPlay = false;
                    ReactionText.CreateText("DEADLOCKED!", Color.white, ReactionText.Directions.TopToBottom, byPassPrevious: true);
                    FillBoard(true);
                }
            });
        }
    }
}