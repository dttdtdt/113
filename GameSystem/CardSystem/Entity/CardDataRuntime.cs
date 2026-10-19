using Godot;
using System;
using System.Collections.Generic;
public partial class CardDataRuntime
{
	private Dictionary<CardEffect,CardAttributeData> CardProperty = new();//效果数字计算器,负责记录并计算效果值
	
	public CardDataRuntime(CardSo template)
	{
		foreach(var (key, value) in template.Effects)
		{
			CardProperty[key] = new CardAttributeData(new List<float>(value));
		}
	}
}
