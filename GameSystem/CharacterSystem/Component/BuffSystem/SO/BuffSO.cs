using Godot;
using System;

public class Buff : Resource
{
	[Export] private int buffId;
	[Export] private String buffName;
	[Export] private Texture2D icon;
	[Export] private int duration;//持续时间,永久buff
	[Export] private int maxStack;//最大层数
	[Export] private int stack;//当前层数
	[Export] private bool stackable;//是否可以叠加
	//[Export] private List<Effect> effect;
	//[Export] private List<Condition> Condition;

	public int BuffId => buffId;
	public String BuffName => buffName;
	public Sprite Icon => icon;
	public int Duration => duration;
	public bool Stackable => stackable;
	//public List<Effect> Effect => effect;
	//public List<Condition> Condition => condition;
}
