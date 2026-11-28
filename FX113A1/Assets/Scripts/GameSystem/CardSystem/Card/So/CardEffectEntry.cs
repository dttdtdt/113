using System;
using System.Collections.Generic;
using UnityEngine;
using EffectData = GameSystem.CardSystem.Effect.impact.Effect;

namespace GameSystem.CardSystem.Card.So
{
    [Serializable]
    public class CardEffectEntry
    {
        [SerializeField] private EffectData effect;
        [SerializeField] private List<float> values = new();

        public CardEffectEntry(EffectData effect, IEnumerable<float> values)
        {
            this.effect = effect;
            this.values = values == null ? new List<float>() : new List<float>(values);
        }

        public EffectData GetEffect() => effect;
        public IReadOnlyList<float> GetValues() => values ?? (IReadOnlyList<float>)Array.Empty<float>();
    }
}
