using Godot;
using System;

public partial class OutDeckManage : DeckManager
{
	
	[Export]
	private Hand _HandDeck;
	
	[Export]
	private DrawDeck _ManageDrawDeck;
	
	[Export]
	private DrawDeck _BattleDrawDeck;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
