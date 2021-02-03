using Godot;
using System;

public class DisplayText : Node2D
{
	[Signal]
	private delegate void FinishedDisplaying(string displayName, string text);
	
	[Export]
	private float TimeToDisplay = 5000;
	[Export]
	private string Text = "";

	private float _currentTime = 0;
	private RichTextLabel _label;
	private string _phase = nameof(CommonDisplayPhase.NONE);
	public string _displayName = "";

	private const string HIDDEN_COLOR = "00ffffff";
	private const string VISIBLE_COLOR = "ffffffff";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = this.GetNode<RichTextLabel>("./RichTextLabel");
		_label.BbcodeText = Text;
	}

	public void SetText(string text)
	{
		Text = text;

		if(_label != null)
		{
			_label.BbcodeText = text;
		}
	}

	public void Display(string displayName = "", float? timeToDisplay = null)
	{
		TimeToDisplay = timeToDisplay ?? TimeToDisplay;
		this.Modulate = new Color(HIDDEN_COLOR);

		_phase = nameof(CommonDisplayPhase.SHOWING);
		_displayName = displayName;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if(_phase == nameof(CommonDisplayPhase.NONE))
			return;

		if(_phase == nameof(CommonDisplayPhase.SHOWING))
		{
			_currentTime += (delta);

			var newColor = this.Modulate.LinearInterpolate(new Color(VISIBLE_COLOR), _currentTime);
			this.Modulate = newColor;

			if(this.Modulate.ToHtml() == VISIBLE_COLOR)
			{
				_phase = nameof(CommonDisplayPhase.SHOW);
				_currentTime = TimeToDisplay;
			}
		}

		if(_phase == nameof(CommonDisplayPhase.SHOW))
		{
			_currentTime -= (delta * 1000);

			if(_currentTime <= 0)
			{
				_currentTime = 0;
				_phase = nameof(CommonDisplayPhase.DECAY);
			}
		}

		if(_phase == nameof(CommonDisplayPhase.DECAY))
		{
			_currentTime += (delta);

			var newColor = this.Modulate.LinearInterpolate(new Color(HIDDEN_COLOR), _currentTime);
			this.Modulate = newColor;

			if(this.Modulate.ToHtml() == HIDDEN_COLOR)
			{
				_phase = nameof(CommonDisplayPhase.NONE);
				_currentTime = 0;

				EmitSignal(nameof(FinishedDisplaying), _displayName, Text);
			}
		}
	}
}
