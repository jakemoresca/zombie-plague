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
	private DisplayText _displayText;
	public Map Map;
	public SpawnQueue SpawnQueue;
	public int _currentPlayersTurn = 0;
	public int _roundNumber = 0;

	public override void _Ready()
	{
		_displayText = this.GetNode<DisplayText>("./DisplayText");
		Map = this.GetNode<Map>("./MainMap");
		SpawnQueue = this.GetNode<SpawnQueue>("./SpawnQueue");

		RegisterSignals();
		Setup();
	}

	private void Setup()
	{
		_diceManager = new DiceManager(this);
		_spawnPointManager = new SpawnPointManager(this);
		_playerManager = new PlayerManager(this);

		_playerManager.SetNumberOfPlayers(5);

		ChangePhase(nameof(GamePhase.GAME_START));
	}

	private void RegisterSignals()
	{
		this.Connect("PhaseChanged", this, "_on_PhaseChanged");
		_displayText.Connect("FinishedDisplaying", this, "_on_DisplayText_FinishedDisplaying");
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
		switch (newPhase)
		{
			case nameof(GamePhase.GAME_START):
				_displayText.SetText("Game START!");
				_displayText.Display(nameof(GamePhase.GAME_START));
				break;

			case nameof(GamePhase.ZOMBIE_START):
				var directionDice = _diceManager.GetDirectionDice();
				directionDice.Position = new Vector2(1029.8f, 396.239f);

				directionDice.ShowDice();

				_displayText.SetText("Zombie Player, Roll a Dice.");
				_displayText.Display();

				break;

			case nameof(GamePhase.PLAYERS_START):

				_displayText.SetText("Human Player, Set your spawn location.");
				_displayText.Display();

				break;

			case nameof(GamePhase.ROUND_START):

				_roundNumber += 1;

				_displayText.SetText($"Round {_roundNumber} Start!");
				_displayText.Display(nameof(GamePhase.ROUND_START), 1000);

				SpawnQueue.HideWindow();
				_spawnPointManager.Hide();

				break;

			case nameof(GamePhase.HUMAN_PLAYER_START):

				var playerName = ((PlayerNumber)_currentPlayersTurn).ToString();
				_displayText.SetText($"{playerName}'s Turn. Survive!");
				_displayText.Display();

				_playerManager.StartPlayerUnitsTurn(_currentPlayersTurn);

				break;

			case nameof(GamePhase.ZOMBIE_PLAYER_START):

				_displayText.SetText($"Z's Turn. Get them!");
				_displayText.Display();

				_playerManager.StartPlayerUnitsTurn(_currentPlayersTurn);

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
			ChangePhase(nameof(GamePhase.PLAYERS_START));

			var nextPlayer = playerNumber + 1;
			SpawnPlayerUnits(nextPlayer);
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
		StartPlayersTurn();
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
