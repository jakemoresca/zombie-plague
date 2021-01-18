using Godot;
using System;

public class RotateRightButton : Area2D
{
	// Declare member variables here. Examples:
	private Map _map;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_RotateRight_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:

					var currentSelectedNode = _map.GetSelectedNode();

					if (currentSelectedNode is Player player)
					{
						player.FaceDirection(GetNewDirection(player));
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
		}
	}

	private string GetNewDirection(Player player)
	{
		switch (player.GetDirection())
		{
			case "up":
				return "right";

			case "down":
				return "left";

			case "left":
				return "up";

			case "right":
				return "down";
		}

		return string.Empty;
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

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
