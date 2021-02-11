using Godot;
using System;

public class AttackIndicator : Area2D
{
	// Declare member variables here. Examples:
	[Export]
	private int Column = 0;

	[Export]
	private int Row = 0;

	private GameManager _gameManager;
	private Map _map;
	private CollisionShape2D _collisionShape;
	private float _currentTime = 0;
	private string _phase = "NONE";
	private const string RED_COLOR = "ffff0000";
	private const string VISIBLE_COLOR = "ffffffff";
	private const string BLUR_COLOr = "31ffffff";
	private bool _disabled = false;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");
		_collisionShape = this.GetNode<CollisionShape2D>("./CollisionShape2D");

		UpdateGridPosition();
	}

	public void UpdateGridPosition(int? column = null, int? row = null)
	{
		if (column != null)
		{
			this.Column = column.Value;
		}

		if (row != null)
		{
			this.Row = row.Value;
		}

		var tileSize = _map.GetTileSize();
		var initCoordinates = _map.GetInitCoordinates();
		var position = new GridPosition { Row = this.Row, Column = this.Column };

		this.Position = GridHelper.GetTargetPosition(_map.Tilemap, position, (int)tileSize, initCoordinates);
		this.Modulate = new Color(RED_COLOR);

		PlayAnimation();
	}

	public new void Hide()
	{
		_collisionShape.Disabled = true;
		base.Hide();
	}

	public new void Show()
	{
		_collisionShape.Disabled = false;
		base.Show();
	}

	public void PlayAnimation()
	{
		_phase = "SHOWING";
	}

	public void StopAnimation()
	{
		_phase = "NONE";
	}

	public override void _Process(float delta)
	{
		if (_phase == "SHOWING")
		{
			_currentTime += (delta);

			var newColor = this.Modulate.LinearInterpolate(new Color(VISIBLE_COLOR), _currentTime);
			this.Modulate = newColor;

			if (this.Modulate.ToHtml() == VISIBLE_COLOR)
			{
				_phase = "DECAY";
				_currentTime = 0;
			}
		}

		if (_phase == "DECAY")
		{
			_currentTime += (delta);

			var newColor = this.Modulate.LinearInterpolate(new Color(RED_COLOR), _currentTime);
			this.Modulate = newColor;

			if (this.Modulate.ToHtml() == RED_COLOR)
			{
				_phase = "SHOWING";
				_currentTime = 0;
			}
		}
	}
}
