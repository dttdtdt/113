using Godot;
using System;
using System.Collections.Generic;
public partial class CardDataRuntime
{
	[Export]
	private Dictionary<CardAttribute,CardAttributeData> CardProperty;
	
	[Export]
	private CardSo baseCardInfo;
	
}
