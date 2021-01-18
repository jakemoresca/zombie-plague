using Godot;
using System;

public class Map : Area2D
{
	// Declare member variables here. Examples:

	[Export]
	private int numberOfColumns = 0;

	[Export]
	private int numberOfRows = 0;

	[Export]
	private float tileSize = 0;

	[Export]
	private float initX = 0;

	[Export]
	private float initY = 0;

	[Export]
	private string mapData = "";

	[Signal]
	private delegate void FinishedUpdating();

	private Godot.Collections.Dictionary _collisionMaps;

	private Godot.Collections.Dictionary _doors;
	private Godot.Collections.Dictionary _windows;

	private Node2D _currentSelectedNode;

	private DisplayText _displayText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		/*
		SetDimension(24, 20);
		SetTileSize(71.28f);
		SetInitCoordinates(-825.44f, -703.207f);
		*/

		_displayText = this.GetNode<DisplayText>("../DisplayText");

		LoadMapData();
		TestInitialSetup();
	}

	public void TestInitialSetup()
	{
		var character1 = this.GetNode<Player>("./Character1");
		var zombie = this.GetNode<Player>("./Zombie");

		character1.SetGridPosition(2, 2);
		zombie.SetGridPosition(10, 11);

		_displayText.SetText("Choose your starting point.");
		_displayText.Display();
	}

	public void SetDimension(int columns, int rows)
	{
		numberOfColumns = columns;
		numberOfRows = rows;
	}

	public (int, int) GetDimension()
	{
		return (numberOfColumns, numberOfRows);
	}

	public void SetTileSize(float tileSize)
	{
		this.tileSize = tileSize;
	}

	public float GetTileSize()
	{
		return tileSize;
	}

	public void SetInitCoordinates(float initX, float initY)
	{
		this.initX = initX;
		this.initY = initY;
	}

	public (float, float) GetInitCoordinates()
	{
		return (initX, initY);
	}

	public void SelectNode(Node2D node)
	{
		if(_currentSelectedNode != null && _currentSelectedNode.IsConnected("FinishedMovement", this, "_on_Player_FinishedMovement"))
			_currentSelectedNode.Disconnect("FinishedMovement", this, "_on_Player_FinishedMovement");

		_currentSelectedNode = node;

		_currentSelectedNode.Connect("FinishedMovement", this, "_on_Player_FinishedMovement");

		if(node is Player player)
		{
			var position = player.GetGridPosition();
			_on_Player_FinishedMovement(position.Column, position.Row, player.GetDirection());
		}
	}

	public Node2D GetSelectedNode()
	{
		return _currentSelectedNode;
	}

	private void _on_Player_FinishedMovement(int column, int row, string direction)
	{
		EmitSignal(nameof(FinishedUpdating));
	}

	private void LoadMapData()
	{
		var mapDataFile = new Godot.File();
		mapDataFile.Open("res://Data/Maps/" + mapData, File.ModeFlags.Read);

		var content = Godot.JSON.Parse(mapDataFile.GetAsText());
		var contentResult = (Godot.Collections.Dictionary)content.Result;

		_collisionMaps = (Godot.Collections.Dictionary)contentResult["collisionMaps"];
		_windows = (Godot.Collections.Dictionary)contentResult["windows"];
		_doors = (Godot.Collections.Dictionary)contentResult["doors"];
	}

	public Godot.Collections.Dictionary GetCollisionMaps()
	{
		return _collisionMaps;
	}

	public Godot.Collections.Dictionary GetDoors()
	{
		return _doors;
	}

	public Godot.Collections.Dictionary GetWindows()
	{
		return _windows;
	}
}
