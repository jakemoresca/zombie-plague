using Godot;
using System;
using System.Collections.Generic;

public class SpawnQueue : Node2D
{
	private GameManager _gameManager;
	private GridContainer _gridContainer;
	private float _currentTime = 0;
	private string _phase = nameof(CommonDisplayPhase.NONE);
	private const string HIDDEN_COLOR = "00ffffff";
	private const string VISIBLE_COLOR = "ffffffff";
	private List<Player> _playerUnits;
	private int _currentSelectedCell = -1;

	public override void _Ready()
	{
		_playerUnits = new List<Player>();
		_gameManager = this.GetNode<GameManager>("../../Root");
		_gridContainer = this.GetNode<GridContainer>("./GridContainer");

		this.Hide();
	}

	public void ShowWindow()
	{
		this.Show();
		this.Modulate = new Color(VISIBLE_COLOR);
	}

	public void HideWindow()
	{
		_phase = nameof(CommonDisplayPhase.DECAY);
	}

	public void AddUnitsToQueue(List<Player> playerUnits)
	{
		_playerUnits = playerUnits;

		DrawPlayerUnits();
	}

	public void DrawPlayerUnits()
	{
		ClearGridChildren();

		var cellIndex = 0;

		foreach (var playerUnit in _playerUnits)
		{
			var spawnCellScene = ResourceLoader.Load<PackedScene>("res://Assets/UI/SpawnQueue/SpawnCell.tscn");

			if (spawnCellScene != null)
			{
				var spawnCellInstance = (SpawnCell)spawnCellScene.Instance();

				_gridContainer.AddChild(spawnCellInstance);

				var cellSprite = playerUnit.GetAnimatedSprite();
				var spriteTexture = cellSprite.Frames.GetFrame("down", 1);
				
				spawnCellInstance.SetCellIndex(cellIndex);
				spawnCellInstance.SetCellSprite(spriteTexture);

				spawnCellInstance.Connect("SpawnCellClicked", this, "_On_SpawnCellClicked");

				cellIndex++;
			}
		}
		
		HighlightCell(0);
		SetSelectedPlayerUnit(0);
	}

	public void HighlightCell(int cellIndex)
	{
		ClearCellHighlights();

		var outlineShader = ResourceLoader.Load<ShaderMaterial>("res://Shaders/Outline2d_outer.tres");
		var cell = _gridContainer.GetChild<TextureRect>(cellIndex);
		cell.Material = outlineShader;
	}

	public void ClearCellHighlights()
	{
		foreach(SpawnCell child in _gridContainer.GetChildren())
		{
			child.Material = null;
		}
	}

	public void ClearGridChildren()
	{
		foreach(SpawnCell child in _gridContainer.GetChildren())
		{
			child.Free();
		}
	}

	public override void _Process(float delta)
	{
		if (_phase == nameof(CommonDisplayPhase.NONE))
			return;

		if (_phase == nameof(CommonDisplayPhase.DECAY))
		{
			_currentTime += (delta * 0.004f);

			var newColor = this.Modulate.LinearInterpolate(new Color(HIDDEN_COLOR), _currentTime);
			this.Modulate = newColor;

			if (this.Modulate.ToHtml() == HIDDEN_COLOR)
			{
				_phase = nameof(CommonDisplayPhase.NONE);
				_currentTime = 0;

				this.Hide();
			}
		}
	}

	private void _On_SpawnCellClicked(int cellIndex)
	{
		HighlightCell(cellIndex);
		SetSelectedPlayerUnit(cellIndex);
	}

	private void SetSelectedPlayerUnit(int cellIndex)
	{
		_currentSelectedCell = cellIndex;

		var player = _playerUnits[cellIndex];
		_gameManager.Map.SelectNode(player);
	}
}
