using Godot;
using System;

public class ForwardButton : Area2D
{
	// Declare member variables here. Examples:
	private Map _map;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../../MainMap");
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }

	private void _on_Forward_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:

					var currentSelectedNode = _map.GetSelectedNode();

					if(currentSelectedNode is PlayerMovement player)
					{
						player.MoveForward();
					}

					break;
			}
		}
	}

	public void UpdateDirectionalButtonsPosition(int column, int row, string playerDirection)
	{
		var directionalButtons = this.GetNode<Node2D>("../../DirectionalButtons");
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		var targetPosition = GetTargetPosition(column, row, playerDirection, tileSize, initCoordinates);
		var targetAngle = GetTargetAngle(playerDirection);

		directionalButtons.Position = targetPosition;
		directionalButtons.RotationDegrees = targetAngle;
	}

	private Vector2 GetTargetPosition(int column, int row, string direction, float tileSize, (float, float) initCoordinates)
	{
		var playerX = ((column - 1) * tileSize) + initCoordinates.Item1;
		var playerY = ((row - 1) * tileSize) + initCoordinates.Item2;

		switch(direction)
		{
			case "up":
				return new Vector2(playerX, playerY - tileSize);

			case "down":
				return new Vector2(playerX, playerY + tileSize);

			case "left":
				return new Vector2(playerX - tileSize, playerY);

			case "right":
				return new Vector2(playerX + tileSize, playerY);
		}

		return new Vector2(playerX, playerY);
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
