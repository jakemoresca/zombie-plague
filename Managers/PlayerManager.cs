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
	}

	public void SetNumberOfPlayers(int numberOfPlayers)
	{
		_numberOfPlayers = numberOfPlayers;
	}

	public int GetNumberOfPlayers()
	{
		return _numberOfPlayers;
	}

	public void CreatePlayerCharacter(int playerNumber)
	{
		//1st player is always the zombie player
		if (playerNumber == (int)PlayerNumber.Zombie)
		{
			var numberOfNonZombiePlayers = _numberOfPlayers - 1;
			var numberOfZombieUnits = _playerUnits.ContainsKey(playerNumber) ? _playerUnits[playerNumber].Count : 0;
			var toCreateZombieUnits = (numberOfNonZombiePlayers * 2) - numberOfZombieUnits;

			//ToDo: randomize/unique character scene
			CreateAdditionalUnit("res://Assets/Zombie/Zombie.tscn", toCreateZombieUnits, playerNumber);
		}
		else
		{
			if(_playerUnits.ContainsKey(playerNumber) && _playerUnits.Count >= 1)
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

			if (unitScene != null)
			{
				if (_playerUnits.ContainsKey(1))
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
}

public enum PlayerNumber
{
	Zombie = 1,
	Player1 = 2,
	Player2 = 3,
	Player3 = 4,
	Player4 = 5
}
