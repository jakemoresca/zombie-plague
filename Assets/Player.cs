using Godot;
using System;

public class Player : Area2D
{
	// Declare member variables here. Examples:
	private Map _map;
	private string _direction;
	private GridPosition _position;
	private int _AP = 0;
	private int _maxAP = 0;
	private bool _disabledWalk;
	private int _playerNumber;

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

		this.Position = GridHelper.GetTargetPosition(_position, tileSize, initCoordinates);
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

		this.Position = GridHelper.GetTargetPosition(_position, tileSize, initCoordinates);
		SetAP(this.AP - 1);

		EmitSignal(nameof(FinishedMovement), _position.Column, _position.Row, _direction);
	}

	public void SpawnTo(int column, int row)
	{
		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();

		_position.Row = row;
		_position.Column = column;

		this.Position = GridHelper.GetTargetPosition(_position, tileSize, initCoordinates);

		this.Show();

		EmitSignal(nameof(FinishedMovement), _position.Column, _position.Row, _direction);
	}

	public void FaceDirection(string direction)
	{
		SetAP(this.AP - 1);

		var animatedSprite = this.GetNode<AnimatedSprite>("./AnimatedSprite");
		animatedSprite.Animation = direction;

		_direction = direction;

		EmitSignal(nameof(FinishedMovement), _position.Column, _position.Row, _direction);
	}

	public void SetAP(int ap)
	{
		_AP = ap;
	}

	public int AP => _AP;
	public int MaxAP => _maxAP;
	public void SetMaxAP(int maxAP)
	{
		_maxAP = maxAP;
	}

	public void ReplenishAP()
	{
		SetAP(MaxAP);
	}

	public bool HasPosition()
	{
		return _position.Column > 0 && _position.Row > 0;
	}

	public AnimatedSprite GetAnimatedSprite()
	{
		return this.GetNode<AnimatedSprite>("./AnimatedSprite");
	}

	public void SetDisabledWalk(bool disabledWalk)
	{
		_disabledWalk = disabledWalk;
	}

	public bool IsDisabledToWalk()
	{
		return _disabledWalk;
	}

	public void SetPlayerNumber(int playerNumber)
	{
		_playerNumber = playerNumber;
	}

	public int GetPlayerNumber()
	{
		return _playerNumber;
	}
}

