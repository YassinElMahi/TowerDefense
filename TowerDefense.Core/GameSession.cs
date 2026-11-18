namespace TowerDefense.Core;

public sealed class GameSession
{
    public const int TickMilliseconds = 350;

    private readonly GameMap _map;
    private readonly WaveDefinition _wave;
    private readonly List<Enemy> _enemies = new();
    private int _remainingToSpawn;
    private int _timeUntilSpawnMs;

    public GameSession(GameMap map, WaveDefinition wave)
    {
        _map = map;
        _wave = wave;
        _remainingToSpawn = wave.EnemyCount;
        _timeUntilSpawnMs = wave.SpawnIntervalMs;

        SpawnEnemy();
    }

    public int BaseHealth { get; private set; } = 10;
    public IEnumerable<MapCell> EnemyPositions => _enemies.Select(enemy => enemy.Position);
    public bool HasEnded => BaseHealth == 0
        || (_remainingToSpawn == 0 && _enemies.Count == 0);

    public void Update()
    {
        if (HasEnded)
        {
            return;
        }

        for (int index = _enemies.Count - 1; index >= 0; index--)
        {
            if (_enemies[index].Advance())
            {
                _enemies.RemoveAt(index);
                BaseHealth--;

                if (BaseHealth == 0)
                {
                    return;
                }
            }
        }

        if (_remainingToSpawn > 0)
        {
            _timeUntilSpawnMs -= TickMilliseconds;

            while (_remainingToSpawn > 0 && _timeUntilSpawnMs <= 0)
            {
                SpawnEnemy();
                _timeUntilSpawnMs += _wave.SpawnIntervalMs;
            }
        }
    }

    private void SpawnEnemy()
    {
        _enemies.Add(new Enemy(_map.Path));
        _remainingToSpawn--;
    }
}