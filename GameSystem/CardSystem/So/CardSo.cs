using Godot;
using System;
using Godot.Collections;

public partial class CardSo : Resource
{
	[Export]
	public String name;
	
	[Export]
	public Texture2D  CardImage;
	
	[Export]
	public CardOccupation Occupation;
	
	[Export]
	public Rarity CardRarity;
	
	[Export]
	public int Cost;
	
	[Export]
	public Dictionary<EffectKeyResource,Array<float>> Effects;
	
	[Export]
	public Array<string> Description;
	
	[Export]
	public bool BattleOrManage;
	
	[Export]
	public bool temp;
}
