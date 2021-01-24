using Godot;
using System;

public class GridCellIndicator : Area2D
{
	// Declare member variables here. Examples:
	[Export]
	private int Column = 0;

	[Export]
	private int Row = 0;

	private Map _map;

	private float _currentTime = 0;
	private string _phase = "NONE";
	private const string RED_COLOR = "ffff0000";
	private const string VISIBLE_COLOR = "ffffffff";
	private bool _disabled = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");

		UpdateGridPosition();
	}

	public void UpdateGridPosition(int? column = null, int? row = null)
	{
		if(column != null)
		{
			this.Column = column.Value;
		}

		if(row != null)
		{
			this.Row = row.Value;
		}

		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();
		var position = new GridPosition { Row = this.Row, Column = this.Column };

		initCoordinates.Item2 += 28;

		this.Position = GridHelper.GetTargetPosition(position, tileSize, initCoordinates);
		this.Modulate = new Color(RED_COLOR);

		PlayAnimation();
	}

	private void PlayAnimation()
	{
		_phase = "SHOWING";
	}

	private void StopAnimation()
	{
		_phase = "NONE";
	}

	public override void _Process(float delta)
	{
		if(_phase == "SHOWING")
		{
			_currentTime += (delta);

			var newColor = this.Modulate.LinearInterpolate(new Color(VISIBLE_COLOR), _currentTime);
			this.Modulate = newColor;

			if(this.Modulate.ToHtml() == VISIBLE_COLOR)
			{
				_phase = "DECAY";
				_currentTime = 0;
			}
		}

		if(_phase == "DECAY")
		{
			_currentTime += (delta);

			var newColor = this.Modulate.LinearInterpolate(new Color(RED_COLOR), _currentTime);
			this.Modulate = newColor;

			if(this.Modulate.ToHtml() == RED_COLOR)
			{
				_phase = "SHOWING";
				_currentTime = 0;
			}
		}
	}

	private void _on_GridCellIndicator_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if(_disabled)
			return;

		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:

					var currentSelectedNode = _map.GetSelectedNode();

					if(currentSelectedNode is Player player)
					{
						player.MoveTo(Column, Row);
						
						StopAnimation();
					}

					break;
			}
		}
	}

}
