using Godot;
using System;
using System.Collections.Generic;

public class GridPosition
{
    public int Column { get; set; }
    public int Row { get; set; }

    public string ToEdgeString()
    {
        return $"col{Column}row{Row}";
    }
}

public static class GridHelper
{
    public static bool CanMoveForward(Map map, int column, int row, string direction)
    {
        var passable = false;

        var (numberOfColumns, numberOfRows) = map.GetDimension();

        if (direction == "up" && row <= 0)
        {
            return passable;
        }

        if (direction == "left" && column <= 0)
        {
            return passable;
        }

        if (direction == "right" && column > numberOfColumns)
        {
            return passable;
        }

        if (direction == "down" && row > numberOfRows)
        {
            return passable;
        }

        return HasObstacle(map, column, row, direction);
    }

    private static bool HasObstacle(Map map, int column, int row, string direction)
    {
        var targetPosition = GetTargetPosition(column, row, direction);

        var collisionMapKey = $"col{targetPosition.Column}row{targetPosition.Row}";
        var passable = true;

        var collisionMaps = map.CollisionMaps;
        var windows = map.Windows;

        var collisionChecks = new List<Tuple<Godot.Collections.Dictionary, bool>>();

        collisionChecks.Add(new Tuple<Godot.Collections.Dictionary, bool>(collisionMaps, false));
        collisionChecks.Add(new Tuple<Godot.Collections.Dictionary, bool>(windows, true));

        foreach (var collisionCheck in collisionChecks)
        {
            if (collisionCheck.Item1.Contains(collisionMapKey))
            {
                var hasFound = false;
                passable = HasCollisionWithCollisionMaps(collisionCheck.Item1, collisionMapKey, out hasFound, collisionCheck.Item2);

                if (hasFound && collisionCheck.Item2 && !passable)
                    break;
            }
        }

        return passable;
    }

    private static bool HasCollisionWithCollisionMaps(Godot.Collections.Dictionary collisionMap, string collisionMapKey, out bool hasFound, bool canClose = false)
    {
        var passable = true;
        hasFound = false;

        if (collisionMap.Contains(collisionMapKey))
        {
            var collisionMapValue = (Godot.Collections.Dictionary)collisionMap[collisionMapKey];

            if (canClose == true)
            {
                if (collisionMapValue.Contains("IsClosed"))
                {
                    var isClosed = bool.Parse(collisionMapValue["IsClosed"].ToString());
                    passable = !isClosed;
					hasFound = true;

                    GD.Print($"Can Pass: {passable} {collisionMapKey}");
                }
                else
                {
                    GD.Print($"Can Pass {true} {collisionMapKey}");

                    passable = true;
                    hasFound = true;
                }
            }
            else
            {
                passable = false;
				hasFound = true;

                GD.Print($"Can Pass: {passable} {collisionMapKey}");
            }
        }

        return passable;
    }

    public static bool HasPlayerUnits(GameManager _root, int column, int row, string direction = "any")
    {
        var targetPosition = GetTargetPosition(column, row, direction);

        return _root.HasPlayerUnits(targetPosition.Column, targetPosition.Row);
    }

    public static bool HasEnemyUnit(GameManager _root, int column, int row, int playerNumber, string direction = "any")
    {
        var targetPosition = GetTargetPosition(column, row, direction);

        return _root.HasEnemyUnit(targetPosition.Column, targetPosition.Row, playerNumber, out _);
    }

    public static bool HasSearchable(Map map, int column, int row, string direction, int playerNumber)
    {
		var targetPosition = GetTargetPosition(column, row, direction);
        var searchableKey = $"col{targetPosition.Column}row{targetPosition.Row}";

        var searchables = map.Searchables;
        var hasSearchable = !HasCollisionWithCollisionMaps(searchables, searchableKey, out _);

        if (hasSearchable)
        {
            var searchable = (Godot.Collections.Dictionary)searchables[searchableKey];
            var hasSearchBy = searchable.Contains($"searchByPlayer{playerNumber}");

            return hasSearchable && !hasSearchBy;
        }

        return hasSearchable;
    }

    public static bool CanSetBarricade(Map map, int column, int row, string direction, int playerNumber)
    {
        if (playerNumber == (int)PlayerNumber.Zombie)
            return false;

		var targetPosition = GetTargetPosition(column, row, direction);
        var collisionMapKey = $"col{targetPosition.Column}row{targetPosition.Row}";
        var windows = map.Windows;

        var collisionChecks = new List<Tuple<Godot.Collections.Dictionary, bool>>();
        collisionChecks.Add(new Tuple<Godot.Collections.Dictionary, bool>(windows, true));

        foreach (var collisionCheck in collisionChecks)
        {
            if (collisionCheck.Item1.Contains(collisionMapKey))
            {
                var hasBarricade = HasCollisionWithCollisionMaps(collisionCheck.Item1, collisionMapKey, out _, collisionCheck.Item2);

                if (hasBarricade)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsSideDirection(string direction, string directionToCompare)
    {
        if (direction == "up" || direction == "down")
        {
            return directionToCompare == "left" || directionToCompare == "right";
        }
        else if (direction == "left" || direction == "right")
        {
            return directionToCompare == "up" || directionToCompare == "down";
        }

        return false;
    }

    public static bool IsOppositeDirection(string direction, string directionToCompare)
    {
        return directionToCompare == GetOppositeDirection(direction);
    }

    public static string GetOppositeDirection(string direction)
    {
        switch (direction)
        {
            case "up":
                return "down";
            case "down":
                return "up";
            case "left":
                return "right";
            case "right":
                return "left";
        }

        return string.Empty;
    }

    public static bool HasFourPileFriendlies(GameManager gameManager, GridPosition position, string doorDirection, int playerNumber)
    {
        var score = 1;

        var fourPilePositions = GetFourPilePositions(position, doorDirection);

        foreach (var fourPilePosition in fourPilePositions)
        {
            if (gameManager.HasFriendlyUnit(fourPilePosition.Column, fourPilePosition.Row, playerNumber))
                score++;

            if (score >= 4)
                break;
        }

        return score >= 4;
    }

    private static List<GridPosition> GetFourPilePositions(GridPosition position, string doorDirection)
    {
        var column = position.Column;
        var row = position.Row;
        var fourPilePositions = new List<GridPosition>();

        if (doorDirection == "right")
        {
            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 1 });
            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row - 1 });

            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 1 });
            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row + 1 });

            fourPilePositions.Add(new GridPosition { Column = column - 2, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column - 3, Row = row });

            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 2 });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 2 });
        }
        else if (doorDirection == "left")
        {
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 1 });
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row - 1 });

            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 1 });
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row + 1 });

            fourPilePositions.Add(new GridPosition { Column = column + 2, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column + 3, Row = row });

            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 2 });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 2 });
        }
        else if (doorDirection == "up")
        {
            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 1 });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 2 });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row - 3 });

            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row });

            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row - 1 });
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row - 1 });

            fourPilePositions.Add(new GridPosition { Column = column - 2, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column + 2, Row = row });
        }
        else if (doorDirection == "down")
        {
            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 1 });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 2 });
            fourPilePositions.Add(new GridPosition { Column = column, Row = row + 3 });

            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row });

            fourPilePositions.Add(new GridPosition { Column = column - 1, Row = row + 1 });
            fourPilePositions.Add(new GridPosition { Column = column + 1, Row = row + 1 });

            fourPilePositions.Add(new GridPosition { Column = column - 2, Row = row });
            fourPilePositions.Add(new GridPosition { Column = column + 2, Row = row });
        }

        return fourPilePositions;
    }

    public static GridPosition GetTargetPosition(int column, int row, string direction, int range = 1)
    {
        switch (direction)
        {
            case "up":
                row -= range;
                break;

            case "left":
                column -= range;
                break;

            case "right":
                column += range;
                break;

            case "down":
                row += range;
                break;
        }

        return new GridPosition { Column = column, Row = row };
    }

    public static Vector2 GetTargetPosition(TileMap tilemap, GridPosition position, int tileSize, (int, int) initCoordinates)
    {
        var (initX, initY) = initCoordinates;

        var vector2Position = new Vector2(position.Column, position.Row);
        var targetPosition = tilemap.MapToWorld(vector2Position, true);

        var targetX = (targetPosition.x + (tileSize / 2)) * 1.5f;
        var targetY = (targetPosition.y + (tileSize / 2)) * 1.5f;

        return new Vector2(targetX, targetY);
    }
}