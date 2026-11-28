using GameSystem.CardSystem.Deck.DeckData;
using UnityEngine;

namespace GameSystem.CardSystem.Deck.Manager
{
    // 沿用原型结构；探索阶段的新规则尚未接入。
    public class OutDeckManage : DeckManager
    {
        [SerializeField] private Hand handDeck = new();
        [SerializeField] private DrawDeck manageDrawDeck = new();
        [SerializeField] private DrawDeck battleDrawDeck = new();

        public Hand HandDeck => handDeck;
        public DrawDeck ManageDrawDeck => manageDrawDeck;
        public DrawDeck BattleDrawDeck => battleDrawDeck;
    }
}
