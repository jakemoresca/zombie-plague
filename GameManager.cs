using Godot;
using System;

public class GameManager : Godot.Object
{
	private string _phase = nameof(GamePhase.NONE);

	// Called when the node enters the scene tree for the first time.
	public void Setup()
	{
		
	}
}

public enum GamePhase
{
	NONE,
	ROUND_START,
	ZOMBIE_START_TURN,
	ZOMBIE_START_MOVE, //sub phase: moving, attacking, breaching
	ZOMBIE_DONE,
	PLAYERS_START_TURN,
	PLAYERS_START_MOVE, //sub phase: moving, searching, attacking, reinforcing, starting_car
	PLAYERS_DONE
}

public enum CommonDisplayPhase
{
	NONE,
	SHOWING,
	SHOW,
	DECAY
}
