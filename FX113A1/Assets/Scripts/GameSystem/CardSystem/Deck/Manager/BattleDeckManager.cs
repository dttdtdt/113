using GameSystem.CardSystem.Deck.DeckData;
using UnityEngine;

namespace GameSystem.CardSystem.Deck.Manager
{
    public class BattleDeckManager : DeckManager
    {
        [SerializeField] private Hand handDeck = new();
        [SerializeField] private DrawDeck battleDrawDeck = new();
        [SerializeField] private DiscardPile discardPile = new();
        [SerializeField] private SharePool sharePool = new();

        public Hand HandDeck => handDeck;
        public DrawDeck BattleDrawDeck => battleDrawDeck;
        public DiscardPile DiscardPile => discardPile;
        public SharePool SharePool => sharePool;
    }
}
