using Godot;
using System;

public partial class BattleSceneMono : Node
{
	private BattleSceneSO SceneData;
	private List<object> Player = new();
	private List<object> Enemy = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = SceneData.PlayerPosition;
		Enemy = SceneData.PlayerPosition
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetCharacter(int position,object Character)
	{
		if(position > Player.Count) return;
		Player[position] = Character;
	}
	
	public void ExchangePosition(int position1,int position2)
	{
		if(position1 > Player.Count ||position2 > Player.Count) return;
		object a;
		a = Player[position1];
		Player[position1] = Player[Position2];
		Player[Position2] = a;
	}
}
