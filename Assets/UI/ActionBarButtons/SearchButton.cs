using Godot;
using System;

public class SearchButton : Area2D
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

	private void _on_Map_finished_updating()
	{
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			var gridPosition = player.GetGridPosition();

			if (GridHelper.HasSearchable(_map, gridPosition.Column, gridPosition.Row, player.GetDirection())
				&& !GridHelper.HasPlayerUnits(_gameManager, gridPosition.Column, gridPosition.Row, player.GetDirection())
				&& !player.IsDisabledToWalk()
				&& player.AP > 0
				&& player.GetPlayerNumber() != (int)PlayerNumber.Zombie)
			{
				this.Modulate = new Color("ffffff");
				_disabled = false;
			}
			else
			{
				this.Modulate = new Color("4affffff");
				_disabled = true;
			}
		}
	}

	private void _on_SearchButton_input_event(object viewport, object @event, int shape_idx)
	{
		// Replace with function body.
	}

}
