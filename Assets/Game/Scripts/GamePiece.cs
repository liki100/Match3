using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int _x;
    private int _y;
    
    private GridCreator _grid;
    private PieceType _type;
    private MovablePiece _movableComponent;
    private ColorPiece _colorComponent;
    private ClearablePiece _clearableComponent;

    public int X => _x;
    public int Y => _y;
    public GridCreator Grid => _grid;
    public PieceType Type => _type;
    public MovablePiece MovableComponent => _movableComponent;
    public ColorPiece ColorComponent => _colorComponent;
    public ClearablePiece ClearableComponent => _clearableComponent;

    private void Awake()
    {
        _movableComponent = GetComponent<MovablePiece>();
        _colorComponent = GetComponent<ColorPiece>();
        _clearableComponent = GetComponent<ClearablePiece>();
    }
    
    private void OnMouseEnter()
    {
        _grid.EnterPiece(this);
    }

    private void OnMouseDown()
    {
        _grid.PressPiece(this);
    }

    private void OnMouseUp()
    {
        _grid.ReleasePiece();
    }

    public void Init(int x, int y, GridCreator grid, PieceType type)
    {
        _x = x;
        _y = y;
        _grid = grid;
        _type = type;
    }

    public void SetPos(int x, int y)
    {
        if (!IsMovable())
            return;

        _x = x;
        _y = y;
    }

    public bool IsMovable()
    {
        return _movableComponent != null;
    }

    public bool IsColored()
    {
        return _colorComponent != null;
    }
    
    public bool IsClearable()
    {
        return _clearableComponent != null;
    }
}
