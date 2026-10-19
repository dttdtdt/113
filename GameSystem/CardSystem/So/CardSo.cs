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
	public Dictionary<CardEffect,Array<float>> Effects;
	
	[Export]
	public Array<string> Description;
}
