using Godot;
using System;

public partial class Effect : Resource
{
	
	[Export] private Condition condition;
	[Export] private CardEffect cardeffect;
	[Export] private Effect effect;
	
	public Condition GetCondition() => condition;
	public CardEffect GetCardEffect() => cardeffect;
	public Effect GetEffect() => effect;
}
