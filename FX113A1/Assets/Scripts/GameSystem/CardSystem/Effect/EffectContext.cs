using System.Collections.Generic;
using GameSystem.BattleSystem.Enum;

namespace GameSystem.CardSystem.Effect
{
    public class EffectContext
    {
        public TriggerTurn Trigger { get; set; }
        public object Caster { get; set; }
        public List<object> Target { get; set; } = new();
        // 保留原型的 int 参数约定；与卡牌 float 参数的适配留待效果结算实现。
        public List<int> Value { get; set; } = new();
    }
}
