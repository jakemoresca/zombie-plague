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
        if(HasDice(nameof(DiceName.DirectionDice)))
        {
            return GetDice(nameof(DiceName.DirectionDice));
        }
        else
        {
            var diceScene = ResourceLoader.Load<PackedScene>("res://Assets/UI/Dice/DirectionDice/DirectionDice.tscn");

            if (diceScene != null)
            {
                var diceInstance = (Dice)diceScene.Instance();
                _root.AddChild(diceInstance);

                diceInstance.Connect("DiceRolled", _root, "_on_Zombie_DiceRolled");
                
                SetDice(nameof(DiceName.ZombieDice), diceInstance);

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
