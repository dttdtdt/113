using Godot;
using System;

public partial class DeckManager : Node
{
	[Export]
	private Hand _HandDeck;
	
	[Export]
	private DrawDeck _DrawDeck;
	
	[Export]
	private DiscardPile _DiscardPile;
	
	[Export]
	private SharePool _SharePool;
	
	public void DrawCardFromDeck(Deck deck1, Deck deck2)//卡组1获得卡牌，从卡组2
	{
		deck1.AddToDeck(deck2.DrawFromDeck());
		if(!(deck1 is Hand))
		{
			deck1.Shuffle();
		}
	}
	
	public void AddCard(Deck deck,CardMono card)
	{
		deck.AddToDeck(card);
	}
	
	public void DeleteCard(Deck deck,int i)
	{
		deck.DeleteCard(i);
	}
	
	public void TakeCardFromDeck(Deck deck1, Deck deck2,int i)
	{
		deck1.AddToDeck(deck2.DeleteCard(i));
		if(!(deck1 is Hand))
		{
			deck1.Shuffle();
		}
	}
}
