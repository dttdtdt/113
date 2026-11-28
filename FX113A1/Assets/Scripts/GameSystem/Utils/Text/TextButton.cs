using System.Collections.Generic;
using GameSystem.CardSystem.Card.Manager;
using GameSystem.CardSystem.Card.So;
using GameSystem.UISystem.UIBound;
using TMPro;
using UnityEngine;
using CardUIManager = GameSystem.UISystem.UIManager.UIManager;

namespace GameSystem.Utils.Text
{
    // 演示输入独立于卡牌视图与对象池，不实现战斗出牌规则。
    public class TextButton : MonoBehaviour
    {
        [SerializeField] private CardSo testCard;
        [SerializeField] private CardUIManager manager;
        [SerializeField] private UnityEngine.UI.Button spawnButton;
        [SerializeField] private UnityEngine.UI.Button costButton;
        [SerializeField] private UnityEngine.UI.Button returnButton;
        [SerializeField] private TMP_Text countLabel;
        [SerializeField, Min(0)] private int initialCards = 3;
        [SerializeField, Min(1)] private int maxVisibleCards = 6;
        private readonly List<CardUIBound> views = new();

        private void OnEnable()
        {
            if (spawnButton != null) spawnButton.onClick.AddListener(SpawnCard);
            if (costButton != null) costButton.onClick.AddListener(ReduceLastCost);
            if (returnButton != null) returnButton.onClick.AddListener(ReturnLastCard);
            UpdateControls();
        }

        private void Start()
        {
            for (int i = 0; i < initialCards; i++) SpawnCard();
        }

        private void OnDisable()
        {
            if (spawnButton != null) spawnButton.onClick.RemoveListener(SpawnCard);
            if (costButton != null) costButton.onClick.RemoveListener(ReduceLastCost);
            if (returnButton != null) returnButton.onClick.RemoveListener(ReturnLastCard);
        }

        public void SpawnCard()
        {
            if (testCard == null || manager == null || views.Count >= maxVisibleCards) return;
            var view = manager.CreateCard(CardManager.CreateCard(testCard));
            if (view != null) views.Add(view);
            UpdateControls();
        }

        public void ReduceLastCost()
        {
            if (views.Count == 0) return;
            var card = views[views.Count - 1].Data;
            card.SetCost(Mathf.Max(0, card.GetCardData().GetCost() - 1));
            UpdateControls();
        }

        public void ReturnLastCard()
        {
            if (views.Count == 0 || manager == null) return;
            int index = views.Count - 1;
            manager.DeleteCard(views[index]);
            views.RemoveAt(index);
            UpdateControls();
        }

        private void UpdateControls()
        {
            if (countLabel != null) countLabel.text = $"场上卡牌  {views.Count} / {maxVisibleCards}";
            if (spawnButton != null) spawnButton.interactable = testCard != null && manager != null && views.Count < maxVisibleCards;
            if (returnButton != null) returnButton.interactable = views.Count > 0;
            if (costButton != null) costButton.interactable = views.Count > 0 && views[views.Count - 1].Data.GetCardData().GetCost() > 0;
        }
    }
}
