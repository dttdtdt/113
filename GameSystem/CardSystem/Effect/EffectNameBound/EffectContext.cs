using Godot;
using System;
using Godot.Collections;
public partial class EffectContext
{
	public string caster {get; set;}
	public Array<string> Target {get; set;}
	public int Value { get; set; }
	
	public EffectContext(string caster,Array<string> Target,int Value)
	{
		this.caster = caster;
		this.Target = Target;
		this.Value = Value;
	}//后续所有的效果制作的参数都由上下文提供
}
