using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int _xDim;
    [SerializeField] private int _yDim;
    [SerializeField] private float _fillTime;
    [SerializeField] private PiecePrefab[] _piecePrefabs;
    [SerializeField] private GameObject _backgroundPrefab;

    private Dictionary<PieceType, GameObject> _piecePrefabDictionary;
    private GamePiece[,] _pieces;
    private bool _inverse;

    private GamePiece _pressedPiece;
    private GamePiece _enteredPiece;

    private void Start()
    {
        _piecePrefabDictionary = new Dictionary<PieceType, GameObject>();

        for (var i = 0; i < _piecePrefabs.Length; i++)
        {
            _piecePrefabDictionary.TryAdd(_piecePrefabs[i].Type, _piecePrefabs[i].Prefab);
        }

        for (var x = 0; x < _xDim; x++)
        {
            for (var y = 0; y < _yDim; y++)
            {
                var background = Instantiate(_backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        _pieces = new GamePiece[_xDim, _yDim];
        
        for (var x = 0; x < _xDim; x++)
        {
            for (var y = 0; y < _yDim; y++)
            {
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }
        
        Destroy(_pieces[1,4].gameObject);
        SpawnNewPiece(1, 4, PieceType.BUBBLE);
        
        Destroy(_pieces[2,4].gameObject);
        SpawnNewPiece(2, 4, PieceType.BUBBLE);
        
        Destroy(_pieces[3,4].gameObject);
        SpawnNewPiece(3, 4, PieceType.BUBBLE);
        
        Destroy(_pieces[5,4].gameObject);
        SpawnNewPiece(5, 4, PieceType.BUBBLE);
        
        Destroy(_pieces[6,4].gameObject);
        SpawnNewPiece(6, 4, PieceType.BUBBLE);
        
        Destroy(_pieces[7,4].gameObject);
        SpawnNewPiece(7, 4, PieceType.BUBBLE);
        
        Destroy(_pieces[4,0].gameObject);
        SpawnNewPiece(4, 0, PieceType.BUBBLE);

        StartCoroutine(Fill());
    }
    
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - _xDim / 2.0f + x, transform.position.y + _yDim / 2.0f - y);
    }

    private IEnumerator Fill()
    {
        var needRefill = true;
        while (needRefill)
        {
            yield return new WaitForSeconds(_fillTime);
            
            while (FillStep())
            {
                _inverse = !_inverse;
                yield return new WaitForSeconds(_fillTime);
            }
            
            needRefill = ClearAllValidMatches();
        }
        
    }

    private bool FillStep()
    {
        var movedPiece = false;

        for (var y = _yDim - 2; y >= 0; y--)
        {
            for (var loopX = 0; loopX < _xDim; loopX++)
            {
                var x = loopX;

                if (_inverse)
                {
                    x = _xDim - 1 - loopX;
                }
                
                var piece = _pieces[x, y];

                if (piece.IsMovable())
                {
                    var pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, _fillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (var diag = -1; diag <= 1; diag++)
                        {
                            if (diag == 0) 
                                continue;

                            var diagX = _inverse ? x - diag : x + diag;

                            if (diagX >= 0 && diagX < _xDim)
                            {
                                var diagonalPiece = _pieces[diagX, y + 1];

                                if (diagonalPiece.Type == PieceType.EMPTY)
                                {
                                    var hasPieceAbove = true;

                                    for (var aboveY = y; aboveY >= 0; aboveY--)
                                    {
                                        var pieceAbove = _pieces[diagX, aboveY];

                                        if (pieceAbove.IsMovable())
                                        {
                                            break;
                                        }

                                        if(pieceAbove.Type == PieceType.BUBBLE)
                                        {
                                            hasPieceAbove = false;
                                            break;
                                        }
                                    }

                                    if (!hasPieceAbove)
                                    {
                                        Destroy(diagonalPiece.gameObject);
                                        piece.MovableComponent.Move(diagX, y + 1, _fillTime);
                                        _pieces[diagX, y + 1] = piece;
                                        SpawnNewPiece(x, y, PieceType.EMPTY);
                                        movedPiece = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (var x = 0; x < _xDim; x++)
        {
            var pieceBelow = _pieces[x, 0];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                var newPiece = Instantiate(_piecePrefabDictionary[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                _pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                _pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                _pieces[x, 0].MovableComponent.Move(x,0, _fillTime);
                _pieces[x, 0].ColorComponent.SetColor((ColorType)Random.Range(0, _pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    private GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        var newPiece = Instantiate(_piecePrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        _pieces[x, y] = newPiece.GetComponent<GamePiece>();
        _pieces[x, y].Init(x, y, this, type);

        return _pieces[x, y];
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
               (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            _pieces[piece1.X, piece1.Y] = piece2;
            _pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null ||
                piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW) 
            {
                var piece1X = piece1.X;
                var piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, _fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, _fillTime);

                if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColored())
                {
                    var clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.SetColor(piece2.ColorComponent.Color);
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }
                
                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColored())
                {
                    var clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.SetColor(piece1.ColorComponent.Color);
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();

                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUM_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }
                
                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUM_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }
                

                _pressedPiece = null;
                _enteredPiece = null;
                
                StartCoroutine(Fill());
            }
            else
            {
                _pieces[piece1.X, piece1.Y] = piece1;
                _pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public void PressPiece(GamePiece piece)
    {
        _pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        _enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(_pressedPiece, _enteredPiece))
        {
            SwapPieces(_pressedPiece, _enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColored())
        {
            var color = piece.ColorComponent.Color;
            var horizontalPieces = new List<GamePiece>();
            var verticalPieces = new List<GamePiece>();
            var matchingPieces = new List<GamePiece>();
            
            horizontalPieces.Add(piece);

            for (var dir = 0; dir <= 1; dir++)
            {
                for (var xOffset = 1; xOffset < _xDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= _xDim)
                    {
                        break;
                    }

                    if (_pieces[x, newY].IsColored() && _pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(_pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                foreach (var horizontalPiece in horizontalPieces)
                {
                    matchingPieces.Add(horizontalPiece);
                }

                return matchingPieces;
            }
            
            verticalPieces.Add(piece);

            for (var dir = 0; dir <= 1; dir++)
            {
                for (var yOffset = 1; yOffset < _yDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    {
                        y = newY - yOffset;
                    }
                    else
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= _yDim)
                    {
                        break;
                    }

                    if (_pieces[newX, y].IsColored() && _pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(_pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                foreach (var verticalPiece in verticalPieces)
                {
                    matchingPieces.Add(verticalPiece);
                }

                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        var needsRefill = false;

        for (var y = 0; y < _yDim; y++)
        {
            for (var x = 0; x < _xDim; x++)
            {
                if (_pieces[x, y].IsClearable())
                {
                    var match = GetMatch(_pieces[x, y], x, y);

                    if (match != null)
                    {
                        var specialPieceType = PieceType.COUNT;
                        var randomPiece = match[Random.Range(0, match.Count)];
                        var specialPieceX = randomPiece.X;
                        var specialPieceY = randomPiece.Y;

                        if (match.Count == 4)
                        {
                            if (_pressedPiece == null || _enteredPiece == null)
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.COLUM_CLEAR,
                                    (int)PieceType.ROW_CLEAR);
                            }
                            else if (_pressedPiece.Y == _enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else
                            {
                                specialPieceType = PieceType.COLUM_CLEAR;
                            }
                        }
                        else if (match.Count >= 5)
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }

                        for (var i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;
                                if (match[i] == _pressedPiece || match[i] == _enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }

                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(_pieces[specialPieceX, specialPieceY]);
                            var newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);
                            if (specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUM_CLEAR && newPiece .IsColored() && match[0].IsColored())
                            {
                                newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }
                            else if (specialPieceType == PieceType.RAINBOW && newPiece.IsColored())
                            {
                                newPiece.ColorComponent.SetColor(ColorType.ANY);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (_pieces[x, y].IsClearable() && !_pieces[x, y].ClearableComponent.IsBeingCleaned)
        {
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            ClearObstacles(x, y);
            
            return true;
        }

        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        for (var adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < _xDim)
            {
                if (_pieces[adjacentX, y].Type == PieceType.BUBBLE && _pieces[adjacentX, y].IsClearable())
                {
                    _pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        for (var adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < _yDim)
            {
                if (_pieces[x, adjacentY].Type == PieceType.BUBBLE && _pieces[x, adjacentY].IsClearable())
                {
                    _pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int row)
    {
        for (var x = 0; x < _xDim; x++)
        {
            ClearPiece(x, row);
        }
    }
    
    public void ClearColumn(int column)
    {
        for (var y = 0; y < _yDim; y++)
        {
            ClearPiece(column, y);
        }
    }

    public void ClearColor(ColorType color)
    {
        for (var x = 0; x < _xDim; x++)
        {
            for (var y = 0; y < _yDim; y++)
            {
                if (_pieces[x, y].IsColored() && _pieces[x, y].ColorComponent.Color == color || color == ColorType.ANY)
                {
                    ClearPiece(x, y);
                }
            }
        }
    }
}
