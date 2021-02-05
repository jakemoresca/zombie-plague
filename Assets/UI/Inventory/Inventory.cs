using Godot;
using System;

public class Inventory : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Hide();
	}

	private void _on_Close_gui_input(object @event)
	{
		this.Hide();
	}
}
