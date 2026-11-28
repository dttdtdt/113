using GameSystem.BattleSystem.Enum;
using UnityEngine;

namespace GameSystem.CardSystem.Effect.Condition.SO
{
    [CreateAssetMenu(fileName = "Condition", menuName = "GameSystem/Condition")]
    public class Condition : ScriptableObject
    {
        [SerializeField] private int duration = -1; // -1 表示永久，消耗时序尚未实现。
        [SerializeField] private TriggerTurn trigger;

        public int Duration() => duration;
        public TriggerTurn GetTrigger() => trigger;
    }
}
