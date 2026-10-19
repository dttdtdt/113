using Godot;
using System;

public partial class CardAttributeData
{
	private float currentValue;
	private float baseValue;//作用于记录卡牌属性的基础和当前值
	private float AMall = 0.0f;
	private float MDall = 1.0f;//以上两行标记加减总量，和乘除总量
	public CardAttributeData(float value)
	{
		this.baseValue = value;
		currentValue = value;
	}
	public void SetCurrent()
	{
		currentValue = (baseValue + AMall) * MDall;//永远先算加减再算乘除，因为我并不想让玩家去算buff最优牌序，先上buff，再打牌这就是最轻松的了，如果出现了存在优先级的buff，另外开一个计算函数
	}
	public void Setbase(
		DataOperation operate,//该参数决定对于基础数值的操作
		float number
	)
	{
		switch(operate)
		{
			case DataOperation.Addition:
				baseValue += number;
				break;
			case DataOperation.Minus:
				baseValue -= number;
				break;
			case DataOperation.Multiply:
				baseValue *= number;
				break;
			case DataOperation.Division:
				baseValue /= number;
				break;
			default:
				break;
		}
	}
	public float getCurrent()
	{
		return currentValue;
	}
}
