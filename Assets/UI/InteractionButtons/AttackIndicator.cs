using Godot;
using System;

public class AttackIndicator : GridIndicator
{
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");
		_collisionShape = this.GetNode<CollisionShape2D>("./CollisionShape2D");

		UpdateGridPosition();
	}
}
