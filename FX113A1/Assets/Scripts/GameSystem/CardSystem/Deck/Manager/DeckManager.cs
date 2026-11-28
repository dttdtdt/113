using GameSystem.CardSystem.Card.Mono;
using GameSystem.CardSystem.Deck.DeckData;
using UnityEngine;
using CardDeck = GameSystem.CardSystem.Deck.DeckData.Deck;

namespace GameSystem.CardSystem.Deck.Manager
{
    public class DeckManager : MonoBehaviour
    {
        // 参数顺序沿用原型：deck1 接收，deck2 提供卡牌。
        public void DrawCardFromDeck(CardDeck deck1, CardDeck deck2)
            => TakeCardFromDeck(deck1, deck2, (deck2?.Count ?? 0) - 1);

        public void AddCard(CardDeck deck, CardMono card) => deck?.AddToDeck(card);
        public void DeleteCard(CardDeck deck, int index) => deck?.DeleteCard(index);

        public void TakeCardFromDeck(CardDeck deck1, CardDeck deck2, int index)
        {
            if (deck1 == null || deck2 == null || ReferenceEquals(deck1, deck2)) return;
            var card = deck2.DeleteCard(index);
            if (card == null) return;
            deck1.AddToDeck(card);
            if (!(deck1 is Hand)) deck1.Shuffle();
        }
    }
}
