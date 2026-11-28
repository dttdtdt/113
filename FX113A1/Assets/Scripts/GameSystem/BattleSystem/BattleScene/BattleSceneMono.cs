using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.BattleSystem.BattleScene
{
    public class BattleSceneMono : MonoBehaviour
    {
        [SerializeField] private BattleSceneSO sceneData;
        private List<GameObject> player = new();
        private List<GameObject> enemy = new();

        public IReadOnlyList<GameObject> Player => player.AsReadOnly();
        public IReadOnlyList<GameObject> Enemy => enemy.AsReadOnly();

        private void Awake()
        {
            if (sceneData != null) Initialize(sceneData);
        }

        public bool Initialize(BattleSceneSO data)
        {
            if (data == null) return false;
            sceneData = data;
            // 复制列表，站位修改不写回共享配置；不在此处实例化角色。
            player = new List<GameObject>(data.GetPlayerPosition());
            enemy = new List<GameObject>(data.GetEnemyPosition());
            return true;
        }

        public void SetCharacter(int position, GameObject character)
        {
            if (position < 0 || position >= player.Count) return;
            player[position] = character;
        }

        public void ExchangePosition(int position1, int position2)
        {
            if (position1 < 0 || position1 >= player.Count || position2 < 0 || position2 >= player.Count) return;
            (player[position1], player[position2]) = (player[position2], player[position1]);
        }
    }
}
