using Godot;
using System;
using System.Collections.Generic;

public partial class MapData
{
	private int _layer;				// 第几小层
	private EventClass _whichEvent;	// 事件类型
	private List<MapData> _nextNode; // 后继节点

	// 只读属性，方便外部访问
	public int Layer => _layer;
	public EventClass WhichEvent => _whichEvent;
	public IReadOnlyList<MapData> NextNode => _nextNode;

	public MapData(int layer, EventClass whichEvent)
	{
		_layer = layer;
		_whichEvent = whichEvent;
		_nextNode = new List<MapData>(); // 构造时初始化，避免 NullReference
	}

	public void AddNode(MapData map)
	{
		_nextNode.Add(map);
	}
}
