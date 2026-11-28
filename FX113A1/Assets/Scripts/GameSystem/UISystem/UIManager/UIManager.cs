using GameSystem.CardSystem.Card.Mono;
using GameSystem.UISystem.UIBound;
using UnityEngine;

namespace GameSystem.UISystem.UIManager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIPool cardPool;
        public int ActiveCount => cardPool != null ? cardPool.ActiveCount : 0;

        public CardUIBound CreateCard(CardMono card)
        {
            if (card == null || cardPool == null) return null;
            var view = cardPool.Get();
            if (view != null) view.Data = card;
            return view;
        }

        public void DeleteCard(CardUIBound view) => cardPool?.ReturnToPool(view);
    }
}
