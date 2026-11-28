using System;
using System.Collections.Generic;
using GameSystem.CardSystem.Card.So;
using EffectData = GameSystem.CardSystem.Effect.impact.Effect;

namespace GameSystem.CardSystem.Card.Entity
{
    public class CardDataRuntime
    {
        private readonly Dictionary<EffectData, CardAttributeData> cardProperty = new();

        private int cost;

        public int GetCost() => cost;

        public void SetCost(int value) => cost = value;

        //查询单个效果数据
        public bool TryGetEffect(EffectData key, out CardAttributeData data)
        {
            data = null;
            return key != null && cardProperty.TryGetValue(key, out data);
        }

        public void ResetAllEffects()
        {
            foreach (var data in cardProperty.Values) data.Reset();
        }

        public void RecalculateAll()
        {
            foreach (var data in cardProperty.Values) data.SetAllCurrent();
        }

        public CardDataRuntime(CardSo template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            foreach(var (key, value) in template.GetEffects())
            {
                cardProperty[key] = new CardAttributeData(new List<float>(value));
            }
            cost = template.GetCost();

        }
    }
}
