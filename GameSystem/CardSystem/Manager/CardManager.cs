using Godot;
using System;

public static class CardManager
{
	public static CardMono CreateCard(CardSo cardso)
	{
		CardMono card = new CardMono(cardso);
		return card;
	}
}
