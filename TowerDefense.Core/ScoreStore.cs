namespace TowerDefense.Core;
using System.Text.Json;

public sealed class ScoreStore
{
    private readonly string _filePath;

    public ScoreStore(string filePath)
{
    _filePath = Path.GetFullPath(filePath);
}
    public IReadOnlyList<int> Load()
{
    if (!File.Exists(_filePath))
    {
        return Array.Empty<int>();
    }

    string json = File.ReadAllText(_filePath);
    int[] scores = JsonSerializer.Deserialize<int[]>(json) ?? Array.Empty<int>();
    return Rank(scores);  

    
}
    public void Save(int score)
    {
        var scores = Load().Append(score);
        int[] bestScores = Rank(scores);
        string json = JsonSerializer.Serialize(bestScores);
        File.WriteAllText(_filePath, json);
    }

    private static int[] Rank(IEnumerable<int> scores)
    {
        return scores
            .OrderByDescending(score => score)
            .Take(10)
            .ToArray();
    }
}