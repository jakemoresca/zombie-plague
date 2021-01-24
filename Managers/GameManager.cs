using Godot;
using System;

public class GameManager : Node2D
{
	[Signal]
	private delegate void PhaseChanged(string newPhase, string oldPhase);

	private string _phase = nameof(GamePhase.NONE);
	private DiceManager _diceManager;
	private SpawnPointManager _spawnPointManager;
	private DisplayText _displayText;
	public Map Map;

	public override void _Ready()
	{
		_displayText = this.GetNode<DisplayText>("./DisplayText");
		Map = this.GetNode<Map>("./MainMap");

		RegisterSignals();
		Setup();
	}

	private void Setup()
	{
		_diceManager = new DiceManager(this);
		_spawnPointManager = new SpawnPointManager(this);

		ChangePhase(nameof(GamePhase.GAME_START));
	}

	private void RegisterSignals()
	{
		//this.Connect("PhaseChanged", this, "_on_PhaseChanged");
		_displayText.Connect("FinishedDisplaying", this, "_on_FinishedDisplaying");
	}

	public void ChangePhase(string phase)
	{
		var oldPhase = _phase;
		_phase = phase;

		EmitSignal("PhaseChanged", phase, oldPhase);
		_on_PhaseChanged(phase, oldPhase);
	}

	private void _on_PhaseChanged(string newPhase, string oldPhase)
	{
		switch(newPhase)
		{
			case nameof(GamePhase.GAME_START):
				_displayText.SetText("Game START!");
				_displayText.Display(nameof(GamePhase.GAME_START));	
				break;
		}
	}

	private void _on_FinishedDisplaying(string displayName, string text)
	{
		switch(displayName)
		{
			case nameof(GamePhase.GAME_START):
				var directionDice = _diceManager.GetDirectionDice();
				directionDice.Position = new Vector2(1029.8f, 396.239f);

				directionDice.ShowDice();

				break;
		}
	}

	private void _on_Zombie_DiceRolled(string rolledValue)
	{
		_spawnPointManager.CreateSpawnPoints(rolledValue);
	}
}

public enum GamePhase
{
	NONE,
	GAME_START,
	ZOMBIE_START,
	PLAYERS_START,
	ROUND_START,
	ZOMBIE_START_TURN,
	ZOMBIE_START_MOVE, //sub phase: moving, attacking, breaching
	ZOMBIE_DONE,
	PLAYERS_START_TURN,
	PLAYERS_START_MOVE, //sub phase: moving, searching, attacking, reinforcing, starting_car
	PLAYERS_DONE,
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
