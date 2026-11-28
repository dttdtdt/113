using System;
using System.Collections.Generic;
using GameSystem.CardSystem.Card.Mono;

namespace GameSystem.CardSystem.Deck.DeckData
{
    [Serializable]
    public class DiscardPile : Deck
    {
        // 原型预留的分区，尚未接入流转规则。
        [NonSerialized] private List<CardMono> locked = new();
        [NonSerialized] private List<CardMono> recycled = new();
    }
}
