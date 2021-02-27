using Godot;
using System;

public class Barricade : Area2D
{
	private Map _map;
	private string _direction;
	private GridPosition _position;

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

		var animatedSprite = this.GetNode<AnimatedSprite>("./AnimatedSprite");
		animatedSprite.Animation = _direction;

		var position = GridHelper.GetTargetPosition(_map.Tilemap, _position, (int)tileSize, initCoordinates);
		this.Position = new Vector2(position.x, position.y - 48);
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
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		switch (_direction)
		{
			case "up":
				_position.Row -= 1;
				break;

			case "left":
				_position.Column -= 1;
				break;

			case "right":
				_position.Column += 1;
				break;

			case "down":
				_position.Row += 1;
				break;
		}

		var position = GridHelper.GetTargetPosition(_map.Tilemap, _position, (int)tileSize, initCoordinates);
		this.Position = new Vector2(position.x, position.y - 28);
	}

	public void PushFrom(string originDirection)
	{
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		switch (originDirection)
		{
			case "up":
				_position.Row -= 1;
				break;

			case "left":
				_position.Column -= 1;
				break;

			case "right":
				_position.Column += 1;
				break;

			case "down":
				_position.Row += 1;
				break;
		}

		var position = GridHelper.GetTargetPosition(_map.Tilemap, _position, (int)tileSize, initCoordinates);
		this.Position = new Vector2(position.x, position.y - 28);
	}

	public void SpawnTo(int column, int row)
	{
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		_position.Row = row;
		_position.Column = column;

		var position = GridHelper.GetTargetPosition(_map.Tilemap, _position, (int)tileSize, initCoordinates);
		this.Position = new Vector2(position.x, position.y - 28);

		this.Show();
	}

	public void FaceDirection(string direction)
	{
		var animatedSprite = this.GetNode<AnimatedSprite>("./AnimatedSprite");
		animatedSprite.Animation = direction;

		_direction = direction;
	}

	public bool HasPosition()
	{
		return _position.Column > 0 && _position.Row > 0;
	}

	public AnimatedSprite GetAnimatedSprite()
	{
		return this.GetNode<AnimatedSprite>("./AnimatedSprite");
	}

	public void KillUnit()
	{
		_map.RemoveChild(this);
		this.Free();
	}
}
