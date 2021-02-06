using Godot;
using System;

public class ItemCell : TextureRect
{
	private TextureRect _sprite;
	private CardData _cardData;
	private int _cellIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = this.GetNode<TextureRect>("./Sprite");

	}

	public void SetCellSprite(Texture texture)
	{
		_sprite.Texture = texture;
	}

	public void SetCellIndex(int cellIndex)
	{
		_cellIndex = cellIndex;
	}

	public void SetCardData(CardData cardData)
	{
		_cardData = cardData;
	}

	public CardData CardData => _cardData;
}
