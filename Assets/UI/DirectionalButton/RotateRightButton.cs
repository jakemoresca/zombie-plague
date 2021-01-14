using Godot;
using System;

public class RotateRightButton : Area2D
{
	// Declare member variables here. Examples:
	private Map _map;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../../MainMap");
	}

	private void _on_RotateRight_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:

					var currentSelectedNode = _map.GetSelectedNode();

					if (currentSelectedNode is PlayerMovement player)
					{
						player.FaceDirection(GetNewDirection(player));
					}

					break;
			}
		}
	}

	private string GetNewDirection(PlayerMovement player)
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

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
