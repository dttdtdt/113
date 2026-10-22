using Godot;
using System;
using System.Collections.Generic;

public partial class CardMono : RefCounted
{

	private CardDataRuntime CardData {get; set;}

	private CardSo baseCardInfo{get; set;}

	public CardDataRuntime GetCardData() => CardData;

	public CardSo GetBaseCardInfo() => baseCardInfo;

	public CardMono(CardSo Card)
	{
		baseCardInfo = Card;
		CardData = new CardDataRuntime(baseCardInfo);
	}

	//数据变化委托，UI 订阅后自动刷新
	public event Action DataChanged;

	private void NotifyDataChanged() => DataChanged?.Invoke();

	//加法修正（自动重算 current）
	public void AddEffectBonus(EffectKeyResource effect, int index, float value)
	{
		if (CardData.TryGetEffect(effect, out var data))
		{
			data.AddBonus(index, value);
			data.SetAllCurrent();
			NotifyDataChanged();
		}
	}

	//乘法修正（自动重算 current）
	public void MultiplyEffectBonus(EffectKeyResource effect, int index, float value)
	{
		if (CardData.TryGetEffect(effect, out var data))
		{
			data.MultiplyBonus(index, value);
			data.SetAllCurrent();
			NotifyDataChanged();
		}
	}

	//修改效果基础值
	public void SetEffectBase(EffectKeyResource effect, List<float> values)
	{
		if (CardData.TryGetEffect(effect, out var data))
		{
			data.SetAllBase(values);
			data.SetAllCurrent();
			NotifyDataChanged();
		}
	}

	//重置单个效果
	public void ResetEffect(EffectKeyResource effect)
	{
		if (CardData.TryGetEffect(effect, out var data))
		{
			data.Reset();
			NotifyDataChanged();
		}
	}

	//重置全部效果
	public void ResetAllEffects()
	{
		CardData.ResetAllEffects();
		NotifyDataChanged();
	}

	public void SetCost(int value)
	{
		CardData.SetCost(value);
		NotifyDataChanged();
	}

	public void ModifyCost(int delta)
	{
		CardData.SetCost(CardData.GetCost() + delta);
		NotifyDataChanged();
	}
}
