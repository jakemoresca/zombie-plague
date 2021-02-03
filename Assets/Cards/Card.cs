using Godot;
using System;

public class Card : Sprite
{
	private CardData _cardData;
	private RichTextLabel _title;
	private RichTextLabel _description;
	private TextureRect _cardImage;
	private int _playerNumber;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_title = this.GetNode<RichTextLabel>("./Title");
		_description = this.GetNode<RichTextLabel>("./Description");
		_cardImage = this.GetNode<TextureRect>("./CardImage");

		this.Hide();
	}

	public void SetCardData(CardData cardData)
	{
		_cardData = cardData;
		ProcessCardData();

		this.Show();
	}

	public void SetPlayerNumber(int playerNumber)
	{
		_playerNumber = playerNumber;
	}

	private void ProcessCardData()
	{
		_title.BbcodeText = _cardData.Name;
		_description.BbcodeText = _cardData.Description;
	}
}
