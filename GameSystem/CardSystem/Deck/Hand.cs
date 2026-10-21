using Godot;
using System;
using System.Collections.Generic;
public partial class Hand : Deck
{
	
	private int MaxHandHold = 10;
	
	public CardMono CheckHandHold()
	{
		if(cardList.Count > MaxHandHold)
		{
			int n = cardList.Count - 1;
			CardMono card = cardList[n];
			cardList.RemoveAt(n); 
			return card;
		}
		return null;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
