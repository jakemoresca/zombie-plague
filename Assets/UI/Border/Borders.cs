using Godot;
using System;

public class Borders : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private Area2D _map;
	private Area2D _upBorder;
	private Area2D _downBorder;
	private bool _isScrollingUp;
	private bool _isScrollingDown;
	private RichTextLabel _debugPosition;
	private float _yUpLimit = 370f;
	private float _yDownLimit = -370f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Area2D>("../MainMap");
		_upBorder = this.GetNode<Area2D>("UpBorder");
		_downBorder = this.GetNode<Area2D>("DownBorder");
		_debugPosition = this.GetNode<RichTextLabel>("DebugMapPosition");

		_isScrollingUp = false;
		_isScrollingDown = false;

		_upBorder.Connect("mouse_entered", this, "_on_UpBorder_mouse_entered");
		_downBorder.Connect("mouse_entered", this, "_on_DownBorder_mouse_entered");

		_upBorder.Connect("mouse_exited", this, "_on_UpBorder_mouse_exited");
		_downBorder.Connect("mouse_exited", this, "_on_DownBorder_mouse_exited");
	}

	private void _on_UpBorder_mouse_entered()
	{
		_isScrollingUp = true;
	}

	private void _on_UpBorder_mouse_exited()
	{
		_isScrollingUp = false;
	}

	private void _on_DownBorder_mouse_entered()
	{
		_isScrollingDown = true;
	}

	private void _on_DownBorder_mouse_exited()
	{
		_isScrollingDown = false;
	}

	public override void _Process(float delta)
	{
		var currentMapPosition = _map.Position;

		if (_isScrollingUp && currentMapPosition.y < _yUpLimit)
		{
			var newPosition = new Vector2(currentMapPosition.x, currentMapPosition.y + 10);

			_map.Position = newPosition;
		}

		if (_isScrollingDown && currentMapPosition.y > _yDownLimit)
		{
			var newPosition = new Vector2(currentMapPosition.x, currentMapPosition.y - 10);

			_map.Position = newPosition;
		}

		_debugPosition.Text = $"Position : {_map.Position.x}         {_map.Position.y}";
	}
}
