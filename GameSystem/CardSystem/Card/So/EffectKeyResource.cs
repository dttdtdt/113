using Godot;

public partial class EffectKeyResource : Resource
{
	[Export] private TriggerTurn Trigger;//条件
	[Export] private CardEffect Effect;//效果

	public TriggerTurn GetTrigger() => Trigger;
	public void SetTrigger(TriggerTurn value) => Trigger = value;

	public CardEffect GetEffect() => Effect;
	public void SetEffect(CardEffect value) => Effect = value;
}
