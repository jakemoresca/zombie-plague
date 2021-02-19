using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CardManager
{
	private GameManager _root;
	private Inventory _inventory;
	private Godot.Collections.Array _cards;
	private Godot.Collections.Array _graveyard;
	private RandomNumberGenerator _random;
	private Dictionary<ulong, List<CardData>> _playerItems;

	public CardManager(GameManager root)
	{
		_root = root;

		_playerItems = new Dictionary<ulong, List<CardData>>();

		_random = new RandomNumberGenerator();
		_random.Randomize();
	}

	public void LoadCards(string mapName)
	{
		var cardDataFile = new Godot.File();
		cardDataFile.Open("res://Data/Cards/" + _root.Map.MapDataFileName, File.ModeFlags.Read);

		var content = Godot.JSON.Parse(cardDataFile.GetAsText());
		var contentResult = (Godot.Collections.Dictionary)content.Result;

		_cards = (Godot.Collections.Array)contentResult["cards"];
		_graveyard = new Godot.Collections.Array();

		cardDataFile.Close();
	}

	public CardData GetRandomCard()
	{
		var selectedIndex = (int)(_random.Randi() % _cards.Count);
		var cardDataJson = (Godot.Collections.Dictionary)_cards[selectedIndex];

		var cardType = cardDataJson["type"].ToString();

		var cardData = new CardData
		{
			Id = cardDataJson["id"].ToString(),
			Name = cardDataJson["name"].ToString(),
			Description = cardDataJson["description"].ToString(),
			Type = cardDataJson["type"].ToString(),
			Image = cardDataJson["image"].ToString(),
		};

		if (cardType == nameof(CardType.Weapon))
		{
			cardData = new WeaponCardData
			{
				Id = cardDataJson["id"].ToString(),
				Name = cardDataJson["name"].ToString(),
				Description = cardDataJson["description"].ToString(),
				Type = cardDataJson["type"].ToString(),
				Image = cardDataJson["image"].ToString(),
				Range = int.Parse(cardDataJson["range"].ToString()),
				Wide = int.Parse(cardDataJson["wide"].ToString()),
			};
		}

		_graveyard.Add(cardDataJson);
		_cards.RemoveAt(selectedIndex);

		return cardData;
	}

	public void InitiateSearch(int playerNumber, string searchableKey)
	{
		_root.Map.IncrementSearchCount(searchableKey, playerNumber);

		var cardData = GetRandomCard();

		_root.Card.SetCardData(cardData);
		_root.Card.SetPlayerNumber(playerNumber);
	}

	public void TakeCard(CardData cardData)
	{
		var currentSelectedNode = _root.Map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			var playerInstanceId = player.GetInstanceId();

			player.SetAP(player.AP - 1, true);

			if (_playerItems.ContainsKey(playerInstanceId))
			{
				if (PlayerHasWeapon(playerInstanceId) && cardData.Type == CardManagerHelper.CardTypes.Weapon)
				{
					ReplaceWeapon(playerInstanceId, (WeaponCardData)cardData, out var replacedWeapon);
				}
				else
				{
					var cards = GetPlayerItems(playerInstanceId);
					_playerItems[playerInstanceId] = cards;
				}
			}
			else
			{
				_playerItems.Add(playerInstanceId, new List<CardData> { cardData });
			}
		}
	}

	private bool PlayerHasWeapon(ulong playerInstanceID)
	{
		var playerItems = GetPlayerItems(playerInstanceID);

		return playerItems.Any(x => x.Type == CardManagerHelper.CardTypes.Weapon);
	}

	private void ReplaceWeapon(ulong playerInstanceID, WeaponCardData newWeapon, out WeaponCardData replacedWeapon)
	{
		var currentWeapon = GetPlayerWeapon(playerInstanceID);
		var playerItems = GetPlayerItems(playerInstanceID);

		playerItems.Remove(currentWeapon);
		playerItems.Add(newWeapon);

		_playerItems[playerInstanceID] = playerItems;

		replacedWeapon = currentWeapon;
	}

	public void RemoveWeapon(ulong playerInstanceID)
	{
		if(PlayerHasWeapon(playerInstanceID))
		{
			var currentWeapon = GetPlayerWeapon(playerInstanceID);
			_playerItems[playerInstanceID].Remove(currentWeapon);
		}
	}

	public WeaponCardData GetPlayerWeapon(ulong playerInstanceID)
	{
		var playerItems = GetPlayerItems(playerInstanceID);
		var hasWeapon = PlayerHasWeapon(playerInstanceID);

		if(hasWeapon)
		{
			return (WeaponCardData)playerItems.FirstOrDefault(x => x.Type == CardManagerHelper.CardTypes.Weapon);
		}
		else
		{
			return GetDefaultWeapon();
		}
	}

	public void DiscardCard(CardData cardData)
	{
		var currentSelectedNode = _root.Map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			player.SetAP(player.AP - 1, true);
		}
	}

	private List<CardData> GetPlayerItems(ulong playerInstanceID)
	{
		if (!_playerItems.TryGetValue(playerInstanceID, out var playerItems))
			return new List<CardData>();

		return playerItems;
	}

	public void OpenInventory(ulong playerInstanceID)
	{
		var inventory = GetInventoryInstance();

		inventory.SetItems(GetPlayerItems(playerInstanceID));
		inventory.ShowWindow();
	}

	private Inventory GetInventoryInstance()
	{
		if (_inventory != null)
			return _inventory;

		var inventoryScene = ResourceLoader.Load<PackedScene>("res://Assets/UI/Inventory/Inventory.tscn");
		_inventory = inventoryScene.Instance() as Inventory;
		_root.AddChild(_inventory);

		return _inventory;
	}

	private WeaponCardData GetDefaultWeapon()
	{
		return new WeaponCardData { Id = "default", Name = "Barehand", Type = nameof(CardType.Weapon), Range = 1, Wide = 1 };
	}
}

public class CardData
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Type { get; set; }
	public string Image { get; set; }
}

public class WeaponCardData : CardData
{
	public int Range { get; set; }
	public int Wide { get; set; }
}

public struct CardType
{
	public string Weapon => "weapon";
	public string Item => "item";
	public string Event => "event";
}

public static class CardManagerHelper
{
	private static CardType _cardType = new CardType();

	public static StreamTexture GetCardResource(string cardImage)
	{
		var cardImageTexture = ResourceLoader.Load<StreamTexture>($"res://Graphics/Items/{cardImage}");
		return cardImageTexture;
	}

	public static CardType CardTypes => _cardType;
}
