using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Board : Singleton<Board>
{
    private BoardConditionChecker _boardConditionChecker;

    [SerializeField, Range(2, 10)] private int width = 10, height = 10;
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
        _boardConditionChecker = GetComponent<BoardConditionChecker>();
    }

    private void Start()
    {
        SelectColors();

        _gamePieces = new GamePiece[width, height];
        _tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObj = Instantiate(tilePf, new Vector2(x, y), Quaternion.identity, transform);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;
                _tiles[x, y] = tile;
                tile.name = "tile " + "x" + x + " y" + y;
            }
        }
        FillBoard();
        WaitAndCheckDeadlock();
    }

    private void SelectColors()
    {
        int[] colorValues = Enumerable.Range(0, gamePieceDefaultIcons.Length).ToArray();
        _selectedColors = colorValues.OrderBy(x => UnityEngine.Random.Range(0, colorValues.Length)).Take(availableColorCount).ToList();
    }

    // private void MakePiece(int x, int y)
    // {
    //     var piece = PoolerHandler.ReturnPooler("GamePiecePooler").GetObject();
    //     piece.transform.position = new Vector2(x, y);
    //     piece.name = "GamePiece " + "x" + x + " y" + y;

    //     int selectedColor = UnityEngine.Random.Range(0, _selectedColors.Count);
    //     GamePiece gamePiece = piece.GetComponent<GamePiece>();
    //     gamePiece.Init(x, y, gamePieceDefaultIcons[selectedColor], (MatchValue)selectedColor);
    //     _gamePieces[x, y] = gamePiece;
    // }

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
        if (_canPlay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider) CheckForExplodable(hit.collider.GetComponent<GamePiece>());
            }
        }
    }

    private void CheckForExplodable(GamePiece clickedPiece)
    {
        if (_gamePieces[clickedPiece.x, clickedPiece.y] == null) return;

        var explodableList = FindAllNeighboursRecursive(clickedPiece);

        if (explodableList.Count >= minPiecesToExplode)
        {
            foreach (var piece in explodableList)
            {
                Explode(piece);
            }

            foreach (var piece in explodableList)
            {
                StartCoroutine(WaitForFillBoard(CollapseColumn(piece.x)));
            }
            WaitAndCheckDeadlock();
        }
    }

    private void CheckConditions() => _boardConditionChecker.CheckConditions(_gamePieces.Cast<GamePiece>().ToList());

    private void WaitForPiecesMoveEnd(System.Action action)
    {
        LerpManager.Wait(piecesMoveTime, () =>
             {
                 LerpManager.WaitForFrames(1, action);
             });
    }

    private void Explode(GamePiece piece)
    {
        _gamePieces[piece.x, piece.y] = null;
        var particle = PoolerHandler.ReturnPooler("StartVFXPooler").GetObject();
        GameManager.PlayParticle(particle, transform, piece.transform.position);
        piece.GetComponent<Poolable>().ClearMe();
        piece.transform.position = new Vector2(piece.x, piece.y + 10);
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
                        _gamePieces[column, i].x = column;
                        _gamePieces[column, i].y = i;
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

    public bool IsWithinBounds(int x, int y) => (x < width && x >= 0) && (y >= 0 && y < height);

    #region  neighbour finding
    public List<GamePiece> FindAllNeighboursRecursive(GamePiece gamePiece, List<GamePiece> result = null)
    {
        List<GamePiece> clickedNeighbours = FindNeighbours(gamePiece.x, gamePiece.y);

        if (clickedNeighbours == null) return new List<GamePiece>();

        if (result == null)
        {
            result = new List<GamePiece>();
        }
        if (!result.Contains(gamePiece)) result.Add(gamePiece);

        var newPieces = new List<GamePiece>();
        foreach (var piece in clickedNeighbours)
        {
            if (!result.Contains(piece))
            {
                result.Add(piece);
                newPieces.Add(piece);
            }
        }

        foreach (var piece in newPieces)
        {
            FindAllNeighboursRecursive(piece, result);
        }

        return result;
    }

    private List<GamePiece> FindNeighbours(int x, int y)
    {
        List<GamePiece> neighbours = new List<GamePiece>();
        if (_gamePieces[x, y] == null) return new List<GamePiece>();
        if (IsWithinBounds(x - 1, y) && _gamePieces[x - 1, y] != null)
        {
            if (_gamePieces[x, y].matchValue == _gamePieces[x - 1, y].matchValue) neighbours.Add(_gamePieces[x - 1, y]);
        }

        if (IsWithinBounds(x + 1, y) && _gamePieces[x + 1, y] != null)
        {
            if (_gamePieces[x, y].matchValue == _gamePieces[x + 1, y].matchValue) neighbours.Add(_gamePieces[x + 1, y]);
        }

        if (IsWithinBounds(x, y - 1) && _gamePieces[x, y - 1] != null)
        {
            if (_gamePieces[x, y].matchValue == _gamePieces[x, y - 1].matchValue) neighbours.Add(_gamePieces[x, y - 1]);
        }

        if (IsWithinBounds(x, y + 1) && _gamePieces[x, y + 1] != null)
        {
            if (_gamePieces[x, y].matchValue == _gamePieces[x, y + 1].matchValue) neighbours.Add(_gamePieces[x, y + 1]);
        }
        return neighbours;
    }

    #endregion

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
        if (cleanBoard)
        {
            foreach (var item in _gamePieces)
            {
                if (item == null) continue;
                item.GetComponent<Poolable>().ClearMe();
                item.transform.position = new Vector2(item.x, item.y + 10);
            }
            Array.Clear(_gamePieces, 0, _gamePieces.Length);
            SelectColors();
        }

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
            gamePiece.Move(gamePiece.x, gamePiece.y, PiecesMoveSpeed);
        }, () =>
        {
            WaitAndCheckDeadlock();
        });
    }
    #endregion

    #region deadlock
    private void WaitAndCheckDeadlock()
    {
        WaitForPiecesMoveEnd(() =>
        {
            if (!CheckForDeadlock())
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

    private bool CheckForDeadlock()
    {
        foreach (var piece in _gamePieces)
        {
            if (piece == null) continue;
            var neighbours = FindAllNeighboursRecursive(piece);
            if (neighbours.Count >= minPiecesToExplode) return false;
        }
        return true;
    }
    #endregion
}