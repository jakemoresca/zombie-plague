using Godot;
using System;

public class Dice : Node2D
{
	[Export]
	private string[] DiceNames;
	[Export]
	private int[] DiceArtAngles;
	[Export]
	private string[] DiceArts;
	
	[Signal]
	private delegate void DiceRolled(string rolledValue);

	private Button _rollButton;
	private Sprite _sprite;
	private RichTextLabel _label;
	private bool _rolled = false;
	private string _phase = nameof(CommonDisplayPhase.NONE);
	private float _currentTime;
	private RandomNumberGenerator _random;
	private string _lastRolledValue;
	private const string HIDDEN_COLOR = "00ffffff";
	private const string VISIBLE_COLOR = "ffffffff";
	private const string ROLLING_PHASE = "ROLLING";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Hide();

		_rollButton = this.GetNode<Button>("./Button");
		_sprite = this.GetNode<Sprite>("./Sprite");
		_label = this.GetNode<RichTextLabel>("./Label");

		_random = new RandomNumberGenerator();
		_random.Randomize();

		_rollButton.Connect("pressed", this, "_on_Button_pressed");
	}

	public void ShowDice()
	{
		this.Show();
		this.Modulate = new Color(VISIBLE_COLOR);
	}

	public void HideDice()
	{
		_phase = nameof(CommonDisplayPhase.DECAY);
	}

	private void _on_Button_pressed()
	{
		_rolled = true;
		_phase = ROLLING_PHASE;
		_lastRolledValue = string.Empty;
	}

	public override void _Process(float delta)
	{
		if(_phase == nameof(CommonDisplayPhase.NONE))
			return;

		if(_phase == ROLLING_PHASE)
		{
			_currentTime += (delta * 1000);

			var selectedIndex = _random.Randi() % DiceArts.Length;

			var texture = ResourceLoader.Load<Texture>(this.Filename.Replace($"{this.Name}.tscn", string.Empty) + DiceArts[selectedIndex]);
			_sprite.Texture = texture;
			_sprite.RotationDegrees = DiceArtAngles[selectedIndex];

			_label.BbcodeText = $"[center]{DiceNames[selectedIndex]}[/center]";

			if(_currentTime >= 1000)
			{
				_lastRolledValue = DiceNames[selectedIndex];
				_phase = nameof(CommonDisplayPhase.NONE);
				_currentTime = 0;

				EmitSignal(nameof(DiceRolled), _lastRolledValue);
			}
		}

		if(_phase == nameof(CommonDisplayPhase.DECAY))
		{
			_currentTime += (delta * 0.004f);

			var newColor = this.Modulate.LinearInterpolate(new Color(HIDDEN_COLOR), _currentTime);
			this.Modulate = newColor;

			if(this.Modulate.ToHtml() == HIDDEN_COLOR)
			{
				_phase = nameof(CommonDisplayPhase.NONE);
				_currentTime = 0;

				this.Hide();
			}
		}
	}
}
