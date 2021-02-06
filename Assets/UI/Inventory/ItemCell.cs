using Godot;
using System;

public class ItemCell : TextureRect
{
	[Signal]
	private delegate void ItemCellClicked(int cellIndex);

	private TextureRect _sprite;
	private CardData _cardData;
	private int _cellIndex;

	public override void _Ready()
	{
		_sprite = this.GetNode<TextureRect>("./Sprite");
	}

	private void _on_ItemCell_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:

					if(_cardData == null || _cardData.Id == null)
						return;

					EmitSignal(nameof(ItemCellClicked), _cellIndex);
					break;
			}
		}
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
