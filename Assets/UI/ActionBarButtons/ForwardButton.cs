using Godot;
using System;

public class ForwardButton : Area2D
{
	private GameManager _gameManager;
	private Map _map;
	private bool _disabled = false;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_Forward_input_event(Viewport viewport, InputEvent @event, int shape_idx)
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
						player.MoveForward();
					}

					break;
			}
		}
	}

	private void _on_Map_finished_updating()
	{
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			var newAngle = GetTargetAngle(player.GetDirection());
			this.RotationDegrees = newAngle;

			var gridPosition = player.GetGridPosition();

			if(GridHelper.CanMoveForward(_map, gridPosition.Column, gridPosition.Row, player.GetDirection()) 
				&& !GridHelper.HasPlayerUnits(_gameManager, gridPosition.Column, gridPosition.Row, player.GetDirection())
				&& !player.IsDisabledToWalk()
				&& player.AP > 0)
			{
				this.Modulate = new Color("ffffff");
				_disabled = false;
			}
			else
			{
				this.Modulate = new Color("4affffff");
				_disabled = true;
			}
		}
	}

	private int GetTargetAngle(string direction)
	{
		switch(direction)
		{
			case "up":
				return 0;

			case "down":
				return 180;

			case "left":
				return 270;

			case "right":
				return 90;
		}

		return 0;
	}
}
