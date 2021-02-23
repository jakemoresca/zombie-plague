using Godot;
using System;
using System.Collections.Generic;

public class GameManager : Node2D
{
	[Signal]
	private delegate void PhaseChanged(string newPhase, string oldPhase);

	private string _phase = nameof(GamePhase.NONE);
	private DiceManager _diceManager;
	private SpawnPointManager _spawnPointManager;
	private PlayerManager _playerManager;
	private CardManager _cardManager;
	private MovementManager _movementManager;
	private DisplayText _displayText;
	public Map Map;
	public SpawnQueue SpawnQueue;
	public int _currentPlayersTurn = 0;
	public int _roundNumber = 0;
	public Card Card;

	public override void _Ready()
	{
		_displayText = this.GetNode<DisplayText>("./DisplayText");
		Map = this.GetNode<Map>("./MainMap");
		SpawnQueue = this.GetNode<SpawnQueue>("./SpawnQueue");
		Card = this.GetNode<Card>("./Card");

		RegisterSignals();
		Setup();
	}

	private void Setup()
	{
		_diceManager = new DiceManager(this);
		_spawnPointManager = new SpawnPointManager(this);
		_playerManager = new PlayerManager(this);
		_cardManager = new CardManager(this);
		_movementManager = new MovementManager(this, _playerManager);

		_playerManager.SetNumberOfPlayers(5);
		_cardManager.LoadCards("MainMap.json");

		ChangePhase(nameof(GamePhase.GAME_START));
	}

	private void RegisterSignals()
	{
		this.Connect("PhaseChanged", this, "_on_PhaseChanged");
		_displayText.Connect("FinishedDisplaying", this, "_on_DisplayText_FinishedDisplaying");
		Map.Connect("FinishedUpdating", this, "_on_Map_FinishedDisplaying");
	}

	public void ChangePhase(string phase)
	{
		var oldPhase = _phase;
		_phase = phase;

		EmitSignal("PhaseChanged", phase, oldPhase);
		//_on_PhaseChanged(phase, oldPhase);
	}

	private void _on_PhaseChanged(string newPhase, string oldPhase)
	{
		var directionDice = _diceManager.GetDirectionDice();

		switch (newPhase)
		{
			case nameof(GamePhase.GAME_START):
				_displayText.SetText("[center]Game START![/center]");
				_displayText.Display(nameof(GamePhase.GAME_START));
				break;

			case nameof(GamePhase.ZOMBIE_START):
				//directionDice.Position = new Vector2(1029.8f, 396.239f);
				directionDice.ShowDice();

				_displayText.SetText("[center]Zombie Player, Roll a Dice.[/center]");
				_displayText.Display();

				break;

			case nameof(GamePhase.PLAYERS_START):

				_displayText.SetText("[center]Human Player, Set your spawn location.[/center]");
				_displayText.Display();

				break;

			case nameof(GamePhase.ROUND_START):

				_roundNumber += 1;

				_displayText.SetText($"[center]Round {_roundNumber} Start![/center]");
				_displayText.Display(nameof(GamePhase.ROUND_START), 1000);

				SpawnQueue.HideWindow();
				_spawnPointManager.Hide();

				break;

			case nameof(GamePhase.HUMAN_PLAYER_START):

				var playerName = ((PlayerNumber)_currentPlayersTurn).ToString();
				_displayText.SetText($"[center]{playerName} Turn. Survive![/center]");
				_displayText.Display();

				_playerManager.StartPlayerUnitsTurn(_currentPlayersTurn);

				break;

			case nameof(GamePhase.ZOMBIE_PLAYER_START):

				_displayText.SetText($"[center]Z's Turn. Get them![/center]");
				_displayText.Display();

				_playerManager.StartPlayerUnitsTurn(_currentPlayersTurn);

				break;

			case nameof(GamePhase.ROUND_END):

				if (_playerManager.CanCreateAdditionalCharacter((int)PlayerNumber.Zombie))
				{
					_displayText.SetText($"Player Z, Let's add minions.");
					_displayText.Display();

					directionDice.ShowDice();
				}
				else
				{
					_displayText.SetText($"You are already strong enough Player Z, no more minions for you.");
					_displayText.Display(nameof(GamePhase.ROUND_START));
				}

				break;
		}
	}

	private void _on_DisplayText_FinishedDisplaying(string displayName, string text)
	{
		switch (displayName)
		{
			case nameof(GamePhase.GAME_START):
				ChangePhase(nameof(GamePhase.ZOMBIE_START));

				break;

			case nameof(GamePhase.ROUND_START):
				StartPlayersTurn();

				break;
		}
	}

	private void _on_Zombie_DiceRolled(string rolledValue)
	{
		_displayText.SetText($"You rolled, {rolledValue}");
		_displayText.Display();

		_spawnPointManager.CreateSpawnPoints(rolledValue, (int)PlayerNumber.Zombie);
		_playerManager.CreatePlayerCharacter((int)PlayerNumber.Zombie);

		var directionDice = _diceManager.GetDirectionDice();
		directionDice.HideDice();

		_playerManager.StartUnitSpawnSubPhase((int)PlayerNumber.Zombie);
	}

	private void _on_SpawnFinished(int playerNumber)
	{
		if (playerNumber == (int)PlayerNumber.Zombie)
		{
			if (_phase == nameof(GamePhase.ROUND_END))
			{
				ChangePhase(nameof(GamePhase.ROUND_START));
			}
			else
			{
				ChangePhase(nameof(GamePhase.PLAYERS_START));

				var nextPlayer = playerNumber + 1;
				SpawnPlayerUnits(nextPlayer);
			}
		}
		else if (playerNumber < (int)PlayerNumber.Player4)
		{
			var nextPlayer = playerNumber + 1;
			SpawnPlayerUnits(nextPlayer);
		}
		else if (playerNumber == (int)PlayerNumber.Player4)
		{
			ChangePhase(nameof(GamePhase.ROUND_START));
		}
	}

	private void SpawnPlayerUnits(int nextPlayer)
	{
		var oppositeDirection = _spawnPointManager.GetOppositeDirection(LastZombieDirection);
		_spawnPointManager.CreateSpawnPoints(oppositeDirection, nextPlayer);
		_playerManager.CreatePlayerCharacter(nextPlayer);
		_playerManager.StartUnitSpawnSubPhase(nextPlayer);
	}

	public string Phase => _phase;
	public void SetLastZombieDirection(string direction) => _spawnPointManager.SetLastZombieDirection(direction);
	public string LastZombieDirection => _spawnPointManager.GetLastZombieDirection();

	public List<int> TurnOrder => new List<int>
	{
		{ (int)PlayerNumber.Player1 },
		{ (int)PlayerNumber.Player2 },
		{ (int)PlayerNumber.Player3 },
		{ (int)PlayerNumber.Player4 },
		{ (int)PlayerNumber.Zombie }
	};

	public int GetNextPlayer()
	{
		if (_currentPlayersTurn == 0 || TurnOrder.IndexOf(_currentPlayersTurn) == TurnOrder.Count - 1)
		{
			return TurnOrder[0];
		}
		else
		{
			return TurnOrder[TurnOrder.IndexOf(_currentPlayersTurn) + 1];
		}
	}

	public void StartPlayersTurn()
	{
		var nextplayerNumber = GetNextPlayer();
		_currentPlayersTurn = nextplayerNumber;

		if (nextplayerNumber == (int)PlayerNumber.Zombie)
		{
			ChangePhase(nameof(GamePhase.ZOMBIE_PLAYER_START));
		}
		else
		{
			ChangePhase(nameof(GamePhase.HUMAN_PLAYER_START));
		}
	}

	public void FinishTurn()
	{
		var unplayedUnits = _playerManager.GetUnplayedUnits(_currentPlayersTurn);

		if (unplayedUnits.Count > 0)
		{
			Map.SelectNode(unplayedUnits[0]);
		}
		else
		{
			if (_currentPlayersTurn == (int)PlayerNumber.Zombie)
			{
				ChangePhase(nameof(GamePhase.ROUND_END));
			}
			else
			{
				StartPlayersTurn();
			}
		}
	}

	public bool HasPlayerUnits(int column, int row) => _playerManager.HasPlayerUnits(column, row);
	public bool HasEnemyUnit(int column, int row, int playerNumber, out Player enemy) => _playerManager.HasEnemyUnit(column, row, playerNumber, out enemy);

	public void InitiateSearch(int playerNumber, string searchableKey)
	{
		_cardManager.InitiateSearch(playerNumber, searchableKey);
	}

	public void TakeCard(CardData cardData) => _cardManager.TakeCard(cardData);

	public void DiscardCard(CardData cardData) => _cardManager.DiscardCard(cardData);

	public void OpenInventory(ulong playerInstanceID) => _cardManager.OpenInventory(playerInstanceID);

	private void _on_Map_FinishedDisplaying()
	{
		if (_phase == nameof(GamePhase.HUMAN_PLAYER_START) || _phase == nameof(GamePhase.ZOMBIE_PLAYER_START))
		{
			var selectedNode = Map.GetSelectedNode();

			if (selectedNode is Player player)
			{
				var playerPosition = player.GetGridPosition();
				var playerWeapon = _cardManager.GetPlayerWeapon(player.GetInstanceId());

				var movePositions = _movementManager.GetMovePositions(playerPosition, player.GetDirection(), player.AP,
					player.GetPlayerNumber(), playerWeapon.Range, playerWeapon.Wide);

				_movementManager.ShowMovePoints(movePositions);
			}
		}
	}

	public void SetupAttack(Player player, PlayerMove playerMove)
	{
		_movementManager.SetCurrentMove(playerMove);

		var attackDice = _diceManager.GetAttackDice();
		attackDice.ShowDice();
	}

	private void _on_Attack_DiceRolled(string rolledValue)
	{
		var selectedPlayer = (Player)Map.GetSelectedNode();
		var weapon = _cardManager.GetPlayerWeapon(selectedPlayer.GetInstanceId());
		var currentMove = _movementManager.CurrentPlayerMove;

		_movementManager.ProcessAttackOutput(rolledValue, weapon, (string text) =>
		{
			_displayText.SetText($"[center]{text}[/center]");
			_displayText.Display();

			_cardManager.RemoveWeapon(selectedPlayer.GetInstanceId());

			return 1;
		},
		(string text) =>
		{
			_displayText.SetText($"[center]{text}[/center]");
			_displayText.Display();

			return 1;
		},
		(string text) =>
		{
			_displayText.SetText($"[center]{text}[/center]");
			_displayText.Display();

			if (GridHelper.CanMoveForward(Map, currentMove.Position.Column, currentMove.Position.Row, currentMove.Direction))
			{
				if (HasEnemyUnit(currentMove.Position.Column, currentMove.Position.Row, selectedPlayer.GetPlayerNumber(), out var enemy))
				{
					enemy.PushFrom(currentMove.Direction);
				}
			}

			return 1;
		},
		(string text) =>
		{
			_displayText.SetText($"[center]{text}[/center]");
			_displayText.Display();

			if (HasEnemyUnit(currentMove.Position.Column, currentMove.Position.Row, selectedPlayer.GetPlayerNumber(), out var enemy))
			{
				_playerManager.KillUnit(enemy);
			}

			return 1;
		});

		selectedPlayer.SetAP(selectedPlayer.AP - currentMove.APWeight, emitSignal: true);

		var attackDice = _diceManager.GetAttackDice();
		attackDice.HideDice();
	}
}

public enum GamePhase
{
	NONE,
	GAME_START,
	ZOMBIE_START,
	PLAYERS_START,
	ROUND_START,
	HUMAN_PLAYER_START,
	ZOMBIE_PLAYER_START,
	ROUND_END,
	GAME_END
}

public enum CommonDisplayPhase
{
	NONE,
	SHOWING,
	SHOW,
	DECAY
}
