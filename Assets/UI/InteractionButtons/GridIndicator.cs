using Godot;
using System;

public class GridIndicator : Area2D
{
	[Export]
	public int Column = 0;

	[Export]
	public int Row = 0;

	public GameManager _gameManager;
	public Map _map;
	public CollisionShape2D _collisionShape;
	public float _currentTime = 0;
	public string _phase = "NONE";
	public string HIGHLIGHT_COLOR = "ffff0000";
	public string VISIBLE_COLOR = "ffffffff";
	public string BLUR_COLOr = "31ffffff";
	public bool _disabled = false;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");
		_collisionShape = this.GetNode<CollisionShape2D>("./CollisionShape2D");

		UpdateGridPosition();
	}

	public virtual void UpdateGridPosition(int? column = null, int? row = null)
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
		this.Modulate = new Color(HIGHLIGHT_COLOR);

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

	public virtual void PlayAnimation()
	{
		_phase = "SHOWING";
	}

	public virtual void StopAnimation()
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

			var newColor = this.Modulate.LinearInterpolate(new Color(HIGHLIGHT_COLOR), _currentTime);
			this.Modulate = newColor;

			if (this.Modulate.ToHtml() == HIGHLIGHT_COLOR)
			{
				_phase = "SHOWING";
				_currentTime = 0;
			}
		}
	}
}
