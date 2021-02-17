using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MovementManager
{
	private GameManager _root;
	private PlayerManager _playerManager;
	private const int MAXVALUE = 100;
	private const int UNPASSABLE = -1;
	private List<GridIndicator> _attackIndicators;
	private List<GridIndicator> _moveIndicators;

	public MovementManager(GameManager root, PlayerManager playerManager)
	{
		_root = root;
		_playerManager = playerManager;
		_attackIndicators = new List<GridIndicator>();
		_moveIndicators = new List<GridIndicator>();
	}

	public IEnumerable<GridPosition> GetAttackPositions(GridPosition position, string direction, int range, int playerNumber)
	{
		var column = position.Column;
		var row = position.Row;
		var (maxColumn, maxRow) = _root.Map.GetDimension();

		var possibleAttackPoints = GetPossibleAttackPoints(position, direction, range);

		return possibleAttackPoints.Where(attackPoint => GridHelper.HasEnemyUnit(_root, attackPoint.Column, attackPoint.Row, playerNumber));
	}

	private List<GridPosition> GetPossibleAttackPoints(GridPosition position, string direction, int range)
	{
		var gridPositions = new List<GridPosition>();

		switch (direction)
		{
			case "up":
				for (var y = 1; y <= range; y++)
				{
					var gridPosition = new GridPosition { Column = position.Column, Row = position.Row - y };
					gridPositions.Add(gridPosition);
				}
				break;

			case "down":
				for (var y = 1; y <= range; y++)
				{
					var gridPosition = new GridPosition { Column = position.Column, Row = position.Row + y };
					gridPositions.Add(gridPosition);
				}
				break;

			case "left":
				for (var x = 1; x <= range; x++)
				{
					var gridPosition = new GridPosition { Column = position.Column - x, Row = position.Row };
					gridPositions.Add(gridPosition);
				}
				break;

			case "right":
				for (var x = 1; x <= range; x++)
				{
					var gridPosition = new GridPosition { Column = position.Column + x, Row = position.Row };
					gridPositions.Add(gridPosition);
				}
				break;
		}

		return gridPositions;
	}

	public IEnumerable<PlayerMove> GetMovePositions(GridPosition position, string direction, int ap, int playerNumber, int range, int wide)
	{
		var column = position.Column;
		var row = position.Row;
		var (maxColumn, maxRow) = _root.Map.GetDimension();

		var edges = GetMapEdges(maxColumn, maxRow);

		var initialNeighbors = GetNeighborEdges(position);

		edges[position.ToEdgeString()].APWeight = 0;

		foreach (var neighbors in initialNeighbors)
		{
			SetScore(edges, neighbors, 0, position, direction, ap, playerNumber, range, wide);
		}

		var movePositions = edges.Where(x => x.Value.APWeight <= ap && x.Key != position.ToEdgeString()).Select(edge => edge.Value);

		return movePositions;
	}


	//ToDo: split functions
	private void SetScore(IDictionary<string, PlayerMove> edges, GridPosition targetPosition, int currentAP, GridPosition currentPosition,
		string currentDirection, int maxAP, int playerNumber, int range, int wide)
	{
		var targetEdge = targetPosition.ToEdgeString();
		var moveType = PlayerMoveType.Move;
		var targetDirection = GetTargetDirection(targetPosition, currentPosition);

		if (!GridHelper.CanMoveForward(_root.Map, currentPosition.Column, currentPosition.Row, targetDirection))
			return;

		if (GridHelper.HasPlayerUnits(_root, currentPosition.Column, currentPosition.Row, targetDirection))
			return;

		if (!edges.ContainsKey(targetEdge))
			return;

		if (edges[targetEdge].Type == PlayerMoveType.Attack)
			return;

		var computedScore = edges[targetEdge].APWeight;

		if (targetDirection == currentDirection)
		{
			computedScore = currentAP + 1 < computedScore ? currentAP + 1 : computedScore;
		}
		else if (IsSideDirection(currentDirection, targetDirection))
		{
			computedScore = currentAP + 2 < computedScore ? currentAP + 2 : computedScore;
		}
		else if (IsOppositeDirection(currentDirection, targetDirection))
		{
			computedScore = currentAP + 3 < computedScore ? currentAP + 3 : computedScore;
		}

		if (currentAP < maxAP)
		{
			var attackPositions = GetAttackPositions(currentPosition, currentDirection, range, playerNumber);

			foreach (var attackPosition in attackPositions)
			{
				GD.Print($"Got attack position to {attackPosition.Column}, {attackPosition.Row} from {currentPosition.Column}, {currentPosition.Row} with {range} range.");

				var attackPositionEdge = attackPosition.ToEdgeString();
				computedScore = currentAP + 1 < computedScore ? currentAP + 1 : computedScore;

				edges[attackPositionEdge].Type = PlayerMoveType.Attack;
				edges[attackPositionEdge].APWeight = computedScore;
				edges[attackPositionEdge].AttackerPosition = currentPosition;
			}
		}

		if (computedScore >= edges[targetEdge].APWeight)
			return;

		if (computedScore <= maxAP)
		{
			edges[targetEdge].APWeight = computedScore;
			edges[targetEdge].Type = moveType;

			if (moveType == PlayerMoveType.Attack)
				return;

			var neighbors = GetNeighborEdges(targetPosition);

			foreach (var neighbor in neighbors)
			{
				SetScore(edges, neighbor, computedScore, targetPosition, targetDirection, maxAP, playerNumber, range, wide);
			}
		}
	}

	private List<GridPosition> GetNeighborEdges(GridPosition currentPosition)
	{
		var x = currentPosition.Column;
		var y = currentPosition.Row;

		var upEdge = new GridPosition { Column = x, Row = y - 1 };
		var downEdge = new GridPosition { Column = x, Row = y + 1 };
		var leftEdge = new GridPosition { Column = x - 1, Row = y };
		var rightEdge = new GridPosition { Column = x + 1, Row = y };

		return new List<GridPosition> { upEdge, downEdge, leftEdge, rightEdge };
	}

	private string GetTargetDirection(GridPosition targetPosition, GridPosition currentPosition)
	{
		if (targetPosition.Column > currentPosition.Column)
		{
			return "right";
		}
		else if (targetPosition.Column < currentPosition.Column)
		{
			return "left";
		}
		else if (targetPosition.Row > currentPosition.Row)
		{
			return "down";
		}
		else if (targetPosition.Row < currentPosition.Row)
		{
			return "up";
		}

		return string.Empty;
	}

	private bool IsSideDirection(string direction, string directionToCompare)
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

	private bool IsOppositeDirection(string direction, string directionToCompare)
	{
		switch (direction)
		{
			case "up":
				return directionToCompare == "down";
			case "down":
				return directionToCompare == "up";
			case "left":
				return directionToCompare == "right";
			case "right":
				return directionToCompare == "left";
		}

		return false;
	}

	private IDictionary<string, PlayerMove> GetMapEdges(int maxColumn, int maxRow)
	{
		IDictionary<string, PlayerMove> edges = new Dictionary<string, PlayerMove>();

		for (int x = 1; x <= maxColumn; x++)
		{
			for (int y = 1; y <= maxRow; y++)
			{
				var gridPosition = new GridPosition { Column = x, Row = y };
				var playerMove = new PlayerMove { Position = gridPosition, APWeight = MAXVALUE };

				edges.Add(gridPosition.ToEdgeString(), playerMove);
			}
		}

		return edges;
	}

	public void ShowMovePoints(IEnumerable<PlayerMove> movePositions)
	{
		InstantiateMoveIndicators(movePositions.Where(x => x.Type == PlayerMoveType.Move).Select(x => x.Position).ToList());
		InstantiateAttackIndicators(movePositions.Where(x => x.Type == PlayerMoveType.Attack).Select(x => x.Position).ToList());
	}

	private void InstantiateAttackIndicators(List<GridPosition> attackPositions)
	{
		InstantiateIndicators(attackPositions, "res://Assets/UI/InteractionButtons/AttackIndicator.tscn", _attackIndicators);
	}

	private void InstantiateMoveIndicators(List<GridPosition> movePositions)
	{
		InstantiateIndicators(movePositions, "res://Assets/UI/InteractionButtons/MoveIndicator.tscn", _moveIndicators);
	}

	private void InstantiateIndicators(List<GridPosition> positions, string resource, List<GridIndicator> indicators)
	{
		var numberOfSpawnPoints = positions.Count;
		var numberOfCurrentCellIndicator = indicators.Count;
		var difference = numberOfCurrentCellIndicator - numberOfSpawnPoints;

		var reuseMax = difference < 0 ? numberOfCurrentCellIndicator : numberOfSpawnPoints;

		//Reuse Logic
		for (var x = 0; x < reuseMax; x++)
		{
			var gridCellIndicator = indicators[x];
			var gridPosition = positions[x];

			gridCellIndicator.UpdateGridPosition(gridPosition.Column, gridPosition.Row);
			gridCellIndicator.Show();
		}

		//Create new ones
		if (difference < 0)
		{
			for (var x = numberOfCurrentCellIndicator; x < numberOfSpawnPoints; x++)
			{
				var gridCellIndicatorScene = ResourceLoader.Load<PackedScene>(resource);

				if (gridCellIndicatorScene != null)
				{
					var gridCellIndicatorInstance = (GridIndicator)gridCellIndicatorScene.Instance();
					_root.Map.AddChild(gridCellIndicatorInstance);

					var gridPosition = positions[x];

					gridCellIndicatorInstance.UpdateGridPosition(gridPosition.Column, gridPosition.Row);
					gridCellIndicatorInstance.Show();

					indicators.Add(gridCellIndicatorInstance);
				}
			}
		}
		else if (difference > 0)
		{
			for (var x = difference - 1; x >= 0; x--)
			{
				indicators[indicators.Count - 1].Free();
				indicators.RemoveAt(indicators.Count - 1);
			}
		}
	}
}

public class PlayerMove
{
	public GridPosition AttackerPosition { get; set; }
	public string Direction { get; set; }
	public GridPosition Position { get; set; }
	public PlayerMoveType Type { get; set; }
	public int APWeight { get; set; }
}

public enum PlayerMoveType
{
	Move,
	Attack
}
