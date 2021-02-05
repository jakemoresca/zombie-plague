using Godot;
using System;

public class MousePointer : Area2D
{
	private Map _map;
	private Sprite _cursor;
	private Sprite _subCursor;

	[Export]
	private float InitialX;

	[Export]
	private float InitialY;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../MainMap");
		_cursor = this.GetNode<Sprite>("./Cursor");
		_subCursor = this.GetNode<Sprite>("./Cursor/SubCursor");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	public override void _Process(float delta)
	{
		var mousePosition = this.GetGlobalMousePosition();
		var initialPosition = new Vector2(InitialX, InitialY);

		var difference = mousePosition - initialPosition;

		var nearestPosition = new Vector2(RoundUp(difference.x), RoundUp(difference.y));

		this.Position = new Vector2(nearestPosition.x + initialPosition.x - 48, nearestPosition.y + initialPosition.y - 48);
	}

	private float RoundUp(float toRound)
	{
		if (toRound % 72 == 0) return toRound;
		return (72 - toRound % 72) + toRound;
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
