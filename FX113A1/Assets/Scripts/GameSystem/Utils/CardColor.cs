using GameSystem.CardSystem.Card.Enum;
using UnityEngine;

namespace GameSystem.Utils
{
    public static class CardOccupationToColor
    {
        public static Color Get(CardOccupation occupation) => occupation switch
        {
            CardOccupation.Warrior => new Color(0.72f, 0.25f, 0.20f),
            CardOccupation.Ranger => new Color(0.20f, 0.48f, 0.34f),
            CardOccupation.Caster => new Color(0.28f, 0.38f, 0.68f),
            CardOccupation.Priest => new Color(0.66f, 0.51f, 0.20f),
            _ => new Color(0.45f, 0.45f, 0.45f)
        };
    }
}
