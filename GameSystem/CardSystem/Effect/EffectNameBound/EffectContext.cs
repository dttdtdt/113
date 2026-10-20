using Godot;
using System;
using System.Collections.Generic;
public partial class EffectContext
{
	public string caster {get; set;}
	public List<string> Target {get; set;}
	public List<int> Value {get;set; }
	
	public EffectContext(string caster,List<string> Target,List<int> Value)
	{
		this.caster = caster;
		this.Target = Target;
		this.Value = Value;
	}//后续所有的效果制作的参数都由上下文提供,现在这一版参数之后再改
}
