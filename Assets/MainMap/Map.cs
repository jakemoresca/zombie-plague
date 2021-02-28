using Godot;
using System;

public class Map : Area2D
{
	[Export]
	private int numberOfColumns = 0;

	[Export]
	private int numberOfRows = 0;

	[Export(PropertyHint.MultilineText, "Grid Cell Size")]
	private int tileSize = 48;

	[Export(PropertyHint.MultilineText, "First Cell Grid Coordinates X-Axis")]
	private int initX = -12;

	[Export(PropertyHint.MultilineText, "First Cell Grid Coordinates Y-Axis")]
	private int initY = -10;

	[Export]
	private string mapData = "";

	[Signal]
	private delegate void FinishedUpdating();

	private Godot.Collections.Dictionary _collisionMaps;

	private Godot.Collections.Dictionary _doors;
	private Godot.Collections.Dictionary _windows;
	private Godot.Collections.Dictionary _searchables;

	private Node2D _currentSelectedNode;
	private TileMap _tileMap;
	private CollisionShape2D _collisionShape2D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		/*
		SetDimension(24, 20);
		SetTileSize(71.28f);
		SetInitCoordinates(-825.44f, -703.207f);
		*/
		_tileMap = this.GetNode<TileMap>("./Ground");
		_collisionShape2D = this.GetNode<CollisionShape2D>("./CollisionShape2D");

		LoadMapData();
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

	public void SetTileSize(int tileSize)
	{
		this.tileSize = tileSize;
	}

	public int GetTileSize()
	{
		return tileSize;
	}

	public void SetInitCoordinates(int initX, int initY)
	{
		this.initX = initX;
		this.initY = initY;
	}

	public (int, int) GetInitCoordinates()
	{
		return (initX, initY);
	}

	public void SelectNode(Node2D node)
	{
		if (_currentSelectedNode != null && _currentSelectedNode.IsConnected("FinishedMovement", this, "_on_Player_FinishedMovement"))
			_currentSelectedNode.Disconnect("FinishedMovement", this, "_on_Player_FinishedMovement");

		_currentSelectedNode = node;

		_currentSelectedNode.Connect("FinishedMovement", this, "_on_Player_FinishedMovement");

		if (node is Player player)
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
		_searchables = (Godot.Collections.Dictionary)contentResult["searchables"];

		mapDataFile.Close();
	}

	public Godot.Collections.Dictionary CollisionMaps => _collisionMaps;

	public Godot.Collections.Dictionary Doors => _doors;

	public Godot.Collections.Dictionary Windows => _windows;
	public Godot.Collections.Dictionary Searchables => _searchables;

	public TileMap Tilemap => _tileMap;

	public string MapDataFileName => mapData;

	public CollisionShape2D CollisionShape2D => _collisionShape2D;

	public void IncrementSearchCount(string searchableKey, int playerNumber)
	{
		var searchable = (Godot.Collections.Dictionary)_searchables[searchableKey];

		var hasSearchCount = searchable.Contains("searchCount");

		if (hasSearchCount)
		{
			var searchCount = (int)searchable["searchCount"];
			searchCount += 1;

			searchable["searchCount"] = searchCount;
		}
		else
		{
			searchable.Add("searchCount", 1);
		}

		searchable.Add($"searchByPlayer{playerNumber}", true);
	}

	public void SetBarricadeStatus(string barricadeKey, bool isClosed)
	{
		Godot.Collections.Dictionary barricade;

		if (_doors.Contains(barricadeKey))
		{
			barricade = (Godot.Collections.Dictionary)_doors[barricadeKey];
		}
		else if (_windows.Contains(barricadeKey))
		{
			barricade = (Godot.Collections.Dictionary)_windows[barricadeKey];
		}
		else
		{
			return;
		}

		var hasIsClosed = barricade.Contains("IsClosed");

		if (hasIsClosed)
		{
			barricade["IsClosed"] = isClosed;
		}
		else
		{
			barricade.Add("IsClosed", isClosed);
		}
	}
}
