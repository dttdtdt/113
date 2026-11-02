using Godot;
using System;

public partial class BuffSo : Effect
{
	[Export] private string BuffName;
	[Export] private Texture2D Icon;
	[Export] private bool Stackable;
	[Export] private int  MaxStack;
	
	public string GetBuffName() => BuffName;
	public Texture2D GetIcon() => Icon;
	public bool GetStackable() => Stackable;
	public int GetMaxStack() => MaxStack;
}
