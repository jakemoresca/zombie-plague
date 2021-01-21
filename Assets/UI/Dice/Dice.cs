using Godot;
using System;

public class Dice : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	[Export]
	private string[] DiceNames;

	[Export]
	private int[] DiceArtAngles;

	[Export]
	private string[] DiceArts;

	private Button _rollButton;
    private bool _rolled = false;
    private string _phase = "NONE";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rollButton = this.GetNode<Button>("./Button");

		_rollButton.Connect("pressed", this, "_on_Button_pressed");
	}

	private void _on_Button_pressed()
	{
		_rolled = true;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
