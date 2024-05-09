using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridNew : MonoBehaviour
{
    [SerializeField] private LevelData _levelData;
    
    [SerializeField] private Item[] _itemPrefabs;
    
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private float _fillTime;

    private Cell[,] _cells;

    public Cell[,] Cells => _cells;

    private int _x => _levelData.X;

    public int X => _x;
    private int _y => _levelData.Y;
    
    public int Y => _y;

    private Array2DBool _activeCells => _levelData.ActiveCells;
    private Array2DBool _spawnCells => _levelData.SpawnCells;
    private Array2DBool _obstaclesCells => _levelData.ObstaclesCells;
    private Array2DPortalEnum _portalCells => _levelData.PortalCells;

    private bool _inverse;

    private readonly Dictionary<ItemType, Item> _itemDictionary = new();

    private Cell _firstCell;
    private Cell _secondCell;

    private Cell _currentFirstCell;
    private Cell _currentSecondCell;

    private void Start()
    {
        foreach (var item in _itemPrefabs)
        {
            _itemDictionary.TryAdd(item.Type, item);
        }

        GenerateGrid();
        SpawnObstacles();
        StartCoroutine(Fill());
    }

    private void GenerateGrid()
    {
        _cells = new Cell[_x, _y];

        for (var x = 0; x < _x; x++)
        {
            for (var y = 0; y < _y; y++)
            {
                var position = GetWorldPosition(x, y);
                var newCell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                newCell.Init(x, y, this);
                newCell.SetActive(_activeCells.GetCell(x, y));
                newCell.SetPortal(_portalCells.GetCell(x, y));
                _cells[x, y] = newCell;
            }
        }
    }

    private void SpawnObstacles()
    {
        for (var x = 0; x < _x; x++)
        {
            for (var y = 0; y < _y; y++)
            {
                if (_obstaclesCells.GetCell(x, y))
                {
                    var cell = _cells[x, y];

                    cell.SetItem(SpawnItem(x, y, ItemType.OBSTACLE));
                }
            }
        }
    }

    private Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - _x / 2.0f + x, transform.position.y + _y / 2.0f - y);
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
        var movedItem = false;

        for (var y = _y - 2; y >= 0; y--)
        {
            for (var defaultX = 0; defaultX < _x; defaultX++)
            {
                var x = _inverse ? _x - 1 - defaultX : defaultX;

                var cell = _cells[x, y];

                if (!cell.IsActive)
                    continue;

                if (cell.IsEmpty)
                    continue;

                if (!cell.Item.IsMovable())
                    continue;

                var cellBelow = _cells[x, y + 1];

                if (cellBelow.IsActive)
                {
                    if (cellBelow.IsEmpty)
                    {
                        movedItem = MoveItem(cell, cellBelow);
                    }
                    else
                    {
                        movedItem = CheckDiagonal(cell, x, y);
                    }
                }
                else
                {
                    if (_portalCells.GetCell(x, y) == PortalType.BOTTOM || _portalCells.GetCell(x, y) == PortalType.BOTH)
                    {
                        for (var portalY = y + 1; portalY < _y; portalY++)
                        {
                            var portalCell = _cells[x, portalY];

                            if (_portalCells.GetCell(x, portalY) == PortalType.TOP || _portalCells.GetCell(x, portalY) == PortalType.BOTH)
                            {
                                if (portalCell.IsActive && portalCell.IsEmpty)
                                {
                                    movedItem = MoveItem(cell, portalCell);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        movedItem = CheckDiagonal(cell, x, y);
                    }
                }
            }
        }
        
        for (var x = 0; x < _x; x++)
        {
            for (var y = 0; y < _y; y++)
            {
                if (!_spawnCells.GetCell(x, y)) 
                    continue;
                
                var cell = _cells[x, y];

                if (!cell.IsEmpty) 
                    continue;

                var newItem = SpawnItem(x, y - 1);
                cell.SetItem(newItem);
                cell.Item.MovableComponent.Move(GetWorldPosition(x, y), _fillTime);
                movedItem = true;
            }
        }

        return movedItem;
    }
    
    private bool CheckDiagonal(Cell cell, int x, int y)
    {
        for (var diagonal = -1; diagonal <= 1; diagonal++)
        {
            if (diagonal == 0)
                continue;

            var diagonalX = _inverse ? x - diagonal : x + diagonal;

            if (diagonalX >= 0 && diagonalX < _x)
            {
                var diagonalCell = _cells[diagonalX, y + 1];

                for (var aboveY = y + 1; aboveY >= 0; aboveY--)
                {
                    var aboveDiagonalCell = _cells[diagonalX, aboveY];

                    if (!aboveDiagonalCell.IsActive || _obstaclesCells.GetCell(diagonalX, aboveY))
                    {
                        if (diagonalCell.IsActive && diagonalCell.IsEmpty)
                        {
                            return MoveItem(cell, diagonalCell);
                        }
                    }
                }
            }
        }
        
        return false;
    }

    private Item SpawnItem(int x, int y, ItemType type = ItemType.NORMAL)
    {
        var newItem = Instantiate(_itemDictionary[type], GetWorldPosition(x, y), Quaternion.identity);

        if (newItem.IsColored())
        {
            var rColor = Random.Range(0, newItem.ColoringComponent.Count);
            newItem.ColoringComponent.Coloring((ColorType)rColor);
        }
        
        return newItem;
    }
    
    private Item SpawnItem(int x, int y, ItemType type, ColorType colorType)
    {
        var newItem = Instantiate(_itemDictionary[type], GetWorldPosition(x, y), Quaternion.identity);

        if (newItem.IsColored())
        {
            newItem.ColoringComponent.Coloring(colorType);
        }
        
        return newItem;
    }

    private bool MoveItem(Cell fromCell, Cell inCell)
    {
        fromCell.Item.MovableComponent.Move(inCell.GetPosition(), _fillTime);
        inCell.SetItem(fromCell.Item);
        fromCell.Clear();
        return true;
    }
    
    public void PressCell(Cell piece)
    {
        _firstCell = piece;
    }
    
    public void PressCellClear()
    {
        _firstCell = null;
    }

    public void EnterCell(Cell piece)
    {
        _secondCell = piece;

        if (_firstCell != null && _secondCell != null)
        {
            if (_firstCell != _secondCell)
            {
                ReleaseItems();
            }
        }
    }

    private void ReleaseItems()
    {
        if (_firstCell.Item.IsColored() && _secondCell.Item.IsColored())
        {
            if (_firstCell.Item.ColoringComponent.Type != _secondCell.Item.ColoringComponent.Type)
            {
                StartCoroutine(SwapItems(_firstCell, _secondCell));
                return;
            }
        }

        _firstCell = null;
        _secondCell = null;
    }
    
    private bool IsAdjacent(Cell cell1, Cell cell2)
    {
        return (cell1.X == cell2.X && Mathf.Abs(cell1.Y - cell2.Y) == 1) ||
               (cell1.Y == cell2.Y && Mathf.Abs(cell1.X - cell2.X) == 1);
    }

    private IEnumerator SwapItems(Cell cell1, Cell cell2)
    {
        if (cell1.Item.IsMovable() && cell2.Item.IsMovable())
        {
            _currentFirstCell = _firstCell;
            _currentSecondCell = _secondCell;
            
            Swap(cell1, cell2);

            _firstCell = null;
            _secondCell = null;

            yield return new WaitForSeconds(_fillTime);
            
            if (GetMath(cell1, cell1.X, cell1.Y) == null && GetMath(cell2, cell2.X, cell2.Y) == null)
            {
                Swap(cell1, cell2);
            }
            else
            {
                ClearAllValidMatches();
                StartCoroutine(Fill());
            }
        }
    }

    private void Swap(Cell cell1, Cell cell2)
    {
        var item1 = cell1.Item;
        var item2 = cell2.Item;

        item1.MovableComponent.Move(cell2.GetPosition(), _fillTime);
        cell2.SetItem(item1);
        item2.MovableComponent.Move(cell1.GetPosition(), _fillTime);
        cell1.SetItem(item2);
    }

    private List<Cell> GetMath(Cell cell, int x, int y)
    {
        if (!cell.Item.IsColored())
            return null;

        var horizontalCells = new List<Cell>();
        var verticalCells = new List<Cell>();
        var matchingCells = new List<Cell>();
        
        // HORIZONTAL START

        horizontalCells = GetHorizontalMatch(cell, x, y);

        if (horizontalCells.Count >= 3)
        {
            matchingCells.AddRange(horizontalCells);

            foreach (var horizontalCell in horizontalCells)
            {
                verticalCells = GetVerticalMatch(horizontalCell, horizontalCell.X, horizontalCell.Y);
                verticalCells.RemoveAt(0);
                
                if (verticalCells.Count < 2)
                {
                    verticalCells.Clear();
                }
                else
                {
                    matchingCells.AddRange(verticalCells);
                }
            }
            
            return matchingCells;
        }
        
        // HORIZONTAL END
        
        horizontalCells.Clear();
        verticalCells.Clear();
        matchingCells.Clear();

        // VERTICAL START

        verticalCells = GetVerticalMatch(cell, x, y);

        if (verticalCells.Count >= 3)
        {
            matchingCells.AddRange(verticalCells);

            foreach (var verticalCell in verticalCells)
            {
                horizontalCells = GetHorizontalMatch(verticalCell, verticalCell.X, verticalCell.Y);
                horizontalCells.RemoveAt(0);
                
                if (horizontalCells.Count < 2)
                {
                    horizontalCells.Clear();
                }
                else
                {
                    matchingCells.AddRange(horizontalCells);
                }
            }
            
            return matchingCells;
        }
        
        // VERTICAL END

        return null;
    }
    
    private List<Cell> GetHorizontalMatch(Cell cell, int x, int y)
    {
        var horizontalCells = new List<Cell>();

        horizontalCells.Add(cell);
        
        for (var direction = -1; direction <= 1; direction++)
        {
            if (direction == 0)
                continue;
            
            var newX = x + direction;
            
            while (newX >= 0 && newX < _x)
            {
                var horizontalCell = _cells[newX, y];

                if (!horizontalCell.IsActive || horizontalCell.IsEmpty)
                    break;

                if (horizontalCell.Item.IsColored() &&
                    horizontalCell.Item.ColoringComponent.Type == cell.Item.ColoringComponent.Type) 
                {
                    horizontalCells.Add(horizontalCell);
                    newX += direction;
                }
                else
                {
                    break;
                }
            }
        }
        
        return horizontalCells;
    }

    private List<Cell> GetVerticalMatch(Cell cell, int x, int y)
    {
        var verticalCells = new List<Cell>();
        
        verticalCells.Add(cell);
        
        for (var direction = -1; direction <= 1; direction++)
        {
            if (direction == 0)
                continue;
            
            var newY = y + direction;

            while (newY >= 0 && newY < _y)
            {
                var verticalCell = _cells[x, newY];

                if (!verticalCell.IsActive || verticalCell.IsEmpty)
                    break;

                if (verticalCell.Item.IsColored() &&
                    verticalCell.Item.ColoringComponent.Type == cell.Item.ColoringComponent.Type)
                {
                    verticalCells.Add(verticalCell);
                    newY += direction;
                }
                else
                {
                    break;
                }
            }
        }

        return verticalCells;
    }

    private bool ClearAllValidMatches()
    {
        var needsRefill = false;

        for (var x = 0; x < _x; x++)
        {
            for (var y = 0; y < _y; y++)
            {
                var cell = _cells[x, y];

                if (cell.IsActive && !cell.IsEmpty)
                {
                    var match = GetMath(cell, x, y);

                    if (match != null)
                    {
                        var specialItemType = ItemType.NORMAL;
                        var randomCell = match[Random.Range(0, match.Count)];
                        var specialItemX = randomCell.X;
                        var specialItemY = randomCell.Y;
                        var specialColorType = match[0].Item.ColoringComponent.Type;

                        if (match.Count >= 4)
                        {
                            if (_currentFirstCell == null || _currentSecondCell == null)
                            {
                                var r = Random.Range(0, 2);
                                
                                specialItemType = r == 0 ? ItemType.ROW_CLEAR : ItemType.COLUM_CLEAR;
                            }
                            else if (_currentFirstCell.Y == _currentSecondCell.Y)
                            {
                                specialItemType = ItemType.ROW_CLEAR;
                            }
                            else
                            {
                                specialItemType = ItemType.COLUM_CLEAR;
                            }
                        }
                        
                        for (var i = 0; i < match.Count; i++)
                        {
                            if (_currentFirstCell == match[i] || _currentSecondCell == match[i])
                            {
                                specialItemX = match[i].X;
                                specialItemY = match[i].Y;
                            }
                            
                            match[i].ClearItem();
                            CheckObstacles(match[i].X, match[i].Y);
                        }

                        if (specialItemType != ItemType.NORMAL)
                        {
                            var newItem = SpawnItem(specialItemX, specialItemY, specialItemType, specialColorType);
                            _cells[specialItemX, specialItemY].SetItem(newItem);
                        }
                        
                        needsRefill = true;
                    }
                }
            }
        }

        _currentFirstCell = null;
        _currentSecondCell = null;

        return needsRefill;
    }

    private void CheckObstacles(int x, int y)
    {
        for (var adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX == x)
                continue;

            if (adjacentX >= 0 && adjacentX < _x)
            {
                var adjacentCell = _cells[adjacentX, y];

                if (adjacentCell.IsActive && !adjacentCell.IsEmpty)
                {
                    if (adjacentCell.ItemType == ItemType.OBSTACLE)
                    {
                        adjacentCell.ClearItem();
                    }
                }
            }
        }
        
        for (var adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY == y)
                continue;

            if (adjacentY >= 0 && adjacentY < _y)
            {
                var adjacentCell = _cells[x, adjacentY];

                if (adjacentCell.IsActive && !adjacentCell.IsEmpty)
                {
                    if (adjacentCell.ItemType == ItemType.OBSTACLE)
                    {
                        adjacentCell.ClearItem();
                    }
                }
            }
        }
    }
}
