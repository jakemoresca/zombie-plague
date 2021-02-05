using Godot;
using System;

public class SpawnCell : TextureRect
{
	[Signal]
	private delegate void SpawnCellClicked(int cellIndex);

	private TextureRect _sprite;
	private int _cellIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = this.GetNode<TextureRect>("./Sprite");

		this.Connect("gui_input", this, "_on_SpawnCell_gui_input");
	}

	public void SetCellSprite(Texture texture)
	{
		_sprite.Texture = texture;
	}

	public void SetCellIndex(int cellIndex)
	{
		_cellIndex = cellIndex;
	}

	private void _on_SpawnCell_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && @mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:
					EmitSignal(nameof(SpawnCellClicked), _cellIndex);
					break;
			}
		}
	}
}
