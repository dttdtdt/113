using Godot;
using System;
using System.Collections.Generic;
public partial class UIPool : Node
{
	[Export] public PackedScene ItemScene { get; set; }
	[Export] public int PoolSize { get; set; } = 10;
	
	private Queue<CardUIBound> Pool = new();
	
	public override void _Ready()
	{
		for(int i = 0 ; i < PoolSize ; i++)	
		{
			var UI = ItemScene.Instantiate<CardUIBound>();
			UI.Visible = false;
			UI.SetPhysicsProcess(false);
			AddChild(UI);
			Pool.Enqueue(UI);
		}
	}
	
	public CardUIBound Get()
	{
		if (Pool.Count > 0)
		{
			var UI = Pool.Dequeue();
			UI.Visible = true;
			UI.SetPhysicsProcess(true);
			return UI;
		}
		// 池耗尽时动态扩展
		var newUI = ItemScene.Instantiate<CardUIBound>();
		AddChild(newUI);
		PoolSize++;
		return newUI;
	}
	
	public void ReturnToPool(CardUIBound UI)
	{
		UI.Visible = false;
		UI.SetPhysicsProcess(false);
		Pool.Enqueue(UI);
	}
}
