using Godot;
using System;

public partial class CardSo : Resource
{
	[Export]
	public String name;
	
	[Export]
	public Texture2D  CardImage;
	
	[Export]
	public CardOccupation Occupation;
	
}
