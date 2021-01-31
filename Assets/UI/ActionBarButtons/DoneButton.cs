using Godot;
using System;

public class DoneButton : Area2D
{
	private Map _map;
	private GameManager _gameManager;
	private bool _disabled = false;

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_Map_finished_updating()
	{
		if (_gameManager.Phase == nameof(GamePhase.HUMAN_PLAYER_START) || _gameManager.Phase == nameof(GamePhase.ZOMBIE_PLAYER_START))
		{
			var currentSelectedNode = _map.GetSelectedNode();

			if (currentSelectedNode is Player player)
			{
				if (player.AP <= 0)
				{
					_gameManager.FinishTurn();
				}
			}
		}
	}

	private void _on_DoneButton_input_event(object viewport, object @event, int shape_idx)
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
						player.SetAP(0);
						_gameManager.FinishTurn();
					}

					break;
			}
		}
	}
}
