using System.Text.Json;

namespace TowerDefense.Core;

public sealed record WaveDefinition(int EnemyCount, int SpawnIntervalMs)
{
    public static WaveDefinition Load(string filePath)
    {
        using var file = File.OpenRead(filePath);
        var wave = JsonSerializer.Deserialize<WaveDefinition>(
            file,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (wave is null || wave.EnemyCount < 1 || wave.SpawnIntervalMs < 1)
        {
            throw new InvalidDataException(
                "La vague doit contenir au moins un ennemi et un intervalle positif.");
        }

        return wave;
    }
}