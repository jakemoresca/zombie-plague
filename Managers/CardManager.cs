using Godot;
using System;
using System.Collections.Generic;

public class CardManager
{
    private GameManager _root;
    private Godot.Collections.Array _cards;
    private Godot.Collections.Array _graveyard;
    private RandomNumberGenerator _random;

    public CardManager(GameManager root)
    {
        _root = root;

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

        var cardData = new CardData 
        {
            Id = cardDataJson["id"].ToString(),
            Name = cardDataJson["name"].ToString(),
            Description = cardDataJson["description"].ToString(),
            Type = cardDataJson["type"].ToString(),
            Image = cardDataJson["image"].ToString(),
        };

        _graveyard.Add(cardDataJson);
        _cards.RemoveAt(selectedIndex);

        return cardData;
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