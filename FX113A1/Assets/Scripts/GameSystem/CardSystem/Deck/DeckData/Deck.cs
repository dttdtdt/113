using System;
using System.Collections.Generic;
using GameSystem.CardSystem.Card.Mono;

namespace GameSystem.CardSystem.Deck.DeckData
{
    [Serializable]
    public class Deck
    {
        // 牌堆内容是局内数据，不将实例写入 Inspector 配置。
        [NonSerialized] protected List<CardMono> cardList = new();
        private static readonly Random Rng = new();

        public int Count => cardList.Count;
        public IReadOnlyList<CardMono> Cards => cardList.AsReadOnly();

        public CardMono DrawFromDeck() => DeleteCard(cardList.Count - 1);

        public void AddToDeck(CardMono card)
        {
            if (card != null) cardList.Add(card);
        }

        public CardMono DeleteCard(int index)
        {
            if (index < 0 || index >= cardList.Count) return null;
            var card = cardList[index];
            cardList.RemoveAt(index);
            return card;
        }

        public void Shuffle()
        {
            for (int i = cardList.Count - 1; i > 0; i--)
            {
                int j = Rng.Next(i + 1);
                (cardList[i], cardList[j]) = (cardList[j], cardList[i]);
            }
        }
    }
}
