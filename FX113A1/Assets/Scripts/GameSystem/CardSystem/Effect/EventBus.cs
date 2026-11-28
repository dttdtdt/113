using System.Collections.Generic;
using GameSystem.BattleSystem.Enum;
using EffectTrigger = GameSystem.CardSystem.Effect.Trigger.Trigger;

namespace GameSystem.CardSystem.Effect
{
    public class EventBus
    {
        private readonly Dictionary<TriggerTurn, List<EffectTrigger>> handlers = new();

        public void Subscribe(EffectTrigger trigger)
        {
            if (trigger == null) return;
            if (!handlers.TryGetValue(trigger.TriggerType, out var list))
            {
                list = new List<EffectTrigger>();
                handlers.Add(trigger.TriggerType, list);
            }
            if (!list.Contains(trigger)) list.Add(trigger);
        }

        public void Unsubscribe(EffectTrigger trigger)
        {
            if (trigger == null) return;
            // TriggerType 可变，退订时移除该实例的全部注册。
            foreach (var list in handlers.Values) list.Remove(trigger);
        }

        public void Invoke(TriggerTurn trigger)
        {
            if (!handlers.TryGetValue(trigger, out var list)) return;
            // 本次发布使用快照，回调中订阅/退订不会破坏遍历。
            foreach (var handler in list.ToArray()) handler.TriggerInvoke();
        }
    }
}
