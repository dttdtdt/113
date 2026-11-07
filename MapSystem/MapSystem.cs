using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public partial class MapSystem : Node
{
	// ---------- 基础字段 ----------
	private MapData _baseNode;
	private int _level;                    // 当前推进到第几层
	private MapBackground _background;
	private int _maxLayer = 8;             // 默认最大层数（含起点和终点，可按需改）

	// ---------- 种子相关 ----------
	private int _seed;                     // 当前地图种子
	public int Seed => _seed;              // 只读暴露，方便存档/分享
	private Random _rng;                   // 由种子驱动的统一随机数生成器

	// ---------- 暴露属性 ----------
	public MapData BaseNode => _baseNode;
	public int MaxLayer => _maxLayer;
	public int Level => _level;

	// 用于记录每层生成的节点，方便连接父子关系
	private List<List<MapData>> _layers;

	// ======================================================================
	//  种子管理
	// ======================================================================

	/// <summary>
	/// 生成一个新的随机种子，并初始化内部 RNG。
	/// 返回生成的种子值，调用方可以保存它用于复现地图。
	/// </summary>
	public int GenerateNewSeed()
	{
		_seed = Guid.NewGuid().GetHashCode();
		_rng = new Random(_seed);
		return _seed;
	}

	/// <summary>
	/// 使用指定种子初始化 RNG，保证同一个种子生成同一张地图。
	/// </summary>
	public void SetSeed(int seed)
	{
		_seed = seed;
		_rng = new Random(_seed);
	}

	/// <summary>
	/// 把种子转成可读的十六进制字符串（方便分享/输入）。
	/// </summary>
	public static string SeedToString(int seed)
	{
		return seed.ToString("X8");
	}

	/// <summary>
	/// 把十六进制字符串解析回种子；解析失败返回 0。
	/// </summary>
	public static int StringToSeed(string s)
	{
		if (string.IsNullOrWhiteSpace(s)) return 0;
		if (int.TryParse(s, System.Globalization.NumberStyles.HexNumber,
			System.Globalization.CultureInfo.InvariantCulture, out int result))
			return result;
		return 0;
	}

	// ======================================================================
	//  地图生成
	// ======================================================================

	/// <summary>
	/// 程序化生成整张地图。必须先调用 GenerateNewSeed() 或 SetSeed()。
	/// 地图结构（参考游戏流程文档）：
	///   Layer 1      : Start（起点，单节点）
	///   Layer 2 ~ N-2: 普通区域层，每层 1~3 个节点，类型不重复
	///   Layer N-1    : Rest（休整层，单节点：Rest）
	///   Layer N      : Boss（终点，单节点）
	/// </summary>
	public void RandomMap()
	{
		if (_rng == null)
		{
			GD.PushWarning("RandomMap: RNG 未初始化，自动生成新种子。");
			GenerateNewSeed();
		}

		// 按最大层分组存放每层节点
		_layers = new List<List<MapData>>();
		for (int i = 0; i <= _maxLayer; i++)
			_layers.Add(new List<MapData>());

		// ---- Layer 1: 起点 ----
		MapData start = new MapData(1, EventClass.Start);
		_layers[1].Add(start);
		_baseNode = start;

		// ---- Layer 2 ~ maxLayer-2: 普通区域 ----
		for (int i = 2; i <= _maxLayer - 2; i++)
		{
			// 每层 1~3 个节点
			int nodeCount = _rng.Next(1, 4);
			// 可用的普通事件类型集合（排除 Start / Boss / Rest）
			List<EventClass> pool = new List<EventClass>
			{
				EventClass.Event,
				EventClass.Monster,
				EventClass.Elite,
				EventClass.Treasure,
				EventClass.Shop,
				EventClass.Rest,
			};

			for (int q = 0; q < nodeCount; q++)
			{
				// 从池中随机抽一个（抽完即移除，保证兄弟节点类型不重复）
				int idx = _rng.Next(pool.Count);
				EventClass ev = pool[idx];
				pool.RemoveAt(idx);

				MapData node = new MapData(i, ev);
				_layers[i].Add(node);

				// 池子空了就重置（理论上每层最多 3 个，池子 5 个，不会耗尽）
				if (pool.Count == 0)
				{
					pool = new List<EventClass>
					{
						EventClass.Event,
						EventClass.Monster,
						EventClass.Elite,
						EventClass.Treasure,
						EventClass.Shop,
					};
				}
			}
		}

		// ---- Layer maxLayer-1: 休整层 ----
		MapData rest = new MapData(_maxLayer - 1, EventClass.Rest);
		_layers[_maxLayer - 1].Add(rest);

		// ---- Layer maxLayer: 终点 Boss ----
		MapData boss = new MapData(_maxLayer, EventClass.Boss);
		_layers[_maxLayer].Add(boss);

		// ---- 连接父子关系 ----
		ConnectLayers();

		_level = 1;

		GD.Print($"[MapSystem] 地图生成完毕，种子={SeedToString(_seed)}，共 {_maxLayer} 层");
	}

	/// <summary>
	/// 连接相邻层：每个节点指向下一层的 1~3 个节点。
	/// 最后一层（Boss）没有后继。
	/// </summary>
	private void ConnectLayers()
	{
		// 从第 1 层连接到第 maxLayer-1 层（最后一层 Boss 没有后继）
		for (int i = 1; i <= _maxLayer - 1; i++)
		{
			List<MapData> current = _layers[i];
			List<MapData> next = _layers[i + 1];

			foreach (MapData node in current)
			{
				if (next.Count == 1)
				{
					// 下一层只有一个节点（Rest / Boss），所有上一层节点都指向它
					node.AddNode(next[0]);
				}
				else
				{
					// 下一层有多个节点：随机选 1~next.Count 个指向
					int linkCount = _rng.Next(1, next.Count + 1);
					// 洗牌后取前 linkCount 个
					List<MapData> shuffled = new List<MapData>(next);
					Shuffle(shuffled);
					for (int k = 0; k < linkCount; k++)
						node.AddNode(shuffled[k]);
				}
			}
		}
	}

	/// <summary>
	/// Fisher–Yates 洗牌，使用同一个 _rng 保证可复现。
	/// </summary>
	private void Shuffle<T>(List<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = _rng.Next(i + 1);
			(list[i], list[j]) = (list[j], list[i]);
		}
	}

	// ======================================================================
	//  调试辅助
	// ======================================================================

	/// <summary>
	/// 打印整张地图结构到控制台，便于检查。
	/// </summary>
	public void PrintMap()
	{
		if (_layers == null)
		{
			GD.PushWarning("PrintMap: 地图尚未生成。");
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"===== Map (seed={SeedToString(_seed)}) =====");
		for (int i = 1; i <= _maxLayer; i++)
		{
			sb.Append($"Layer {i,2}: ");
			foreach (var n in _layers[i])
			{
				sb.Append($"[{n.Layer}:{n.WhichEvent}] → ");
				if (n.NextNode != null)
				{
					foreach (var nx in n.NextNode)
						sb.Append($"{nx.WhichEvent} ");
				}
				sb.Append("| ");
			}
			sb.AppendLine();
		}
		sb.AppendLine("========================================");
		GD.Print(sb.ToString());
	}
}
