using Godot;
using System;

public class GROUND_TILEMAP : TileMap
{
	[Export]
	private Godot.Collections.Dictionary Animations;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//var cell = this.GetCell(-12, -10);
		//var autotileCell = this.GetCellAutotileCoord(-12, -10);  // cellid: 9 - autotile: 2, 1
	}
}
