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

		return HasObstacle(map, column, row, direction);
	}

	public static Vector2 GetTargetPosition(TileMap tilemap, GridPosition position, int tileSize, (int, int) initCoordinates)
	{
		var (initX, initY) = initCoordinates;

		var vector2Position = new Vector2(position.Column + initX, position.Row + initY);
		var targetPosition = tilemap.MapToWorld(vector2Position, true);

		var targetX = (targetPosition.x - (tileSize / 2)) * 1.5f;
		var targetY = (targetPosition.y - (tileSize / 2)) * 1.5f;

		return new Vector2(targetX, targetY);
	}

	private static bool HasObstacle(Map map, int column, int row, string direction)
	{
		var collisionMapKey = $"col{column}row{row}";
		var passable = true;

		var collisionMaps = map.CollisionMaps;
		var doors = map.Doors;
		var windows = map.Windows;

		var collisionChecks = new List<Tuple<Godot.Collections.Dictionary, bool>>();

		collisionChecks.Add(new Tuple<Godot.Collections.Dictionary, bool>(collisionMaps, false));
		collisionChecks.Add(new Tuple<Godot.Collections.Dictionary, bool>(doors, true));
		collisionChecks.Add(new Tuple<Godot.Collections.Dictionary, bool>(windows, true));

		foreach(var collisionCheck in collisionChecks)
		{
			if (collisionCheck.Item1.Contains(collisionMapKey))
			{
				passable = HasCollisionWithCollisionMaps(collisionCheck.Item1, collisionMapKey, direction, collisionCheck.Item2);
			}
		}

		return passable;
	}

	private static bool HasCollisionWithCollisionMaps(Godot.Collections.Dictionary collisionMap, string collisionMapKey,
		string direction, bool canClose = false)
	{
		var passable = true;

		if (collisionMap.Contains(collisionMapKey))
		{
			var collisionMapValue = (Godot.Collections.Dictionary)collisionMap[collisionMapKey];

			if (collisionMapValue.Contains(direction))
			{
				if (canClose == true)
				{
					if (collisionMapValue.Contains("IsClosed"))
					{
						var isClosed = bool.Parse(collisionMapValue["IsClosed"].ToString());
						passable = !isClosed && !bool.Parse(collisionMapValue[direction].ToString());

						GD.Print($"Can Pass: {passable} {collisionMapKey}");
					}
					else
					{
						GD.Print($"Can Pass {collisionMapKey}");
						passable = true;
					}
				}
				else
				{
					passable = !bool.Parse(collisionMapValue[direction].ToString());

					GD.Print($"Can Pass: {passable} {collisionMapKey}");
				}
			}
		}

		return passable;
	}

	public static bool HasPlayerUnits(GameManager _root, int column, int row, string direction = "any")
	{
		var tempColumn = column;
		var tempRow = row;

		if (direction == "up")
		{
			tempRow -= 1;
		}
		else if (direction == "left")
		{
			tempColumn -= 1;
		}
		else if (direction == "right")
		{
			tempColumn += 1;
		}
		else if (direction == "down")
		{
			tempRow += 1;
		}

		return _root.HasPlayerUnits(tempColumn, tempRow);
	}

	public static bool HasEnemyUnit(GameManager _root, int column, int row, int playerNumber, string direction = "any")
	{
		var tempColumn = column;
		var tempRow = row;

		if (direction == "up")
		{
			tempRow -= 1;
		}
		else if (direction == "left")
		{
			tempColumn -= 1;
		}
		else if (direction == "right")
		{
			tempColumn += 1;
		}
		else if (direction == "down")
		{
			tempRow += 1;
		}

		return _root.HasEnemyUnit(tempColumn, tempRow, playerNumber);
	}

	public static bool HasSearchable(Map map, int column, int row, string direction, int playerNumber)
	{
		var searchableKey = $"col{column}row{row}";
		
		var searchables = map.Searchables;
		var hasSearchable = !HasCollisionWithCollisionMaps(searchables, searchableKey, direction);

		if(hasSearchable)
		{
			var searchable = (Godot.Collections.Dictionary)searchables[searchableKey];
			var hasSearchBy = searchable.Contains($"searchByPlayer{playerNumber}");

			return hasSearchable && !hasSearchBy;
		}

		return hasSearchable;
	}
}