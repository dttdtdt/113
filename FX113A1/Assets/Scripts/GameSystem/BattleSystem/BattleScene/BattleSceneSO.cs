using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.BattleSystem.BattleScene
{
    [CreateAssetMenu(fileName = "BattleScene", menuName = "GameSystem/Battle Scene")]
    public class BattleSceneSO : ScriptableObject
    {
        // 用 GameObject 表达可序列化的初始占位引用，角色业务类型留待后续实现。
        [SerializeField] private List<GameObject> playerPosition = new();
        [SerializeField] private List<GameObject> enemyPosition = new();

        public IReadOnlyList<GameObject> GetPlayerPosition() => playerPosition.AsReadOnly();
        public IReadOnlyList<GameObject> GetEnemyPosition() => enemyPosition.AsReadOnly();

        public void SetPlayerPosition(IEnumerable<GameObject> value)
            => playerPosition = value == null ? new() : new(value);
        public void SetEnemyPosition(IEnumerable<GameObject> value)
            => enemyPosition = value == null ? new() : new(value);
    }
}
