using Godot;
using System;

public class GridPosition
{
    public int Column { get; set; }
    public int Row { get; set; }
}

public static class GridHelper
{
    public static bool CanMoveForward(Map map, int column, int row, string direction)
    {
        var passable = false;

        var (numberOfColumns, numberOfRows) = map.GetDimension();

        if (direction == "up" && row <= 1)
        {
            return passable;
        }

        if (direction == "left" && column <= 1)
        {
            return passable;
        }

        if (direction == "right" && column >= numberOfColumns)
        {
            return passable;
        }

        if (direction == "down" && row >= numberOfRows)
        {
            return passable;
        }

        var collisionMaps = map.GetCollisionMaps();
        var collisionMapKey = $"col{column}row{row}";

        if(collisionMaps.Contains(collisionMapKey))
        {
            var collisionMapValue = (Godot.Collections.Dictionary)collisionMaps[collisionMapKey];

            if(collisionMapValue.Contains(direction))
            {
                return !bool.Parse(collisionMapValue[direction].ToString());
            }
        }

        return true;
    }
}