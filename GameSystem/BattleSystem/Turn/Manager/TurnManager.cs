using Godot;
using System;

public partial class TurnManager : Node
{
	private TriggerTurn MainTurn = TriggerTurn.nothing;
	private TriggerTurn BattleTurn = TriggerTurn.nothing;
	private bool MainTurnChanged = false;
	private bool BattleTurnChanged = false;
	
	private EventBus TurnStartEvent;
	private EventBus TurnEndEvent;
	private EventBus AfterBattleEvent;
	private EventBus AfterDrawEvent;
	private EventBus AfterTakeDamageEvent;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TurnStartEvent       = new EventBus();
		TurnEndEvent         = new EventBus();
		AfterBattleEvent     = new EventBus();
		AfterDrawEvent       = new EventBus();
		AfterTakeDamageEvent = new EventBus();
		MainTurn = TriggerTurn.turnstart;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(MainTurnChanged)
		{
			switch(MainTurn)
			{
				case TriggerTurn.nothing://玩家回合中
					break;
				case TriggerTurn.turnstart:
					//执行回调
					TurnStartEvent.Invoke();
					break;
				case TriggerTurn.turnend:
					//执行回调
					TurnEndEvent.Invoke();
					break;
				default:
					break;	
			}
			MainTurnChanged = false;
		}
		if(BattleTurnChanged)
		{
			switch(BattleTurn)
			{
				case TriggerTurn.nothing:
					break;
				case TriggerTurn.afterbattle:
					AfterBattleEvent.Invoke();
					break;
				case TriggerTurn.afterdraw:
					AfterDrawEvent.Invoke();
					break;
				case TriggerTurn.aftertakedamage:
					AfterTakeDamageEvent.Invoke();
					break;
				default:
					break;
			}
			BattleTurnChanged = false;
		}
		
	}
	
	// ==================== 主回合事件注册 ====================

	/// <summary>注册回合开始时的回调</summary>
	public int OnTurnStart(Action handler) => TurnStartEvent.Subscribe(handler);

	/// <summary>注册回合结束时的回调</summary>
	public int OnTurnEnd(Action handler) => TurnEndEvent.Subscribe(handler);

	// ==================== 战斗阶段事件注册 ====================

	/// <summary>注册战斗结算后的回调</summary>
	public int OnAfterBattle(Action handler) => AfterBattleEvent.Subscribe(handler);

	/// <summary>注册抽牌后的回调</summary>
	public int OnAfterDraw(Action handler) => AfterDrawEvent.Subscribe(handler);

	/// <summary>注册受到伤害后的回调</summary>
	public int OnAfterTakeDamage(Action handler) => AfterTakeDamageEvent.Subscribe(handler);

	// ==================== 通用取消订阅 ====================

	/// <summary>通过注册时返回的Token取消任意事件的订阅</summary>
	public void Unsubscribe(EventBus bus, int token) => bus.Unsubscribe(token);
	
	
	public void ChangeMainTurn(TriggerTurn a)
	{
		MainTurn = a;
		MainTurnChanged = true;
	}
	
	public void ChangeBattleTurn(TriggerTurn a)
	{
		BattleTurn = a;
		BattleTurnChanged = true;
	}
}
