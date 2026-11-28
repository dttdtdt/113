using Godot;
using System;
public partial class TextButton : Button
{
	[Export] public CardSo TestCard { get; set; }    // 测试用卡牌模板
	[Export] public UIManager Manager { get; set; }  // 场景中的 UIManager 节点

	private int _spawnCount; // 测试用：每点一次偏移一点位置，避免多张牌叠在一起

	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		if (TestCard == null || Manager == null)
		{
			GD.PushWarning("TextButton：TestCard 或 Manager 未在编辑器中配置");
			return;
		}

		var card = new CardMono(TestCard);
		CardUIBound ui = Manager.CreateCard(card);
		if (ui != null)
		{
			ui.Position = new Vector2(100 + _spawnCount * 40, 100 + _spawnCount * 40);
			_spawnCount++;
		}
	}
}
