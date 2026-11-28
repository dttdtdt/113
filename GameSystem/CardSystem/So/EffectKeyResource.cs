using Godot;
using System;

public partial class EffectKeyResource : Resource
{
	[Export] public TriggerTurn Trigger { get; set; }
	[Export] public CardEffect Effect { get; set; }
}
