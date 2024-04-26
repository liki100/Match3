using UnityEngine;

public class ClearLinePiece : ClearablePiece
{
    [SerializeField] private bool _isRow;
    
    public override void Clear()
    {
        base.Clear();

        if (_isRow)
        {
            _piece.Grid.ClearRow(_piece.Y);
        }
        else
        {
            _piece.Grid.ClearColumn(_piece.X);
        }
    }
}
