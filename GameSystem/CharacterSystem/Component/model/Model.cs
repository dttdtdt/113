using Godot;
using System;

public partial class Model : Node
{
	[Export] public SpriteFrames CardFrames { get; set; }
	private AnimatedSprite2D _sprite;
	
	private Vector2 position;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (CardFrames != null)
		{
			_sprite.SpriteFrames = CardFrames;
		}

		Play(CharacterStatus.idle);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public void Play(CharacterStatus status)
	{
		 if (_sprite == null)
			return;
		 _sprite.Play(status.ToString());
	}
}
