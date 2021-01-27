using Godot;
using System;

public class APStatus : Node2D
{
	private Map _map;
	private RichTextLabel _apLabel;
	private bool _disabled = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = this.GetNode<Map>("../../MainMap");
		_apLabel = this.GetNode<RichTextLabel>("./Cell/AP");

		_map.Connect("FinishedUpdating", this, "_on_Map_finished_updating");
	}

	private void _on_Map_finished_updating()
	{
		var currentSelectedNode = _map.GetSelectedNode();

		if (currentSelectedNode is Player player)
		{
			if(!player.IsDisabledToWalk())
			{
				_apLabel.Text = $"AP:{player.AP}";
			}
			else
			{
				_apLabel.Text = $"AP:0";
			}
		}
	}
}
