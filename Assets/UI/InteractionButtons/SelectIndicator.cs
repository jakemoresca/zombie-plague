using Godot;
using System;

public class SelectIndicator : Node2D
{
	// Declare member variables here. Examples:
	private Map _map;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_Map_finished_updating()
	{
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			var gridPosition = player.GetGridPosition();
			var position = new GridPosition { Row = gridPosition.Row, Column = gridPosition.Column };

			this.Position = GridHelper.GetTargetPosition(_map.Tilemap, position, (int)tileSize, initCoordinates);
		}
	}
}
