using Godot;
using System;
using System.Collections.Generic;
public partial class EffectContext
{
	public TriggerTurn trigger {get; set;}
	public object caster {get; set;}
	public List<object> Target {get; set;}
	public List<int> Value {get;set; }
	
	
	//后续所有的效果制作的参数都由上下文提供,现在这一版参数之后再改
	//玩家做的任何操作，产生的任何效果都会生成一份上下文去传入事件管理中枢
	
}
