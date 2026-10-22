using Godot;
using System;
using Godot.Collections;

public partial class CardSo : Resource
{
	[Export]
	public String name{ get; set; }
	
	[Export]
	public Texture2D  CardImage{ get; set; }
	
	[Export]
	public CardType  cardType{ get; set; }
	
	[Export]
	public CardOccupation Occupation{ get; set; }
	
	[Export]
	public Rarity CardRarity{ get; set; }
	
	[Export]
	public int Cost{ get; set; }
	
	[Export]
	public Dictionary<EffectKeyResource,Array<float>> Effects{ get; set; }
	
	[Export]
	public Array<string> Description{ get; set; }
	
	[Export]
	public bool BattleOrManage{ get; set; }
	
	[Export]
	public bool temp{ get; set; }
	
	[Export]
	public OutOrIn CardClass{ get; set; }
}
