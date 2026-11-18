namespace TowerDefense.Core;

public sealed class GameSession
{
    public GameSession(GameMap map)
    {
        ActiveEnemy = new Enemy(map.Path);
    }

    public int BaseHealth { get; private set; } = 10;
    public Enemy? ActiveEnemy { get; private set; }

    public void Update()
    {
        if (ActiveEnemy is not { } enemy)
        {
            return;
        }

        if (enemy.Advance())
        {
            BaseHealth--;
            ActiveEnemy = null;
        }
    }
}