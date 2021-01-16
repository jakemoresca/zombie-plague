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
        passable = true;

        if(collisionMaps.Contains(collisionMapKey))
        {
            var collisionMapValue = (Godot.Collections.Dictionary)collisionMaps[collisionMapKey];

            if(collisionMapValue.Contains(direction))
            {
                passable = !bool.Parse(collisionMapValue[direction].ToString());
            }
        }

        var doors = map.GetDoors();

        if(doors.Contains(collisionMapKey))
        {
            var collisionMapValue = (Godot.Collections.Dictionary)doors[collisionMapKey];

            if(collisionMapValue.Contains(direction))
            {
                if(collisionMapValue.Contains("IsClosed"))
                {
                    var isClosed = bool.Parse(collisionMapValue["IsClosed"].ToString());
                    passable = !isClosed && !bool.Parse(collisionMapValue[direction].ToString());
                }
                else
                {
                    passable = true;
                }
            }
        }

        var windows = map.GetWindows();

        if(windows.Contains(collisionMapKey))
        {
            var collisionMapValue = (Godot.Collections.Dictionary)windows[collisionMapKey];

            if(collisionMapValue.Contains(direction))
            {
                if(collisionMapValue.Contains("IsClosed"))
                {
                    var isClosed = bool.Parse(collisionMapValue["IsClosed"].ToString());
                    passable = !isClosed && !bool.Parse(collisionMapValue[direction].ToString());
                }
                else
                {
                    passable = true;
                }
            }
        }

        return passable;
    }
}