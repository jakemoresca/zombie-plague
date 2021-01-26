using Godot;
using System;

public class MousePointer : Area2D
{
	private Map _map;
	private Sprite _cursor;
	private Sprite _subCursor;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../MainMap");
		_cursor = this.GetNode<Sprite>("./Cursor");
		_subCursor = this.GetNode<Sprite>("./Cursor/SubCursor");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	public override void _Process(float delta)
	{
		this.Position = this.GetGlobalMousePosition();
	}

	public void SetSubCursor(Texture texture, float scale = 1f)
	{
		_subCursor.Texture = texture;
		_subCursor.Scale = new Vector2(scale, scale);
	}

	private void _on_Map_finished_updating()
	{
		/*
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			var gridPosition = player.GetGridPosition();
			var position = new GridPosition { Row = gridPosition.Row, Column = gridPosition.Column };

			initCoordinates.Item2 += 28;

			this.Position = GridHelper.GetTargetPosition(position, tileSize, initCoordinates);
		}
		*/
	}
}
