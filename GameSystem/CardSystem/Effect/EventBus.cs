using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class EventBus
{
	private readonly Dictionary<TriggerTurn, List<Trigger>> _handlers = new();

	public void Subscribe(Trigger trigger)
	{
		TriggerTurn type = trigger.triggerType;
		if(_handlers.ContainsKey(type))
		{
			_handlers[type].Add(trigger);
		}
		else
		{
			_handlers[type] = new();
			_handlers[type].Add(trigger);
		}
	}

	public void Unsubscribe(Trigger trigger)
	{
		TriggerTurn type = trigger.triggerType;
		if(_handlers.ContainsKey(type))
		{
			_handlers[type].Remove(trigger);
		}
	}

	public void Invoke(TriggerTurn trigger)
	{
		foreach (var kvp in _handlers[trigger]) 
			kvp.TriggerInvoke();
	}
}
