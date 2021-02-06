using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory : Node2D
{
	private GameManager _gameManager;
	private List<CardData> _items;
	private GridContainer _gridContainer;
	private ItemCell _weaponCell;
	private float _currentTime = 0;
	private string _phase = nameof(CommonDisplayPhase.NONE);
	private const string HIDDEN_COLOR = "00ffffff";
	private const string VISIBLE_COLOR = "ffffffff";

	public override void _Ready()
	{
		_gameManager = this.GetNode<GameManager>("../../Root");
		_gridContainer = this.GetNode<GridContainer>("./ScrollContainer/ItemContainer");
		_weaponCell = this.GetNode<ItemCell>("./WeaponCell");

		this.Hide();
	}

	private void _on_Close_pressed()
	{
		HideWindow();
	}

	public void SetItems(List<CardData> items)
	{
		_items = items;
		DrawItems();
	}

	public void ShowWindow()
	{
		this.Modulate = new Color(VISIBLE_COLOR);

		this.Show();
	}

	public void HideWindow()
	{
		_phase = nameof(CommonDisplayPhase.DECAY);
	}

	public void DrawItems()
	{
		ClearGridChildren();

		var weapon = _items.FirstOrDefault(x => x.Type == CardManagerHelper.CardTypes.Weapon);

		if (weapon != null)
		{
			var cellTexture = CardManagerHelper.GetCardResource(weapon.Image);

			_weaponCell.SetCardData(weapon);
			_weaponCell.SetCellIndex(-1);
			_weaponCell.SetCellSprite(cellTexture);
		}

		var cellIndex = 0;

		foreach (var item in _items.Where(x => x.Type == CardManagerHelper.CardTypes.Item))
		{
			var itemCellScene = ResourceLoader.Load<PackedScene>("res://Assets/UI/Inventory/ItemCell.tscn");

			if (itemCellScene != null)
			{
				var itemCellInstance = (ItemCell)itemCellScene.Instance();

				_gridContainer.AddChild(itemCellInstance);

				var cellTexture = CardManagerHelper.GetCardResource(item.Image);

				itemCellInstance.SetCardData(item);
				itemCellInstance.SetCellIndex(cellIndex);
				itemCellInstance.SetCellSprite(cellTexture);

				//itemCellInstance.Connect("SpawnCellClicked", this, "_On_SpawnCellClicked");

				cellIndex++;
			}
		}

		//HighlightCell(0);
		//SetSelectedPlayerUnit(0);
	}

	public void ClearGridChildren()
	{
		_weaponCell.SetCellSprite(null);

		foreach (ItemCell child in _gridContainer.GetChildren())
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
}
