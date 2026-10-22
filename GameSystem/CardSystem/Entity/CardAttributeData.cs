using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardAttributeData
{
	private List<float> currentValue;
	private List<float> baseValue;
	private List<float> AMall;
	private List<float> MDall;

	public CardAttributeData(List<float> value)
	{
		int count = value.Count;
		baseValue = new List<float>(value);
		currentValue = new List<float>(value);
		AMall = Enumerable.Repeat(0.0f, count).ToList();
		MDall = Enumerable.Repeat(1.0f, count).ToList();
	}

	public void AddBonus(int index, float value)
	{
		if (index < 0 || index >= AMall.Count) return;
		AMall[index] += value;
	}

	public void MultiplyBonus(int index, float value)
	{
		if (index < 0 || index >= MDall.Count) return;
		MDall[index] *= value;
	}

	public void Reset()
	{
		for (int i = 0; i < AMall.Count; i++) AMall[i] = 0.0f;
		for (int i = 0; i < MDall.Count; i++) MDall[i] = 1.0f;
		currentValue = new List<float>(baseValue);
	}

	public void SetAllCurrent()
	{
		for (int i = 0; i < baseValue.Count; i++)  // 去掉了 -1
		{
			currentValue[i] = (baseValue[i] + AMall[i]) * MDall[i];
		}
	}

	public void SetAllBase(List<float> number)
	{
		if (number == null || number.Count != baseValue.Count) return;
		for (int i = 0; i < baseValue.Count; i++)  // 去掉了 -1
		{
			baseValue[i] = number[i];
		}
	}

	public List<float> GetCurrent() => currentValue;
}
