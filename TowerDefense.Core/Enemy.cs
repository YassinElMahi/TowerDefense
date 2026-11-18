namespace TowerDefense.Core;

public sealed class Enemy
{
    private readonly IReadOnlyList<MapCell> _path;
    private int _pathIndex;

    internal Enemy(IReadOnlyList<MapCell> path)
    {
        _path = path;
    }

    public MapCell Position => _path[_pathIndex];

    internal bool Advance()
    {
        _pathIndex++;
        return _pathIndex == _path.Count - 1;
    }
}
