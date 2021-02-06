using Godot;
using System;

public class Card : Sprite
{
	[Signal]
	private delegate void Card_Taken(string cardId, int playerNumber);

	[Signal]
	private delegate void Card_Discarded(string cardId, int playerNumber);

	[Signal]
	private delegate void Card_Event_Enacted(string cardId, int playerNumber);

	private CardData _cardData;
	private RichTextLabel _title;
	private RichTextLabel _description;
	private TextureRect _cardImage;
	private string _cardType;
	private int _playerNumber;
	private GameManager _root;

	private Button _okButton;
	private Button _takeButton;
	private Button _discardButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_title = this.GetNode<RichTextLabel>("./Title");
		_description = this.GetNode<RichTextLabel>("./Description");
		_cardImage = this.GetNode<TextureRect>("./CardImage");
		_root = this.GetNode<GameManager>("../../Root");

		_okButton = this.GetNode<Button>("./OkButton");
		_takeButton = this.GetNode<Button>("./TakeButton");
		_discardButton = this.GetNode<Button>("./DiscardButton");

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
		_title.BbcodeText = _cardData?.Name;
		_description.BbcodeText = _cardData?.Description;

		if (_cardData == null)
		{
			_cardImage.Texture = null;
		}
		else
		{
			_cardImage.Texture = CardManagerHelper.GetCardResource(_cardData.Image);

			switch (_cardData.Type)
			{
				case "weapon":
				case "item":
					_okButton.Hide();
					_takeButton.Show();
					_discardButton.Show();
					break;

				case "event":
					_okButton.Show();
					_takeButton.Hide();
					_discardButton.Hide();
					break;
			}
		}
	}

	private void _on_TakeButton_pressed()
	{
		var cardId = _cardData.Id;
		var playerNumber = _playerNumber;

		_root.TakeCard(this._cardData);
		
		ClearData();
		this.Hide();

		EmitSignal(nameof(Card_Taken), cardId, playerNumber);
	}

	private void _on_DiscardButton_pressed()
	{
		var cardId = _cardData.Id;
		var playerNumber = _playerNumber;

		_root.DiscardCard(_cardData);

		ClearData();
		this.Hide();

		EmitSignal(nameof(Card_Discarded), cardId, playerNumber);
	}

	private void _on_OkButton_pressed()
	{
		var cardId = _cardData.Id;
		var playerNumber = _playerNumber;

		ClearData();
		this.Hide();

		EmitSignal(nameof(Card_Event_Enacted), cardId, playerNumber);
	}

	private void ClearData()
	{
		SetCardData(null);
		SetPlayerNumber(0);
	}
}
