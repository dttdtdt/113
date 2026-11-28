using Godot;
using System;

public partial class Trigger
{
	public TriggerTurn triggerType{get;set;}
	public EffectContext ctx{get;set;}
	public Action<EffectContext> _action;
	
	public void TriggerInvoke()
	{
		_action.Invoke(ctx);
	}
}
