using Godot;
using System;

public class MoveIndicator : GridIndicator
{
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");
		_collisionShape = this.GetNode<CollisionShape2D>("./CollisionShape2D");
		HIGHLIGHT_COLOR = "ff00ff00";
		VISIBLE_COLOR = "ff00ff00";

		UpdateGridPosition();
	}
}
