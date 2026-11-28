using System;
using GameSystem.CardSystem.Card.So;
using GameSystem.CardSystem.Card.Mono;

namespace GameSystem.CardSystem.Card.Manager
{
    public static class CardManager
    {
        public static CardMono CreateCard(CardSo cardso)
        {
            CardMono card = new CardMono(cardso);
            return card;
        }
    }
}
