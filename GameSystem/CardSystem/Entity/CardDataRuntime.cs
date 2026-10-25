using Godot;
using System;
using System.Collections.Generic;
public partial class CardDataRuntime
{
	private Dictionary<EffectKeyResource,CardAttributeData> CardProperty = new();//效果数字计算器,负责记录并计算效果值
	
	private int cost;

	public int GetCost() => cost;

	public void SetCost(int value) => cost = value;

	//查询单个效果数据
	public bool TryGetEffect(EffectKeyResource key, out CardAttributeData data)
		=> CardProperty.TryGetValue(key, out data);

	public void ResetAllEffects()
	{
		foreach (var data in CardProperty.Values) data.Reset();
	}

	public void RecalculateAll()
	{
		foreach (var data in CardProperty.Values) data.SetAllCurrent();
	}

	public CardDataRuntime(CardSo template)
	{
		foreach(var (key, value) in template.GetEffects())
		{
			CardProperty[key] = new CardAttributeData(new List<float>(value));
		}
		cost = template.GetCost();
		
	}
}
