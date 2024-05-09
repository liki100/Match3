using Array2DEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelData", order = 41)]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;
    [SerializeField] private Array2DBool _activeCells;
    [SerializeField] private Array2DBool _spawnCells;
    [SerializeField] private Array2DBool _obstaclesCells;
    [SerializeField] private Array2DPortalEnum _portalCells;

    public int X => _x;
    public int Y => _y;
    public Array2DBool ActiveCells => _activeCells;
    public Array2DBool SpawnCells => _spawnCells;
    public Array2DBool ObstaclesCells => _obstaclesCells;
    public Array2DPortalEnum PortalCells => _portalCells;

    private void OnValidate()
    {
        _activeCells.GridSize = new Vector2Int(_x, _y);
        _spawnCells.GridSize = new Vector2Int(_x, _y);
        _obstaclesCells.GridSize = new Vector2Int(_x, _y);
        _portalCells.GridSize = new Vector2Int(_x, _y);
    }
}
