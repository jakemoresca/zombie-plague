using Godot;
using System;
using System.Collections.Generic;

public class SpawnPointManager
{
    private GameManager _root;
    private List<GridCellIndicator> _gridCellIndicator;
    private string _lastZombieDirection = "";

    public SpawnPointManager(GameManager root)
    {
        _root = root;
        _gridCellIndicator = new List<GridCellIndicator>();
    }

    public void Hide()
    {
        foreach(var gridCell in _gridCellIndicator)
        {
            gridCell.Hide();
        }
    }

    public void Show()
    {
        foreach(var gridCell in _gridCellIndicator)
        {
            gridCell.Show();
        }
    }

    public void CreateSpawnPoints(string direction, int playerNumber)
    {
        var (maxColumns, maxRows) = _root.Map.GetDimension();
        List<Tuple<int, int>> spawnCellPosition;

        if(playerNumber == (int)PlayerNumber.Zombie)
            _lastZombieDirection = direction;

        if (direction != "Wild")
        {
            spawnCellPosition = CreateSpawnCellPositions(direction, maxColumns, maxRows);
        }
        else
        {
            spawnCellPosition = new List<Tuple<int, int>>();

            spawnCellPosition.AddRange(CreateSpawnCellPositions("North", maxColumns, maxRows));
            spawnCellPosition.AddRange(CreateSpawnCellPositions("East", maxColumns, maxRows));
            spawnCellPosition.AddRange(CreateSpawnCellPositions("West", maxColumns, maxRows));
            spawnCellPosition.AddRange(CreateSpawnCellPositions("South", maxColumns, maxRows));
        }

        InstantiateSpawnPoints(spawnCellPosition);
    }

    private List<Tuple<int, int>> CreateSpawnCellPositions(string direction, int maxColumns, int maxRows)
    {
        var spawnCellPosition = new List<Tuple<int, int>>();

        if (direction == "North")
        {
            for (var x = 0; x < maxColumns; x++)
            {
                spawnCellPosition.Add(new Tuple<int, int>(x, 0));
            }
        }
        else if (direction == "South")
        {
            for (var x = 0; x < maxColumns; x++)
            {
                spawnCellPosition.Add(new Tuple<int, int>(x, maxRows - 1));
            }
        }
        else if (direction == "East")
        {
            for (var y = 0; y < maxRows; y++)
            {
                spawnCellPosition.Add(new Tuple<int, int>(maxColumns - 1, y));
            }
        }
        else if (direction == "West")
        {
            for (var y = 0; y < maxRows; y++)
            {
                spawnCellPosition.Add(new Tuple<int, int>(0, y));
            }
        }

        return spawnCellPosition;
    }

    private void InstantiateSpawnPoints(List<Tuple<int, int>> spawnPoints)
    {
        var numberOfSpawnPoints = spawnPoints.Count;
        var numberOfCurrentCellIndicator = _gridCellIndicator.Count;
        var difference = numberOfCurrentCellIndicator - numberOfSpawnPoints;

        var reuseMax = difference < 0 ? numberOfCurrentCellIndicator : numberOfSpawnPoints;

        //Reuse Logic
        for (var x = 0; x < reuseMax; x++)
        {
            var gridCellIndicator = _gridCellIndicator[x];
            var gridPosition = spawnPoints[x];

            gridCellIndicator.UpdateGridPosition(gridPosition.Item1, gridPosition.Item2);
            gridCellIndicator.Show();
        }

        //Create new ones
        if (difference < 0)
        {
            for (var x = numberOfCurrentCellIndicator; x < numberOfSpawnPoints; x++)
            {
                var gridCellIndicatorScene = ResourceLoader.Load<PackedScene>("res://Assets/UI/InteractionButtons/GridCellIndicator.tscn");

                if (gridCellIndicatorScene != null)
                {
                    var gridCellIndicatorInstance = (GridCellIndicator)gridCellIndicatorScene.Instance();
                    _root.Map.AddChild(gridCellIndicatorInstance);

                    var gridPosition = spawnPoints[x];

                    gridCellIndicatorInstance.UpdateGridPosition(gridPosition.Item1, gridPosition.Item2);
                    gridCellIndicatorInstance.Show();

                    _gridCellIndicator.Add(gridCellIndicatorInstance);
                }
            }
        }
        else if(difference > 0)
        {
            for(var x = difference - 1; x >= 0; x--)
            {
                _gridCellIndicator[_gridCellIndicator.Count - 1].Free();
                _gridCellIndicator.RemoveAt(_gridCellIndicator.Count - 1);
            }
        }
    }

    public void SetLastZombieDirection(string direction)
    {
        _lastZombieDirection = direction;
    }

    public string GetLastZombieDirection()
    {
        return _lastZombieDirection;
    }

    public string GetOppositeDirection(string direction)
    {
        switch(direction)
        {
            case "North":
                return "South";
            case "East":
                return "West";
            case "West":
                return "East";
            case "South":
                return "North";
        }

        return string.Empty;
    }
}
