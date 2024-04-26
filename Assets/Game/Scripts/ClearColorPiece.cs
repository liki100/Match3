public class ClearColorPiece : ClearablePiece
{
    private ColorType _color;

    public override void Clear()
    {
        base.Clear();

        _piece.Grid.ClearColor(_color);
    }

    public void SetColor(ColorType color)
    {
        _color = color;
    }
}
