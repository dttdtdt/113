using Godot;
using System;

public partial class Condition : Resource
{
	[Export] private int duration;//持续时间,-1为永久
	[Export] private TriggerTurn trigger;
	
	public int Duration() => duration;
	public TriggerTurn GetTrigger() => trigger;
}
