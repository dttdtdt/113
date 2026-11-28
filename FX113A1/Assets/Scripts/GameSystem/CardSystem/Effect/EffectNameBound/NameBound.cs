using System.Collections.Generic;
using GameSystem.CardSystem.Card.Enum;

namespace GameSystem.CardSystem.Effect.EffectNameBound
{
    public static class NameBound
    {
        public delegate bool EffectHandler(EffectContext context);

        // 原型尚未实现具体效果函数，此处仅迁移注册入口。
        public static readonly Dictionary<CardEffect, EffectHandler> Bounds = new();
    }
}
