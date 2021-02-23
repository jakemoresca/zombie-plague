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

		this.Connect("input_event", this, "_on_Indicator_input_event");
	}

	private void _on_Indicator_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:
					
					var selectedNode = _map.GetSelectedNode();

					if(selectedNode is Player player)
					{
						MovePlayerUnitIntoPosition(player, this.PlayerMove.AttackerPosition);
						_gameManager.SetupAttack(player, PlayerMove);
					}

					break;
			}
		}
	}
}
