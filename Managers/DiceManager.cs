using Godot;
using System;
using System.Collections.Generic;

public class DiceManager
{
    private GameManager _root;
    private Dictionary<string, Dice> _dice;

    public DiceManager(GameManager root)
	{
        _root = root;
        _dice = new Dictionary<string, Dice>();
	}

    public Dice GetDice(string diceName)
    {
        return _dice[diceName];
    }

    public bool HasDice(string diceName)
    {
        return _dice.ContainsKey(diceName);
    }

    public void SetDice(string diceName, Dice dice)
    {
        _dice[diceName] = dice;
    }

    public Dice GetDirectionDice()
    {
        return GetDice(nameof(DiceName.DirectionDice), "res://Assets/UI/Dice/DirectionDice/DirectionDice.tscn", "_on_Direction_DiceRolled");
    }

    public Dice GetAttackDice()
    {
        return GetDice(nameof(DiceName.AttackDice), "res://Assets/UI/Dice/AttackDice/AttackDice.tscn", "_on_Attack_DiceRolled");
    }

    public Dice GetZombieDice()
    {
        return GetDice(nameof(DiceName.ZombieDice), "res://Assets/UI/Dice/ZombieDice/ZombieDice.tscn", "_on_Zombie_DiceRolled");
    }

    public Dice GetInfectionDice()
    {
        return GetDice(nameof(DiceName.InfectionDice), "res://Assets/UI/Dice/InfectionDice/InfectionDice.tscn", "_on_Infection_DiceRolled");
    }

    private Dice GetDice(string diceName, string resource, string eventName)
    {
        if(HasDice(diceName))
        {
            return GetDice(diceName);
        }
        else
        {
            var diceScene = ResourceLoader.Load<PackedScene>(resource);

            if (diceScene != null)
            {
                var diceInstance = (Dice)diceScene.Instance();
                _root.AddChild(diceInstance);

                diceInstance.Connect("DiceRolled", _root, eventName);
                
                SetDice(diceName, diceInstance);

                return diceInstance;
            }

            return default(Dice);
        }
    }
}

public enum DiceName
{
    DirectionDice,
    AttackDice,
    InfectionDice,
    ZombieDice
}
