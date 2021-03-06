using Godot;
using System;

public class SetBarricade : Area2D
{
	private GameManager _gameManager;
	private Map _map;
	private bool _disabled = false;
	private Sprite _sprite;
	private bool _isSetMode = true;

	private string SetBarricadeImage = "res://Assets/UI/ActionBarButtons/SetBarricade.png";
	private string DestroyBarricadeImage = "res://Assets/UI/ActionBarButtons/DestroyBarricade.png";

	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_gameManager = this.GetNode<GameManager>("../../../Root");
		_sprite = this.GetNode<Sprite>("./Sprite");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_Map_finished_updating()
	{
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			var gridPosition = player.GetGridPosition();
			var direction = player.GetDirection();
			var targetPosition = GridHelper.GetTargetPosition(gridPosition.Column, gridPosition.Row, direction);
			var playerNumber = player.GetPlayerNumber();
			var isHumanPlayer = playerNumber != (int)PlayerNumber.Zombie;
			var isZombiePlayer = playerNumber == (int)PlayerNumber.Zombie;

			if (isZombiePlayer || _gameManager.HasBarricade(targetPosition.Column, targetPosition.Row))
			{
				ChangeToDestroy();
			}
			else
			{
				ChangeToSet();
			}

			if (_isSetMode)
			{
				if (GridHelper.CanSetBarricade(_map, gridPosition.Column, gridPosition.Row, direction, playerNumber)
					&& !player.IsDisabledToWalk()
					&& player.AP > 0
					&& isHumanPlayer)
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
			else
			{
				if(GridHelper.HasFourPileFriendlies(_gameManager, gridPosition, direction, playerNumber) && isZombiePlayer && player.AP > 0)
				{
					this.Modulate = new Color("ffffff");
					_disabled = false;
				}
				else if(_gameManager.HasBarricade(targetPosition.Column, targetPosition.Row) && isHumanPlayer && player.AP >= 4)
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
	}

	private void _on_SetBarricade_input_event(object viewport, object @event, int shape_idx)
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
						var gridPosition = player.GetGridPosition();
						var targetPosition = GridHelper.GetTargetPosition(gridPosition.Column, gridPosition.Row, player.GetDirection());

						if(_isSetMode)
						{
							_gameManager.SpawnBarricade(targetPosition.Column, targetPosition.Row);
							player.SetAP(player.AP - 1, true);

							_gameManager.CheckForVictory();
						}
						else
						{
							_gameManager.RemoveBarricade(targetPosition.Column, targetPosition.Row);

							if(player.GetPlayerNumber() == (int)PlayerNumber.Zombie)
							{
								player.MoveForward();
							}
							else
							{
								player.SetAP(player.AP - 4, true);
							}
						}
					}

					break;
			}
		}
	}

	private void ChangeToDestroy()
	{
		_isSetMode = false;

		var texture = ResourceLoader.Load<Texture>(DestroyBarricadeImage);
		_sprite.Texture = texture;
	}

	private void ChangeToSet()
	{
		_isSetMode = true;

		var texture = ResourceLoader.Load<Texture>(SetBarricadeImage);
		_sprite.Texture = texture;
	}
}
