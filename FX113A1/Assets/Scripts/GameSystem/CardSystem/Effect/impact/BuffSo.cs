using UnityEngine;

namespace GameSystem.CardSystem.Effect.impact
{
    [CreateAssetMenu(fileName = "Buff", menuName = "GameSystem/Buff")]
    public class BuffSo : Effect
    {
        [SerializeField] private string buffName;
        [SerializeField] private Texture2D icon;
        [SerializeField] private bool stackable;
        [SerializeField] private int maxStack;

        public string GetBuffName() => buffName;
        public Texture2D GetIcon() => icon;
        public bool GetStackable() => stackable;
        public int GetMaxStack() => maxStack;
    }
}
