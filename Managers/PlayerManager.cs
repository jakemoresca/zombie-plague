using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager
{
	private GameManager _root;
	private int _numberOfPlayers;
	private Dictionary<int, List<Player>> _playerUnits;
	public int ZombieAP = 2;
	public int PlayerAP = 4;

	public PlayerManager(GameManager root)
	{
		_root = root;
		_playerUnits = new Dictionary<int, List<Player>>();

		_root.SpawnQueue.Connect("SpawnFinished", root, "_on_SpawnFinished");
	}

	public void SetNumberOfPlayers(int numberOfPlayers)
	{
		_numberOfPlayers = numberOfPlayers;
	}

	public int GetNumberOfPlayers()
	{
		return _numberOfPlayers;
	}

	public bool CanCreateAdditionalCharacter(int playerNumber)
	{
		if (playerNumber == (int)PlayerNumber.Zombie)
		{
			var numberOfNonZombiePlayers = _numberOfPlayers - 1;
			var numberOfZombieUnits = _playerUnits.ContainsKey(playerNumber) ? _playerUnits[playerNumber].Count : 0;
			var maxNumberOfZombieUnits = numberOfNonZombiePlayers * 4;

			var toCreateZombieUnits = _root.Phase == nameof(GamePhase.ZOMBIE_START) ? 1 : maxNumberOfZombieUnits - numberOfZombieUnits;

			if (toCreateZombieUnits > 2)
				toCreateZombieUnits = 2;

			return toCreateZombieUnits > 0;
		}
		else
		{
			if (_playerUnits.ContainsKey(playerNumber) && _playerUnits.Count >= 1)
				return false;

			return true;
		}
	}

	public void CreatePlayerCharacter(int playerNumber)
	{
		if (playerNumber == (int)PlayerNumber.Zombie)
		{
			var numberOfNonZombiePlayers = _numberOfPlayers - 1;
			var numberOfZombieUnits = _playerUnits.ContainsKey(playerNumber) ? _playerUnits[playerNumber].Count : 0;
			var maxNumberOfZombieUnits = numberOfNonZombiePlayers * 4;

			var toCreateZombieUnits = _root.Phase == nameof(GamePhase.ZOMBIE_START) ? 1 : maxNumberOfZombieUnits - numberOfZombieUnits;

			if (toCreateZombieUnits > 2)
				toCreateZombieUnits = 2;

			//ToDo: randomize/unique character scene
			CreateAdditionalUnit("res://Assets/Zombie/Zombie.tscn", toCreateZombieUnits, playerNumber);
		}
		else
		{
			if (_playerUnits.ContainsKey(playerNumber) && _playerUnits.Count >= 1)
				return;

			//ToDo: randomize/unique character scene
			CreateAdditionalUnit("res://Assets/Character1/Character1.tscn", 1, playerNumber);
		}
	}

	public void CreateAdditionalUnit(string resource, int numberOfUnits, int playerNumber)
	{
		var unitScene = ResourceLoader.Load<PackedScene>(resource);

		for (int i = 0; i < numberOfUnits; i++)
		{
			var unitInstance = (Player)unitScene.Instance();

			_root.Map.AddChild(unitInstance);

			unitInstance.Scale = new Vector2(2, 2);
			unitInstance.SetDisabledWalk(true);
			unitInstance.Hide();
			unitInstance.SetPlayerNumber(playerNumber);
			unitInstance.SetMaxAP(playerNumber == (int)PlayerNumber.Zombie ? ZombieAP : PlayerAP);

			if (unitScene != null)
			{
				if (_playerUnits.ContainsKey(playerNumber))
				{
					var playerList = _playerUnits[playerNumber];
					playerList.Add(unitInstance);

					_playerUnits[playerNumber] = playerList;
				}
				else
				{
					_playerUnits.Add(playerNumber, new List<Player> { unitInstance });
				}
			}
		}
	}

	public void StartUnitSpawnSubPhase(int playerNumber)
	{
		var unpositionedPlayerUnits = GetUnpositionedPlayerUnits(playerNumber);

		_root.SpawnQueue.AddUnitsToQueue(unpositionedPlayerUnits);
		_root.SpawnQueue.ShowWindow();
	}

	public List<Player> GetUnpositionedPlayerUnits(int playerNumber)
	{
		return _playerUnits[playerNumber].Where(x => !x.HasPosition()).ToList();
	}

	public void StartPlayerUnitsTurn(int playerNumber)
	{
		var playerUnits = _playerUnits[playerNumber];

		if(!playerUnits.Any())
		{
			return;
		}

		foreach (var playerUnit in playerUnits)
		{
			playerUnit.ReplenishAP();
			playerUnit.SetDisabledWalk(false);
		}

		_root.Map.SelectNode(playerUnits[0]);
	}

	public List<Player> GetUnplayedUnits(int playerNumber)
	{
		return _playerUnits[playerNumber].Where(x => x.AP > 0).ToList();
	}

	public bool HasPlayerUnits(int column, int row)
	{
		return _playerUnits.Any(x => x.Value.Any(y =>
		{
			var position = y.GetGridPosition();

			return position.Column == column && position.Row == row;
		}));
	}

	public bool HasEnemyUnit(int column, int row, int playerNumber, out Player enemy)
	{
		if (playerNumber == (int)PlayerNumber.Zombie)
		{
			foreach (var player in _playerUnits.Where(x => x.Key != (int)PlayerNumber.Zombie))
			{
				foreach (var playerUnit in player.Value)
				{
					var position = playerUnit.GetGridPosition();
					var isEnemyInPosition = position.Column == column && position.Row == row;

					if (isEnemyInPosition)
					{
						enemy = playerUnit;
						return true;
					}
				}
			}
		}
		else
		{
			foreach (var player in _playerUnits.Where(x => x.Key == (int)PlayerNumber.Zombie))
			{
				foreach (var playerUnit in player.Value)
				{
					var position = playerUnit.GetGridPosition();
					var isEnemyInPosition = position.Column == column && position.Row == row;

					if (isEnemyInPosition)
					{
						enemy = playerUnit;
						return true;
					}
				}
			}
		}

		enemy = null;
		return false;
	}

	public void KillUnit(Player player)
	{
		var playerNumber = player.GetPlayerNumber();
		
		_playerUnits[playerNumber].Remove(player);

		if(playerNumber == (int)PlayerNumber.Zombie)
		{
			player.KillUnit();
		}
		else
		{
			player.SetPlayerNumber((int)PlayerNumber.Zombie);
			player.SetMaxAP(ZombieAP);
			player.SetAP(0);
			
			_playerUnits[(int)PlayerNumber.Zombie].Add(player);
		}
	}
}

public enum PlayerNumber
{
	Zombie = 1,
	Player1 = 2,
	Player2 = 3,
	Player3 = 4,
	Player4 = 5
}
