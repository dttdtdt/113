using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class EventBus
{
	private int _nextId = 0;
	private readonly Dictionary<int, Action> _handlers = new();

	// 注册时返回唯一句柄
	public int Subscribe(Action handler)
	{
		int id = _nextId++;
		_handlers[id] = handler;
		return id;
	}

	// 通过句柄精确移除，绝对安全
	public void Unsubscribe(int id)
	{
		_handlers.Remove(id);
	}

	public void Invoke()
	{
		// 遍历副本，防止回调内部修改集合导致异常
		foreach (var kvp in _handlers.ToList()) 
			kvp.Value();
	}
}
