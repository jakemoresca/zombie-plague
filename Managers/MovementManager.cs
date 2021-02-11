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
    private List<AttackIndicator> _attackIndicators;

    public MovementManager(GameManager root, PlayerManager playerManager)
    {
        _root = root;
        _playerManager = playerManager;
        _attackIndicators = new List<AttackIndicator>();
    }

    public void GetAttackPositions(GridPosition position, string direction, int ap, int range, int wide)
    {
        GetMovePositions(position, direction, ap - 1);
    }

    public List<string> GetMovePositions(GridPosition position, string direction, int ap)
    {
        var column = position.Column;
        var row = position.Row;
        var (maxColumn, maxRow) = _root.Map.GetDimension();

        var edges = GetMapEdges(maxColumn, maxRow);

        var upEdgeKey = $"col{position.Column}row{position.Row}_up";
        var downEdgeKey = $"col{position.Column}row{position.Row}_down";
        var leftEdgeKey = $"col{position.Column}row{position.Row}_left";
        var rightEdgeKey = $"col{position.Column}row{position.Row}_right";

        SetScore(edges, upEdgeKey, 0, direction, ap);
        SetScore(edges, downEdgeKey, 0, direction, ap);
        SetScore(edges, leftEdgeKey, 0, direction, ap);
        SetScore(edges, rightEdgeKey, 0, direction, ap);

        return edges.Where(x => x.Value <= ap)
            .Select(x => x.Key)
            .ToList();
    }

    private void SetScore(IDictionary<string, int> edges, string currentEdge, int currentAP, string currentDirection, int maxAP)
    {
        if(!edges.ContainsKey(currentEdge))
            return;

        var edgeDirection = GetDirection(currentEdge);
        var computedScore = edges[currentEdge];

        if (edgeDirection == currentDirection)
        {
            computedScore = currentAP + 1 < computedScore ? currentAP + 1 : computedScore;
        }
        else if (IsSideDirection(currentDirection, edgeDirection))
        {
            computedScore = currentAP + 2 < computedScore ? currentAP + 1 : computedScore;
        }
        else if (IsOppositeDirection(currentDirection, edgeDirection))
        {
            computedScore = currentAP + 3 < computedScore ? currentAP + 1 : computedScore;
        }

        if(computedScore >= edges[currentEdge])
            return;

        if (computedScore < maxAP)
        {
            edges[currentEdge] = computedScore;

            var neighbors = GetNeighborEdges(currentEdge);

            foreach (var neighbor in neighbors)
            {
                SetScore(edges, neighbor, computedScore, edgeDirection, maxAP);
            }
        }
    }

    private List<string> GetNeighborEdges(string currentEdge)
    {
        var edgeDirection = GetDirection(currentEdge);
        var parsedKey = currentEdge.Split("_")[0].Replace("col", "").Replace("row", "-");  //From col12row10_down to 12-10

        var x = int.Parse(parsedKey.Split("-")[0]);
        var y = int.Parse(parsedKey.Split("-")[1]);

        switch (edgeDirection)
        {
            case "up":
                y -= 1;
                break;

            case "down":
                y += 1;
                break;
                
            case "left":
                x -= 1;
                break;
                
            case "right":
                x += 1;
                break;
                
        }

        var upEdge = $"col{x}row{y}_up";
        var downEdge = $"col{x}row{y}_down";
        var leftEdge = $"col{x}row{y}_left";
        var rightEdge = $"col{x}row{y}_right";

        return new List<string> { upEdge, downEdge, leftEdge, rightEdge };
    }

    private string GetDirection(string edgeKey)
    {
        return edgeKey.Split("_")[1];
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

    private IDictionary<string, int> GetMapEdges(int maxColumn, int maxRow)
    {
        IDictionary<string, int> edges = new Dictionary<string, int>();

        for (int x = 1; x <= maxColumn; x++)
        {
            for (int y = 1; y <= maxRow; y++)
            {
                var upEdge = new KeyValuePair<string, int>($"col{x}row{y}_up", MAXVALUE);
                var downEdge = new KeyValuePair<string, int>($"col{x}row{y}_down", MAXVALUE);
                var leftEdge = new KeyValuePair<string, int>($"col{x}row{y}_left", MAXVALUE);
                var rightEdge = new KeyValuePair<string, int>($"col{x}row{y}_right", MAXVALUE);

                edges.Add(upEdge);
                edges.Add(downEdge);
                edges.Add(leftEdge);
                edges.Add(rightEdge);
            }
        }

        return edges;
    }

    public void ShowMovePoints(List<string> edges)
    {
        var movePositions = edges.Select(edge =>
        {
            var parsedKey = edge.Split("_")[0].Replace("col", "").Replace("row", "-");

            var x = int.Parse(parsedKey.Split("-")[0]);
            var y = int.Parse(parsedKey.Split("-")[1]);

            return new Tuple<int, int>(x, y);
        });

        InstantiateAttackIndicators(movePositions.ToList());
    }

    private void InstantiateAttackIndicators(List<Tuple<int, int>> movePositions)
    {
        var numberOfSpawnPoints = movePositions.Count;
        var numberOfCurrentCellIndicator = _attackIndicators.Count;
        var difference = numberOfCurrentCellIndicator - numberOfSpawnPoints;

        var reuseMax = difference < 0 ? numberOfCurrentCellIndicator : numberOfSpawnPoints;

        //Reuse Logic
        for (var x = 0; x < reuseMax; x++)
        {
            var gridCellIndicator = _attackIndicators[x];
            var gridPosition = movePositions[x];

            gridCellIndicator.UpdateGridPosition(gridPosition.Item1, gridPosition.Item2);
            gridCellIndicator.Show();
        }

        //Create new ones
        if (difference < 0)
        {
            for (var x = numberOfCurrentCellIndicator; x < numberOfSpawnPoints; x++)
            {
                var gridCellIndicatorScene = ResourceLoader.Load<PackedScene>("res://Assets/UI/InteractionButtons/AttackIndicator.tscn");

                if (gridCellIndicatorScene != null)
                {
                    var gridCellIndicatorInstance = (AttackIndicator)gridCellIndicatorScene.Instance();
                    _root.Map.AddChild(gridCellIndicatorInstance);

                    var gridPosition = movePositions[x];

                    gridCellIndicatorInstance.UpdateGridPosition(gridPosition.Item1, gridPosition.Item2);
                    gridCellIndicatorInstance.Show();

                    _attackIndicators.Add(gridCellIndicatorInstance);
                }
            }
        }
        else if(difference > 0)
        {
            for(var x = difference - 1; x >= 0; x--)
            {
                _attackIndicators[_attackIndicators.Count - 1].Free();
                _attackIndicators.RemoveAt(_attackIndicators.Count - 1);
            }
        }
    }
}

public class AttackPosition
{
    public GridPosition Target { get; set; }
    public string Direction { get; set; }
    public GridPosition Position { get; set; }
}
