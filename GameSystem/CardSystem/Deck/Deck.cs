using Godot;
using System;
using System.Collections.Generic;
public partial class Deck : Node
{
	internal List<CardMono> cardList;
	
	private bool BattleOrManage;
	
	private readonly RandomNumberGenerator _rng = new();//随机数种子
	
	public CardMono DrawFromDeck()
	{
		if (cardList.Count == 0)
			return null;
			
		int lastIndex = cardList.Count - 1;
		CardMono card = cardList[lastIndex];
		cardList.RemoveAt(lastIndex); 
		return card;
	}
	
	public void AddToDeck(CardMono card)
	{
		cardList.Add(card);
	}
	
	public CardMono DeleteCard(int i)
	{
		if(i < (cardList.Count - 1))
		{
			CardMono card = cardList[i];
			cardList.RemoveAt(i);
			return card;
		}
		return null;
	}
	
	
	public void Shuffle()
	{
		int n = cardList.Count;
		
		for(int i = n - 1 ; i > 0  ; i--)
		{
			int j = _rng.RandiRange(0, i);
			(cardList[i], cardList[j]) = (cardList[j], cardList[i]); // 元组交换，简洁高效
		}
	}
	
	public override void _Ready()
	{
		cardList = new List<CardMono>();
	}
}
