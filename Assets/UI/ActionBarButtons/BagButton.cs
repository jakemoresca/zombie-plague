using Godot;
using System;

public class BagButton : Area2D
{
	private GameManager _gameManager;
	private Map _map;
	private bool _disabled = false;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_BagButton_input_event(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (_disabled)
			return;

		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:

					var currentSelectedNode = _map.GetSelectedNode();

					if (currentSelectedNode is Player player)
					{
						_gameManager.OpenInventory(player.GetInstanceId());
					}

					break;
			}
		}
	}

	private void _on_Map_finished_updating()
	{
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			if (player.GetPlayerNumber() == (int)PlayerNumber.Zombie)
			{
				this.Modulate = new Color("4affffff");
				_disabled = true;
			}
			else
			{
				this.Modulate = new Color("ffffff");
				_disabled = false;
			}
		}
	}
}
