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
	
	
}
