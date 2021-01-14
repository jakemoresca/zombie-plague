using Godot;
using System;

public class Map : Area2D
{
	// Declare member variables here. Examples:
	private int _numberOfColumns = 0;
	private int _numberOfRows = 0;
	private float _tileSize = 0;
	private float _initX = 0;
	private float _initY = 0;
	private Node2D _currentSelectedNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetDimension(24, 20);
		SetTileSize(71.28f);
		SetInitCoordinates(-825.44f, -703.207f);

		TestInitialSetup();
	}

	public void TestInitialSetup()
	{
		var character1 = this.GetNode<PlayerMovement>("./Character1");
		var zombie = this.GetNode<PlayerMovement>("./Zombie");

		character1.SetGridPosition(2, 2);
		zombie.SetGridPosition(10, 11);
	}

	public void SetDimension(int columns, int rows)
	{
		_numberOfColumns = columns;
		_numberOfRows = rows;
	}

	public (int, int) GetDimension()
	{
		return (_numberOfColumns, _numberOfRows);
	}

	public void SetTileSize(float tileSize)
	{
		_tileSize = tileSize;
	}

	public float GetTileSize()
	{
		return _tileSize;
	}

	public void SetInitCoordinates(float initX, float initY)
	{
		_initX = initX;
		_initY = initY;
	}

	public (float, float) GetInitCoordinates()
	{
		return (_initX, _initY);
	}

	public void SelectNode(Node2D node)
	{
		if(_currentSelectedNode != null && _currentSelectedNode.IsConnected("FinishedMovement", this, "_on_Player_FinishedMovement"))
			_currentSelectedNode.Disconnect("FinishedMovement", this, "_on_Player_FinishedMovement");

		_currentSelectedNode = node;

		_currentSelectedNode.Connect("FinishedMovement", this, "_on_Player_FinishedMovement");

		if(node is PlayerMovement player)
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
		var forwardButton = this.GetNode<ForwardButton>("./DirectionalButtons/Forward");
		forwardButton.UpdateDirectionalButtonsPosition(column, row, direction);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
