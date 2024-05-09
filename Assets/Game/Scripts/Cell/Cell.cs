using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private CellView _view;
    public int X { get; private set; }
    public int Y { get; private set; }
    
    private bool _isActive;
    
    private GridNew _grid;
    public bool IsActive => _isActive;
    public bool IsEmpty => _item == null;
    
    private Item _item;

    public Item Item => _item;
    public ItemType ItemType => _item.Type;

    public void Init(int x, int y, GridNew grid)
    {
        X = x;
        Y = y;
        
        _grid = grid;
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        _view.SetActive(active);
    }

    public void SetPortal(PortalType type)
    {
        _view.SetSprite(type);
    }
    
    public void SetItem(Item item)
    {
        _item = item;
        _item.transform.parent = transform;
    }

    public void Clear()
    {
        _item = null;
    }

    public void ClearItem()
    {
        if (!IsActive || IsEmpty) 
            return;
        
        _item.Clear(_grid, X, Y);
        Clear();
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    private void OnMouseEnter()
    {
        if (!IsActive || IsEmpty)
            return;

        _grid.EnterCell(this);
    }
    
    private void OnMouseDown()
    {
        if (!IsActive || IsEmpty)
            return;
        
        _grid.PressCell(this);
    }

    private void OnMouseUp()
    {
        _grid.PressCellClear();
    }
}
