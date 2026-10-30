using Godot;
using System;

public partial class BuffSo : Effect
{
	[Export] private string Name;
	[Export] private Texture2D Icon;
	[Export] private bool Stackable;
	[Export] private int  MaxStack;
	
	public string GetName() => Name;
	public Texture2D GetIcon() => Icon;
	public bool GetStackable() => Stackable;
	public int GetMaxStack() => MaxStack;
}
