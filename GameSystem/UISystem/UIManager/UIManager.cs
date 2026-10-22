using Godot;
using System;

public partial class UIManager : Node
{
	[Export] private UIPool cardPool;
	
	public CardUIBound CreateCard(CardMono card)
	{
		CardUIBound ui = cardPool.Get();
		ui.Data = card; // Data setter 内部已触发 Refresh
		return ui;
	}
	
	public void DeleteCard(CardUIBound UI)
	{
		cardPool.ReturnToPool(UI);
	}
}
