using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class VictoryManager
{
	private GameManager _root;
    private PlayerManager _playerManager;
    private readonly int safeInitX;
    private readonly int safeInitY;
    private readonly int safeTotalX;
    private readonly int safeTotalY;
    private bool _hasZombieOnSafeZone = false;
    private bool _hasStartedCar = false;
    private bool _completedBarricade = false;
    private bool _hasNoMoreHumanPlayers = false;

	public VictoryManager(GameManager root, PlayerManager playerManager, int safeInitX, int safeInitY, int safeTotalX, int safeTotalY)
	{
		_root = root;
        _playerManager = playerManager;

        this.safeInitX = safeInitX;
        this.safeInitY = safeInitY;
        this.safeTotalX = safeTotalX;
        this.safeTotalY = safeTotalY;
    }

    public void CheckForVictory()
    {
        if(HumanPlayersWon)
        {
            _root.SetupVictory(VictoryResult.HumanWinner);
        }
        else if(HasNohumanPlayers)
        {
            _root.SetupVictory(VictoryResult.ZombieWinner);
        }
        else
        {
            _root.SetupVictory(VictoryResult.NoWinner);
        }
    }

    public void SetHasStartedCar(bool hasStartedCar)
    {
        _hasStartedCar = hasStartedCar;
    }

    public void CheckHasCompletedBarricade()
    {
        //ToDo: do completed barricade logic
        _completedBarricade = false;
    }

    public void CheckIfHasNoHumanPlayers()
    {
        _hasNoMoreHumanPlayers = !_playerManager.HasHumanPlayerUnits();
    }

    private bool HumanPlayersWon => HasStartedCar || (HasNoZombieOnSafeZone && CompletedBarricade);
    private bool HasStartedCar => _hasStartedCar;
    private bool HasNoZombieOnSafeZone => !_hasZombieOnSafeZone;
    private bool CompletedBarricade => _completedBarricade;

    private void CheckIfHasNoZombieOnSafeZone()
    {
        for(var x = safeInitX; safeInitX <= safeTotalX; x++)
        {
            for(var y = safeInitY; safeInitY <= safeTotalY; y++)
            {
                if(_playerManager.HasEnemyUnit(x, y, (int)PlayerNumber.Player1, out _))
                {
                    _hasZombieOnSafeZone = false;

                    return;
                }
            }
        }
    }

    private bool HasNohumanPlayers => _hasNoMoreHumanPlayers;
}

public enum VictoryResult
{
    ZombieWinner,
    HumanWinner,
    NoWinner
}