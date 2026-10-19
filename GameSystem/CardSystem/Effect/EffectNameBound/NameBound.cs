using Godot;
using System;
using System.Collections.Generic;
public class NameBound
{
	public delegate bool EffectHandler(EffectContext context);
	
	public readonly static Dictionary<CardEffect,EffectHandler> Bounds = new()
	{
		
	};//将效果名与效果函数入口绑定
	
	
}
