using Godot;
using System;

public class PlayerMovement : Area2D
{
	// Declare member variables here. Examples:
	private Map _map;
	private string _direction;
	private GridPosition _position;

	[Signal]
	public delegate void FinishedMovement(int column, int row, string direction);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_direction = "up";

		_position = new GridPosition { Column = 0, Row = 0 };

		this.Connect("input_event", this, "_on_Player_input_event");
	}

	public void SetGridPosition(int column, int row, string direction = "up")
	{
		_direction = direction;
		_position.Column = column;
		_position.Row = row;

		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		this.Position = GetTargetPosition(_position, tileSize, initCoordinates);
	}

	public GridPosition GetGridPosition()
	{
		return _position;
	}

	public string GetDirection()
	{
		return _direction;
	}

	private void _on_Player_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:
					GD.Print($"Left button was clicked at {mouseEvent.Position}");
					_map.SelectNode(this);
					break;
			}
		}
	}

	public void MoveForward()
	{
		var currentPosition = this.Position;
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		switch (_direction)
		{
			case "up":
				_position.Row -= 1;
				this.Position = GetTargetPosition(_position, tileSize, initCoordinates);

				break;

			case "left":
				_position.Column -= 1;
				this.Position = GetTargetPosition(_position, tileSize, initCoordinates);

				break;

			case "right":
				_position.Column += 1;
				this.Position = GetTargetPosition(_position, tileSize, initCoordinates);

				break;

			case "down":
				_position.Row += 1;
				this.Position = GetTargetPosition(_position, tileSize, initCoordinates);

				break;
		}

		EmitSignal(nameof(FinishedMovement), _position.Column, _position.Row, _direction);
	}

	public void FaceDirection(string direction)
	{
		var animatedSprite = this.GetNode<AnimatedSprite>("./AnimatedSprite");
		animatedSprite.Animation = direction;

		_direction = direction;

		EmitSignal(nameof(FinishedMovement), _position.Column, _position.Row, _direction);
	}

	private Vector2 GetTargetPosition(GridPosition position, float tileSize, (float, float) initCoordinates)
	{
		var targetX = ((position.Column - 1) * tileSize) + initCoordinates.Item1;
		var targetY = ((position.Row - 1) * tileSize) + initCoordinates.Item2;

		return new Vector2(targetX, targetY);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}

