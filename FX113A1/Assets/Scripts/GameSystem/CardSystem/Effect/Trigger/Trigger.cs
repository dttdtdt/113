using System;
using GameSystem.BattleSystem.Enum;

namespace GameSystem.CardSystem.Effect.Trigger
{
    public class Trigger
    {
        public TriggerTurn TriggerType { get; set; }
        public EffectContext Context { get; set; }
        public Action<EffectContext> Action { get; set; }

        public void TriggerInvoke() => Action?.Invoke(Context);
    }
}
