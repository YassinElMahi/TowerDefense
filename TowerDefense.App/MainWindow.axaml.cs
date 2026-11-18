using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using TowerDefense.Core;

namespace TowerDefense.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var mapPanel = this.FindControl<StackPanel>("MapPanel")!;
        string filePath = Path.Combine(AppContext.BaseDirectory, "map.json");
        GameMap map = GameMap.Load(filePath);

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
    }

    private static Border CreateCell(GameMap map, MapCell position)
    {
        IBrush background = map.IsBuildable(position)
            ? Brushes.DarkGreen
            : Brushes.DimGray;

        string label = "";

        if (position == map.Spawn)
        {
            background = Brushes.RoyalBlue;
            label = "D";
        }
        else if (position == map.Base)
        {
            background = Brushes.Firebrick;
            label = "B";
        }

        return new Border
        {
            Width = 38,
            Height = 38,
            Margin = new Thickness(1),
            Background = background,
            Child = new TextBlock
            {
                Text = label,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }
}