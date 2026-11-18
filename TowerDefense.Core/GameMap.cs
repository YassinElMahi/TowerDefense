using System.Text.Json;

namespace TowerDefense.Core;

public readonly record struct MapCell(int Row, int Column);

public sealed class GameMap
{
    private GameMap(int rows, int columns, MapCell[] path)
    {
        Rows = rows;
        Columns = columns;
        Path = Array.AsReadOnly(path);
    }

    public int Rows { get; }
    public int Columns { get; }
    public IReadOnlyList<MapCell> Path { get; }

    public MapCell Spawn => Path[0];
    public MapCell Base => Path[^1];

    public bool IsBuildable(MapCell cell)
    {
        return Contains(cell, Rows, Columns) && !Path.Contains(cell);
    }

    public static GameMap Load(string filePath)
    {
        using var file = File.OpenRead(filePath);
        var data = JsonSerializer.Deserialize<MapData>(
            file,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data is null || data.Rows < 1 || data.Columns < 1 ||
            data.Path is not { Length: >= 2 } path)
        {
            throw new InvalidDataException("La carte doit avoir des dimensions et un chemin valide.");
        }

        var visited = new HashSet<MapCell>();

        for (int index = 0; index < path.Length; index++)
        {
            MapCell cell = path[index];

            if (!Contains(cell, data.Rows, data.Columns) || !visited.Add(cell))
            {
                throw new InvalidDataException("Le chemin contient une case hors carte ou répétée.");
            }

            if (index > 0)
            {
                MapCell previous = path[index - 1];
                int distance = Math.Abs(cell.Row - previous.Row)
                    + Math.Abs(cell.Column - previous.Column);

                if (distance != 1)
                {
                    throw new InvalidDataException("Deux cases successives du chemin doivent se toucher.");
                }
            }
        }

        return new GameMap(data.Rows, data.Columns, path);
    }

    private static bool Contains(MapCell cell, int rows, int columns)
    {
        return cell.Row >= 0 && cell.Row < rows
            && cell.Column >= 0 && cell.Column < columns;
    }

    private sealed class MapData
    {
        public MapData() { }

        public int Rows { get; init; }
        public int Columns { get; init; }
        public MapCell[]? Path { get; init; }
    }
}