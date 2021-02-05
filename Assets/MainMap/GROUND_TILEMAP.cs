using Godot;
using System;
using System.Collections.Generic;

public class GROUND_TILEMAP : TileMap
{
	[Export]
	public string[] resources;
	[Export]
	public int[] resourceTriggerTileIds;
	[Export]
	public int[] numberOfResources;

	private Dictionary<string, AnimatedSprite> _currentAnimations;
	private RandomNumberGenerator _random;

	public override void _Ready()
	{
		_random = new RandomNumberGenerator();
		_random.Randomize();

		_currentAnimations = new Dictionary<string, AnimatedSprite>();


		for (int i = 0; i < resources.Length; i++)
		{
			var resource = resources[i];
			var resourceTriggerTileId = resourceTriggerTileIds[i];
			var numberOfResource = numberOfResources[i];

			for (var x = 1; x <= numberOfResource; x++)
			{
				var resourceKey = $"{resource}_{x}";

				var resourceScene = ResourceLoader.Load<PackedScene>(resource);
				var resourceSceneInstance = (AnimatedSprite)resourceScene.Instance();

				resourceSceneInstance.Position = GetAnimationRandomPosition(resourceTriggerTileId);
				resourceSceneInstance.Scale = new Vector2(0.3f, 0.3f);
				resourceSceneInstance.Connect("animation_finished", this, "_on_Animation_Finished", new Godot.Collections.Array { resourceTriggerTileId, resourceKey });

				this.AddChild(resourceSceneInstance);

				_currentAnimations.Add(resourceKey, resourceSceneInstance);

				resourceSceneInstance.Play();
			}
		}
	}

	private void _on_Animation_Finished(int resourceTriggerTileId, string resourceKey)
	{
		var resourceSceneInstance = _currentAnimations[resourceKey];
		resourceSceneInstance.Position = GetAnimationRandomPosition(resourceTriggerTileId);
	}

	private Vector2 GetAnimationRandomPosition(int resourceTriggerTileId)
	{
		var usedCells = this.GetUsedCellsById(resourceTriggerTileId);
		var selectedIndex = (int)(_random.Randi() % usedCells.Count);
		var cellCoordinates = (Vector2)usedCells[selectedIndex];
		var cellPosition = this.MapToWorld(cellCoordinates);

		return cellPosition;
	}
}
