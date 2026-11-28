using System.Collections.Generic;
using GameSystem.CardSystem.Card.Enum;
using EffectData = GameSystem.CardSystem.Effect.impact.Effect;
using UnityEngine;

namespace GameSystem.CardSystem.Card.So
{
    [CreateAssetMenu(fileName = "Card", menuName = "GameSystem/Card")]
    public class CardSo : ScriptableObject
    {
        [SerializeField] private string cardName;
        [SerializeField] private Texture2D cardImage;
        [SerializeField] private CardType cardType;
        [SerializeField] private CardOccupation occupation;
        [SerializeField] private Rarity cardRarity;
        [SerializeField] private int cost;
        // Unity 使用条目列表保存效果配置，运行时再转换为查询字典。
        [SerializeField] private List<CardEffectEntry> effects = new();
        [SerializeField] private List<string> description = new();
        [SerializeField] private bool battleOrManage;
        [SerializeField] private bool temp;
        [SerializeField] private OutOrIn cardClass;

        public string GetCardName() => cardName;
        public void SetCardName(string value) => cardName = value;
        public Texture2D GetCardImage() => cardImage;
        public void SetCardImage(Texture2D value) => cardImage = value;
        public CardType GetCardType() => cardType;
        public void SetCardType(CardType value) => cardType = value;
        public CardOccupation GetOccupation() => occupation;
        public void SetOccupation(CardOccupation value) => occupation = value;
        public Rarity GetCardRarity() => cardRarity;
        public void SetCardRarity(Rarity value) => cardRarity = value;
        public int GetCost() => cost;
        public void SetCost(int value) => cost = value;
        public List<string> GetDescription() => new(description ?? new List<string>());
        public void SetDescription(List<string> value) => description = value == null ? new() : new(value);
        public bool GetBattleOrManage() => battleOrManage;
        public void SetBattleOrManage(bool value) => battleOrManage = value;
        public bool GetTemp() => temp;
        public void SetTemp(bool value) => temp = value;
        public OutOrIn GetCardClass() => cardClass;
        public void SetCardClass(OutOrIn value) => cardClass = value;

        public Dictionary<EffectData, List<float>> GetEffects()
        {
            var result = new Dictionary<EffectData, List<float>>();
            if (effects == null) return result;
            foreach (var entry in effects)
            {
                if (entry == null || entry.GetEffect() == null) continue;
                result[entry.GetEffect()] = new List<float>(entry.GetValues());
            }
            return result;
        }

        public void SetEffects(Dictionary<EffectData, List<float>> value)
        {
            effects = new List<CardEffectEntry>();
            if (value == null) return;
            foreach (var pair in value)
            {
                if (pair.Key != null) effects.Add(new CardEffectEntry(pair.Key, pair.Value));
            }
        }
    }
}
