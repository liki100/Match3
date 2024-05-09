public class RowClearItem : Item
{
    public override void Clear(GridNew grid, int x, int y)
    {
        if (_isBeingCleaned)
            return;
        
        base.Clear(grid, x, y);
    
        for (var newX = 0; newX < grid.X; newX++)
        {
            if (newX == x)
                continue;

            var cell = grid.Cells[newX, y];

            if (!cell.IsActive || cell.IsEmpty)
                continue;

            cell.ClearItem();
        }
    }
}
