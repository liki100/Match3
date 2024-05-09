public class ColumClearItem : Item
{
    public override void Clear(GridNew grid, int x, int y)
    {
        if (_isBeingCleaned)
            return;
        
        base.Clear(grid, x, y);
    
        for (var newY = 0; newY < grid.Y; newY++)
        {
            if (newY == y)
                continue;

            var cell = grid.Cells[x, newY];

            if (!cell.IsActive || cell.IsEmpty)
                continue;

            cell.ClearItem();
        }
    }
}
