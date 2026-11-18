using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using TowerDefense.Core;

namespace TowerDefense.App;

public partial class MainWindow : Window
{
    private readonly Dictionary<MapCell, TextBlock> _cellLabels = new();
    private readonly HashSet<MapCell> _displayedEnemies = new();

    public MainWindow()
    {
        InitializeComponent();

        var mapPanel = this.FindControl<StackPanel>("MapPanel")!;
        var healthText = this.FindControl<TextBlock>("BaseHealthText")!;

        GameMap map = GameMap.Load(Path.Combine(AppContext.BaseDirectory, "map.json"));
        WaveDefinition wave = WaveDefinition.Load(
            Path.Combine(AppContext.BaseDirectory, "waves.json"));

        var game = new GameSession(map, wave);

        for (int row = 0; row < map.Rows; row++)
        {
            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

            for (int column = 0; column < map.Columns; column++)
            {
                var position = new MapCell(row, column);
                rowPanel.Children.Add(CreateCell(map, position));
            }

            mapPanel.Children.Add(rowPanel);
        }

        UpdateDisplay(map, game, healthText);

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(GameSession.TickMilliseconds)
        };

        timer.Tick += (_, _) =>
        {
            game.Update();
            UpdateDisplay(map, game, healthText);

            if (game.HasEnded)
            {
                timer.Stop();
            }
        };

        Opened += (_, _) => timer.Start();
        Closed += (_, _) => timer.Stop();
    }

    private Border CreateCell(GameMap map, MapCell position)
    {
        IBrush background = map.IsBuildable(position) ? Brushes.DarkGreen : Brushes.DimGray;

        if (position == map.Spawn)
        {
            background = Brushes.RoyalBlue;
        }
        else if (position == map.Base)
        {
            background = Brushes.Firebrick;
        }

        var label = new TextBlock
        {
            Text = GetCellLabel(map, position),
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        _cellLabels.Add(position, label);

        return new Border
        {
            Width = 38,
            Height = 38,
            Margin = new Thickness(1),
            Background = background,
            Child = label
        };
    }

    private void UpdateDisplay(GameMap map, GameSession game, TextBlock healthText)
    {
        healthText.Text = $"PV de la base : {game.BaseHealth}";

        foreach (MapCell previous in _displayedEnemies)
        {
            TextBlock label = _cellLabels[previous];
            label.Text = GetCellLabel(map, previous);
            label.Foreground = Brushes.White;
        }

        _displayedEnemies.Clear();

        foreach (MapCell position in game.EnemyPositions)
        {
            TextBlock label = _cellLabels[position];
            label.Text = "●";
            label.Foreground = Brushes.Gold;
            _displayedEnemies.Add(position);
        }
    }

    private static string GetCellLabel(GameMap map, MapCell position)
    {
        if (position == map.Spawn)
        {
            return "D";
        }

        return position == map.Base ? "B" : "";
    }
}