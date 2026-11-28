using GameSystem.CardSystem.Card.Enum;
using UnityEngine;
using EffectCondition = GameSystem.CardSystem.Effect.Condition.SO.Condition;

namespace GameSystem.CardSystem.Effect.impact
{
    [CreateAssetMenu(fileName = "Effect", menuName = "GameSystem/Effect")]
    public class Effect : ScriptableObject
    {
        [SerializeField] private EffectCondition condition;
        [SerializeField] private CardEffect cardEffect;
        [SerializeField] private Effect effect;

        public EffectCondition GetCondition() => condition;
        public CardEffect GetCardEffect() => cardEffect;
        public Effect GetEffect() => effect;
    }
}
