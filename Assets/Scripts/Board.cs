using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : Singleton<Board>
{
    private BoardConditionChecker _boardConditionChecker;

    [SerializeField] private int width = 10, height = 10;
    public Sprite[] gamePieceDefaultIcons;

    private GamePiece[,] _gamePieces;
    private int[,] _tiles;

    private List<GamePiece> _explodingPieces = new List<GamePiece>();
    [SerializeField, Space(10)] private int minExplosionCount = 2;
    public List<int[,]> fillableTiles = new List<int[,]>();
    [SerializeField, Space(5)] private float piecesMoveTime = .5f;
    private float PiecesMoveSpeed { get => 1 / piecesMoveTime; }

    private void Awake()
    {
        _boardConditionChecker = GetComponent<BoardConditionChecker>();
    }

    private void Start()
    {
        _gamePieces = new GamePiece[width, height];
        _tiles = new int[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = PoolerHandler.ReturnPooler("GamePiecePooler").GetObject();
                tile.transform.position = new Vector2(x, y);
                tile.name = "x" + x + " y" + y;

                int selectedColor = Random.Range(0, gamePieceDefaultIcons.Length);
                GamePiece gamePiece = tile.GetComponent<GamePiece>();
                gamePiece.Init(x, y, gamePieceDefaultIcons[selectedColor], (MatchValue)selectedColor);
                _gamePieces[x, y] = gamePiece;
            }
        }
        _boardConditionChecker.CheckConditions(_gamePieces.Cast<GamePiece>().ToList());
    }

    private void Update()
    {
        if (GameManager.gameStatus == GameManager.GameStatus.PLAY)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider) CheckForExplodable(hit.collider.GetComponent<GamePiece>());
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in GetColumunOrRawPieces(7))
            {
                Explode(item);
            }
        }
    }

    private void CheckForExplodable(GamePiece clickedPiece)
    {
        if (_gamePieces[clickedPiece.x, clickedPiece.y] == null || _explodingPieces.FirstOrDefault(x => x.y == clickedPiece.y)) return;
        var explodableList = FindAllNeighboursRecursive(clickedPiece);
        explodableList.RemoveAll(x => _explodingPieces.Contains(x));
        if (explodableList.Count >= minExplosionCount)
        {
            _explodingPieces.AddRange(explodableList);

            foreach (var piece in explodableList)
            {
                Explode(piece);
            }

            foreach (var piece in explodableList)
            {
                CollapseColumn(piece.x);
            }
            LerpManager.Wait(piecesMoveTime, () =>
             {
                 LerpManager.WaitForFrames(1, () =>
                  {
                      FillCheck();
                      _boardConditionChecker.CheckConditions(_gamePieces.Cast<GamePiece>().ToList());
                  });
             });
        }
    }

    private void Explode(GamePiece item)
    {
        var particle = PoolerHandler.ReturnPooler("StartVFXPooler").GetObject();
        GameManager.PlayParticle(particle, transform, item.transform.position);
        item.GetComponent<Poolable>().ClearMe();
        _gamePieces[item.x, item.y] = null;
    }

    private void CollapseColumn(int column)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        if (GetColumunOrRawPieces(column).Count == 0)
        {
            _explodingPieces.RemoveAll(x => x.x == column);
        }

        for (int i = 0; i < height - 1; i++)
        {
            if (_gamePieces[column, i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (_gamePieces[column, j] != null)
                    {
                        _gamePieces[column, j].Move(column, i, PiecesMoveSpeed);
                        if (!movingPieces.Contains(_gamePieces[column, i])) movingPieces.Add(_gamePieces[column, i]);
                        _gamePieces[column, j] = null;
                        break;
                    }
                    else
                    {
                        _explodingPieces.Remove(_explodingPieces.FirstOrDefault(x => x.x == column && x.y == i));
                    }
                }
            }
        }
    }

    public void PlaceGamePieceAt(int x, int y, GamePiece piece)
    {
        _gamePieces[x, y] = piece;
        _explodingPieces.Remove(_explodingPieces.FirstOrDefault(x => x.x == piece.x && x.y == piece.y));
        piece.name = "x" + x + " y" + y;
    }

    private List<GamePiece> GetColumunOrRawPieces(int column, int rowIndex = 0, bool row = false)
    {
        List<GamePiece> pieces = new List<GamePiece>();
        if (row)
            pieces = _gamePieces.Cast<GamePiece>().Where(x => x != null && x.y == rowIndex).ToList();
        else
            pieces = _gamePieces.Cast<GamePiece>().Where(x => x != null && x.x == column).ToList();

        return pieces;
    }

    public List<GamePiece> FindAllNeighboursRecursive(GamePiece gamePiece, List<GamePiece> result = null)
    {
        List<GamePiece> clickedNeighbours = FindNeighbours(gamePiece.x, gamePiece.y, true);

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

    private List<GamePiece> FindNeighbours(int x, int y, bool correctMatchValue = false)
    {
        List<GamePiece> neighbours = new List<GamePiece>();
        if (IsWithinBounds(x - 1, y) && _gamePieces[x - 1, y] != null)
        {
            if (correctMatchValue && _gamePieces[x, y].matchValue == _gamePieces[x - 1, y].matchValue) neighbours.Add(_gamePieces[x - 1, y]);
            else if (!correctMatchValue) neighbours.Add(_gamePieces[x - 1, y]);
        }

        if (IsWithinBounds(x + 1, y) && _gamePieces[x + 1, y] != null)
        {
            if (correctMatchValue && _gamePieces[x, y].matchValue == _gamePieces[x + 1, y].matchValue) neighbours.Add(_gamePieces[x + 1, y]);
            else if (!correctMatchValue) neighbours.Add(_gamePieces[x + 1, y]);
        }

        if (IsWithinBounds(x, y - 1) && _gamePieces[x, y - 1] != null)
        {
            if (correctMatchValue && _gamePieces[x, y].matchValue == _gamePieces[x, y - 1].matchValue) neighbours.Add(_gamePieces[x, y - 1]);
            else if (!correctMatchValue) neighbours.Add(_gamePieces[x, y - 1]);
        }

        if (IsWithinBounds(x, y + 1) && _gamePieces[x, y + 1] != null)
        {
            if (correctMatchValue && _gamePieces[x, y].matchValue == _gamePieces[x, y + 1].matchValue) neighbours.Add(_gamePieces[x, y + 1]);
            else if (!correctMatchValue) neighbours.Add(_gamePieces[x, y + 1]);
        }
        return neighbours;
    }

    public bool IsWithinBounds(int x, int y) => (x < width && x >= 0) && (y >= 0 && y < height);

    private void FillCheck()
    {
        foreach (var item in _gamePieces)
        {
            if (item == null) fillableTiles.Add(new int[,] { { item.x, item.y } });
        }
        print(fillableTiles.Count);
    }
}
