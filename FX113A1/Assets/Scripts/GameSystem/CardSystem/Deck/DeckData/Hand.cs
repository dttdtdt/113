using System;
using GameSystem.CardSystem.Card.Mono;
using UnityEngine;

namespace GameSystem.CardSystem.Deck.DeckData
{
    [Serializable]
    public class Hand : Deck
    {
        [SerializeField, Min(0)] private int maxHandHold = 10;

        // 保留原型的超限取回接口，手牌位合并等规则不在本次迁移中新增。
        public CardMono CheckHandHold()
            => cardList.Count > maxHandHold ? DrawFromDeck() : null;
    }
}
