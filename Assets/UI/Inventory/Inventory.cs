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
	private Sprite _cardDetails;
	private float _currentTime = 0;
	private string _phase = nameof(CommonDisplayPhase.NONE);
	private const string HIDDEN_COLOR = "00ffffff";
	private const string VISIBLE_COLOR = "ffffffff";
	private int _currentSelectedCell = -2;
	private bool _detailVisible = false;

	public override void _Ready()
	{
		_gameManager = this.GetNode<GameManager>("../../Root");
		_gridContainer = this.GetNode<GridContainer>("./ScrollContainer/ItemContainer");
		_weaponCell = this.GetNode<ItemCell>("./WeaponCell");
		_cardDetails = this.GetNode<Sprite>("./CardDetails");

		_weaponCell.Connect("ItemCellClicked", this, "_On_ItemCellClicked");

		this.Hide();
		_cardDetails.Hide();
	}

	private void _on_Close_pressed()
	{
		HideWindow();
	}

	public void SetItems(List<CardData> items)
	{
		_items = items;

		var cardDetailLabel = _cardDetails.GetNode<RichTextLabel>("./Label");
		cardDetailLabel.BbcodeText = string.Empty;

		DrawItems();
	}

	public void ShowWindow()
	{
		this.Modulate = new Color(VISIBLE_COLOR);

		this.Show();
	}

	public void HideWindow()
	{
		_cardDetails.Hide();
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

				itemCellInstance.Connect("ItemCellClicked", this, "_On_ItemCellClicked");

				cellIndex++;
			}
		}
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

	private void _On_ItemCellClicked(int cellIndex)
	{
		if(cellIndex == -1 && _weaponCell.CardData == null)
			return;

		if(_detailVisible)
		{
			_detailVisible = false;
			_cardDetails.Hide();

			return;
		}

		HighlightCell(cellIndex);
		ShowCardDetails(cellIndex);
	}

	public void HighlightCell(int cellIndex)
	{
		ClearCellHighlights();

		var outlineShader = ResourceLoader.Load<ShaderMaterial>("res://Shaders/Outline2d_outer.tres");
		var cell = cellIndex == -1 ? _weaponCell : _gridContainer.GetChild<ItemCell>(cellIndex);
		cell.Material = outlineShader;
	}

	public void ClearCellHighlights()
	{
		_weaponCell.Material = null;

		foreach(ItemCell child in _gridContainer.GetChildren())
		{
			child.Material = null;
		}
	}

	private void ShowCardDetails(int cellIndex)
	{
		_currentSelectedCell = cellIndex;
		_detailVisible = true;

		var gridCell = cellIndex == -1 ? _weaponCell : _gridContainer.GetChild<ItemCell>(cellIndex);
		var cardDetailLabel = _cardDetails.GetNode<RichTextLabel>("./Label");

		cardDetailLabel.BbcodeText = gridCell.CardData.Description;

		_cardDetails.Show();
	}
}
