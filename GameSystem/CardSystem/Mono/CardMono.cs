using Godot;
using System;
using Godot.Collections;

public partial class CardMono : Node
{
	
	private CardDataRuntime CardData;
	
	[Export]
	private CardSo baseCardInfo;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CardData = new CardDataRuntime(baseCardInfo);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
