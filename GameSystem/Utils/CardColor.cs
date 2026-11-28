using Godot;
using System;
using System.Collections.Generic;
public static class CardOccupationToColor
{
	private static readonly Dictionary<CardOccupation, Color> Colors = new()
	{
		[CardOccupation.warrior] = new Color(0.8f, 0.2f, 0.2f),   // 红
		[CardOccupation.caster] = new Color(0.3f, 0.4f, 0.9f),   // 蓝
		[CardOccupation.ranger] = new Color(0.2f, 0.7f, 0.3f),   // 绿
		[CardOccupation.priest] = new Color(0.9f, 0.85f, 0.3f),  // 金
	};

	/// <summary>
	/// 获取职业颜色，未知职业返回灰色
	/// </summary>
	public static Color Get(CardOccupation className)
	{
		return Colors.TryGetValue(className, out var color)
			? color
			: new Color(0.5f, 0.5f, 0.5f);
	}
}
