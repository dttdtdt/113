# Roguelike 卡牌系统设计文档

> 目标：设计一套适用于《杀戮尖塔》+《暗黑地牢》类型独立游戏的、可扩展、可复用、数据驱动的卡牌系统。
>
> 核心原则：**卡牌是数据，Effect 是规则，CardManager 负责生命周期，Combat 系统负责执行。**

---

# 1. 卡牌系统的职责

卡牌系统主要负责：

1. 定义一张卡牌是什么。
2. 定义卡牌什么时候可以打出。
3. 定义卡牌可以选择什么目标。
4. 定义卡牌打出后产生什么效果。
5. 定义卡牌的费用、稀有度、类型、标签。
6. 管理卡牌升级、强化、变形、复制、移除。
7. 管理抽牌堆、手牌、弃牌堆、消耗堆。
8. 为遗物、状态、敌人技能等其他系统提供统一的卡牌接口。

卡牌系统**不应该直接负责**：

- 播放动画
- 控制摄像机
- 播放音效
- 操作 UI
- 直接修改敌人 HP
- 直接决定战斗流程
- 处理地图
- 处理商店

这些事情应该交给其他系统。

---

# 2. 卡牌的基本模型

一张卡牌可以抽象成：

```text
Card
├── Identity
├── Cost
├── Type
├── Target
├── Rarity
├── Tags
├── Conditions
├── Effects
├── Keywords
├── Upgrade
├── Visual
└── Runtime State
```

其中：

- Identity：卡牌是谁
- Cost：打出需要什么资源
- Type：攻击、防御、技能、能力、诅咒等
- Target：作用于谁
- Rarity：稀有度
- Tags：构筑标签
- Conditions：打出限制
- Effects：打出后做什么
- Keywords：特殊规则
- Upgrade：升级后的变化
- Visual：美术表现
- Runtime State：战斗中的运行状态

---

# 3. CardDefinition 与 CardInstance 分离

这是卡牌系统非常重要的设计。

不要把“卡牌模板”和“某一次运行中的具体卡牌”混在一起。

## 3.1 CardDefinition

表示“这是什么卡”。

例如：

```text
CardDefinition
ID = slash
Name = 重击
Cost = 1
Type = Attack
Rarity = Common
BaseEffects = Damage(8)
```

它是静态数据。

## 3.2 CardInstance

表示“当前这一次 Run 中的这一张卡”。

例如：

```text
CardInstance
DefinitionID = slash
UpgradeLevel = 1
TemporaryCostModifier = -1
TemporaryDamageModifier = +3
UniqueID = 48A2D9
```

这解决了很多问题：

- 同一张卡可以有不同升级状态
- 某张卡可以被临时强化
- 某张卡可以被诅咒
- 某张卡可以拥有本局专属修改
- 存档时只保存 Instance 状态

---

# 4. 推荐的数据结构

可以先定义：

```csharp
public class CardDefinition
{
    public string Id;
    public string Name;
    public string Description;

    public int BaseCost;

    public CardType Type;
    public CardRarity Rarity;
    public TargetType TargetType;

    public List<CardTag> Tags;
    public List<EffectDefinition> Effects;
    public List<ConditionDefinition> Conditions;

    public CardUpgradeData UpgradeData;
}
```

运行时：

```csharp
public class CardInstance
{
    public string UniqueId;
    public string DefinitionId;

    public int UpgradeLevel;

    public List<Modifier> Modifiers;
}
```

---

# 5. 卡牌类型

不要只做“攻击 / 防御”。

建议至少：

```text
Attack
Skill
Power
Status
Curse
Quest
Special
```

## 5.1 Attack

直接产生攻击性结果。

例如：

```text
重击
穿刺
火球
横扫
连射
处刑
```

## 5.2 Skill

产生辅助效果，但不是持续型能力。

例如：

```text
格挡
治疗
抽牌
加能量
施加 Buff
施加 Debuff
移动
换位
```

## 5.3 Power

永久或持续存在的能力。

例如：

```text
战斗专注
鲜血仪式
毒素掌握
狂热
```

设计上可以：

```text
Play Card
↓
Apply Permanent Combat Status
```

## 5.4 Status

通常不是玩家正常获取的主动牌，而是系统、敌人或特殊效果产生的卡牌。

例如：

```text
Burn
Wound
Dazed
Stun
```

## 5.5 Curse

负面卡。

例如：

```text
伤口
恐惧
腐败
诅咒
```

可以提供：

- 无法打出
- 打出会受到伤害
- 占据手牌
- 污染牌组
- 降低资源

## 5.6 Special

不符合常规分类的特殊卡。

例如：

```text
变身
临时卡
Boss 卡
事件卡
召唤卡
```

---

# 6. 卡牌标签系统

类型不能表达完整构筑逻辑，因此需要 Tags。

推荐：

```text
Physical
Magical
Melee
Ranged
Defense
Attack
Bleed
Poison
Fire
Ice
Holy
Dark
Stress
Summon
Draw
Discard
Exhaust
Combo
Critical
Counter
Position
Risk
Reward
```

例如：

```text
毒刃

Type:
Attack

Tags:
Physical
Melee
Poison
Bleed
```

标签主要用于：

- 卡牌之间的联动
- 遗物触发
- 被动技能
- 套牌统计
- UI 展示
- 构筑系统

---

# 7. 为什么一定要有 Tags

例如有一件遗物：

> 每打出一张 Poison 卡，抽 1 张牌。

它不需要知道：

- 毒刃是什么
- 毒箭是什么
- 毒雾是什么

只需要：

```text
WHEN CardPlayed
IF Tag == Poison
THEN DrawCard(1)
```

这样新增卡牌时不需要修改遗物。

---

# 8. 卡牌费用系统

不要把 Cost 简单设计成一个 `int`。

建议：

```text
Cost
├── EnergyCost
├── HP Cost
├── Gold Cost
├── Discard Cost
├── Sacrifice Cost
├── Resource Cost
└── Special Requirement
```

例如：

```text
献祭
Cost:
Energy = 0
HP = 8
```

或者：

```text
血契
Cost:
Energy = 1
HP = 5
```

统一抽象为：

```text
CostDefinition
```

---

# 9. 多资源费用

推荐：

```csharp
public class CardCost
{
    public List<ResourceCost> Costs;
}
```

例如：

```json
{
    "costs": [
        {
            "resource": "Energy",
            "value": 2
        },
        {
            "resource": "HP",
            "value": 5
        }
    ]
}
```

这样未来加入：

- 法力
- 弹药
- 怒气
- 灵魂
- 行动力

都不用修改卡牌系统结构。

---

# 10. Target 系统

目标类型建议统一定义：

```text
Self
SingleAlly
AllAllies
SingleEnemy
AllEnemies
RandomEnemy
RandomAlly
LowestHP
HighestHP
All
Position
Area
None
```

再进一步：

```text
TargetSelector
```

例如：

```text
最低生命值友军
```

可以表示为：

```text
TargetSelector
→ Ally
→ Sort by HP
→ Lowest
```

---

# 11. Target 与 Effect 分离

卡牌不要把：

```text
DealDamageToEnemy
```

写死。

应该：

```text
TargetSelector
+
Effect
```

例如：

```text
Target:
SingleEnemy

Effect:
Damage(10)
```

这样同一个 Damage Effect 就可以用于：

- 单体敌人
- 全体敌人
- 随机敌人
- 自己
- 队友

---

# 12. Effect 系统

Effect 是卡牌系统最重要的组成部分。

推荐第一版实现以下 Effect：

## 12.1 基础效果

```text
Damage
Heal
Block
GainResource
LoseResource
DrawCard
DiscardCard
ExhaustCard
AddCard
RemoveCard
Shuffle
```

## 12.2 状态效果

```text
ApplyStatus
RemoveStatus
ModifyStatusStack
```

## 12.3 战斗效果

```text
ChangePosition
MoveTarget
Summon
Kill
ModifyIntent
ForceAction
SkipTurn
```

## 12.4 卡组效果

```text
DuplicateCard
UpgradeCard
DowngradeCard
TransformCard
DiscoverCard
```

---

# 13. Effect 参数化

不要为每一种数字效果建立独立类。

例如：

```text
DamageEffect
Amount = 10
```

可以进一步支持公式：

```text
Amount = BaseDamage + Strength * 2
```

或者：

```text
Amount = MissingHP * 0.1
```

或者：

```text
Amount = CardsPlayedThisTurn * 3
```

建议以后建立：

```text
ValueExpression
```

用于描述数值来源。

---

# 14. ValueExpression

推荐支持：

```text
Constant
Stat
Resource
StatusStacks
CardCount
HandSize
DiscardSize
DamageDealt
MissingHP
MaxHP
TurnNumber
EnemyCount
AllyCount
Random
```

例如：

```text
Damage = 8 + Strength × 2
```

可以表达成：

```text
Add
├── Constant(8)
└── Multiply
    ├── Stat(Strength)
    └── Constant(2)
```

这样设计之后，复杂卡牌几乎都能数据化。

---

# 15. Effect 的执行方式

推荐：

```text
Card
 ↓
Resolve Target
 ↓
Resolve Conditions
 ↓
Resolve Value
 ↓
Create Action
 ↓
ActionResolver
 ↓
GameState
 ↓
EventBus
```

例如：

```text
穿刺

1. 选择敌人
2. 判断敌人是否有效
3. 计算伤害
4. 产生 DamageAction
5. 产生 BleedAction
6. 广播事件
7. 触发遗物
8. 触发状态
```

---

# 16. Condition 条件系统

卡牌不仅要有 Effect，还要有 Condition。

建议支持：

```text
HasStatus
HasTag
HPAbove
HPBelow
ResourceAbove
ResourceBelow
CardCount
HandSize
DeckContains
EnemyCount
AllyCount
PositionCheck
TurnCheck
PreviousCard
CardsPlayedThisTurn
```

---

# 17. 条件示例

例如：

> 如果敌人已经流血，额外造成 10 点伤害。

可以设计为：

```text
Condition:
Target HasStatus(Bleed)

Effect:
Damage(10)
```

或者：

> 手牌中至少有 3 张攻击牌时，获得 1 点能量。

```text
Condition:
HandContainsTag(Attack, >= 3)

Effect:
GainEnergy(1)
```

---

# 18. Trigger 与 Card

卡牌效果可能触发额外效果：

```text
CardPlayed
↓
Effect
↓
Event
↓
Trigger
↓
Additional Effect
```

例如：

```text
卡牌：
攻击造成 8 点伤害。

遗物：
每打出攻击牌，获得 1 点护甲。

流程：

CardPlayed
 ↓
Relic Trigger
 ↓
GainBlock(1)
```

---

# 19. Keyword 系统

很多卡牌机制可以使用 Keyword 表达。

建议第一版加入：

```text
Exhaust
Retain
Innate
Ethereal
Unplayable
XCost
Temporary
Generated
Upgrade
Combo
Counter
Bleed
Poison
Stress
```

---

# 20. 常用关键词设计

## Exhaust

本回合打出后进入消耗堆。

```text
Play
 ↓
ExhaustPile
```

## Retain

回合结束不进入弃牌堆。

```text
TurnEnd
 ↓
Remain in Hand
```

## Innate

战斗开始时进入手牌。

## Ethereal

回合结束时如果仍在手牌，则自动消耗。

## XCost

消耗剩余全部能量，并根据能量决定效果。

```text
Damage = X × 5
```

---

# 21. 卡牌升级

卡牌升级不要只考虑：

```text
8 Damage
→
12 Damage
```

可以支持多种升级方式：

```text
Numeric Upgrade
Effect Upgrade
Cost Upgrade
Target Upgrade
Keyword Upgrade
Condition Upgrade
```

例如：

### 数值升级

```text
Damage:
8 → 12
```

### 费用升级

```text
Cost:
2 → 1
```

### 效果升级

```text
攻击
↓
攻击 + 抽牌
```

### 标签升级

```text
Attack
↓
Attack + Bleed
```

---

# 22. UpgradeDefinition

例如：

```json
{
    "upgrade": {
        "cost": -1,
        "damage": +2,
        "addTags": [
            "Bleed"
        ]
    }
}
```

或者：

```json
{
    "upgrade": {
        "addEffects": [
            {
                "type": "DrawCard",
                "value": 1
            }
        ]
    }
}
```

---

# 23. 卡牌稀有度

建议：

```text
Basic
Common
Uncommon
Rare
Epic
Legendary
Curse
```

如果接近《杀戮尖塔》的结构，可以简化为：

```text
Basic
Common
Uncommon
Rare
Curse
Status
```

稀有度主要影响：

- 出现概率
- 卡牌强度
- 构筑方向
- 商店价值
- 事件奖励

而不是简单决定“越稀有越强”。

---

# 24. 稀有度与设计定位

### Basic

稳定、简单、教学用途。

### Common

构筑基础。

### Uncommon

建立套路。

### Rare

改变构筑方向。

### Legendary

改变游戏规则。

例如：

```text
Common
+2 攻击

Uncommon
毒流核心

Rare
所有毒伤害翻倍

Legendary
每次施加毒时，敌人失去最大生命 1%
```

---

# 25. 卡牌设计的三个层次

设计卡牌时最好分三个层次：

## 第一层：即时收益

例如：

```text
攻击 8
格挡 10
抽 2
治疗 5
```

## 第二层：构筑联动

例如：

```text
如果你有毒状态，则...
```

## 第三层：改变规则

例如：

```text
本场战斗中，每施加 3 层毒，再施加一次额外毒。
```

Rare / Boss / Build 核心卡通常应该更多集中在第二、第三层。

---

# 26. 卡牌的构筑定位

每张卡都应该有一个明确的“职责”。

建议分为：

```text
Basic Damage
Burst Damage
Sustained Damage
Defense
Draw
Energy
Scaling
Setup
Payoff
Control
Utility
Position
Recovery
Finish
```

例如：

```text
毒箭

类型：
Attack

构筑定位：
Setup

作用：
给敌人叠毒
```

而：

```text
腐蚀爆发

类型：
Damage

构筑定位：
Payoff

作用：
引爆毒
```

这样可以形成：

```text
Setup
 ↓
Engine
 ↓
Payoff
```

---

# 27. 卡组 Archetype

设计卡牌时不要一张一张孤立地设计。

先设计 Archetype。

例如：

```text
Bleed
Poison
Critical
Block
Counter
Stress
Summon
Discard
Exhaust
Combo
Position
Risk / Reward
```

每个 Archetype 至少需要：

```text
Setup Cards
Payoff Cards
Engine Cards
Defense Cards
Utility Cards
Scaling Cards
```

---

# 28. 一个完整 Archetype 示例：流血

## Setup

```text
割伤
攻击 5
施加 Bleed 2
```

## Engine

```text
鲜血追猎
对流血目标攻击时，额外造成 4 点伤害。
```

## Payoff

```text
血爆
消耗目标所有 Bleed。
每消耗 1 层造成 3 点伤害。
```

## Defense

```text
浴血
获得 8 格挡。
每有 1 个流血敌人，再获得 1 格挡。
```

## Scaling

```text
血债
敌人的 Bleed 层数越高，你造成的伤害越高。
```

---

# 29. 卡牌不是越强越好

真正好的卡牌应该有：

```text
收益
+
代价
+
条件
+
机会成本
```

例如：

```text
超载

2 能量

造成 20 点伤害。

代价：
丢弃一张牌。
```

它不只是“20 伤害”，而是：

```text
高收益
+
高费用
+
手牌代价
```

这样才会产生决策。

---

# 30. 卡牌设计的核心：决策而不是数值

优秀卡牌应该问玩家：

> “现在用，还是留到以后？”

例如：

```text
强击
1 能量
12 伤害
```

决策很少。

而：

```text
血契
0 能量
失去 6 HP
获得 2 能量
```

就会产生：

- 现在缺能量吗？
- 我还有多少 HP？
- 后面有没有 Boss？
- 这个回合值得冒险吗？

---

# 31. 卡牌的机会成本

卡牌强度不能只看：

```text
Damage / Cost
```

还要考虑：

```text
Hand Slot
Draw Opportunity
Deck Space
Timing
Target Restriction
Risk
Build Requirement
```

例如一个很强的卡牌，如果：

```text
抽到率很低
需要特定条件
占用手牌
不能每回合使用
```

依然可能是平衡的。

---

# 32. Card Power Budget

可以给每张卡建立一个简单的 Power Budget：

```text
基础效果价值
+
额外效果价值
+
联动价值
-
费用
-
限制
-
风险
```

例如：

```text
1 Energy Attack

基础：
8 Damage

额外：
造成 Bleed 2

限制：
只能攻击 Bleed 目标

风险：
无

最终强度：
需要根据整个 Archetype 调整。
```

这不是精确数学，而是一种统一设计思路。

---

# 33. 初始卡组设计

第一局不应该让玩家一开始就拥有完整玩法。

推荐：

```text
基础攻击 × 4
基础防御 × 4
职业特色卡 × 2
```

例如：

```text
8 张基础卡
+
2 张职业卡
=
10 张起始牌组
```

然后通过战斗、事件、商店逐渐构筑。

---

# 34. 卡牌奖励

每场战斗结束后：

```text
RewardGenerator
```

生成：

```text
Gold
Card
Potion
Relic
Upgrade
Remove
```

卡牌奖励建议：

```text
Common 权重最高
Uncommon 次之
Rare 最低
```

Boss 奖励可以：

```text
3 张 Rare 卡三选一
```

---

# 35. Discover 系统

推荐把：

> “从多个选项中选择一张牌”

统一设计为：

```text
DiscoverCardsEffect
```

例如：

```text
Discover 3
 ↓
玩家选择 1
 ↓
Add to Hand
```

以后可以复用到：

- 随机事件
- 卡牌效果
- Boss
- 遗物
- 商店

---

# 36. Transform 系统

一些卡牌可以：

```text
Card A
 ↓
Transform
 ↓
Card B
```

比如：

```text
普通攻击
 ↓
狂暴攻击
```

事件可以：

```text
Transform random card
```

因此需要：

```text
TransformCardEffect
```

---

# 37. Copy / Duplicate 系统

不要直接复制对象。

应该：

```text
DuplicateCardEffect
```

根据：

```text
CardDefinition ID
+
CardInstance State
```

生成新的 CardInstance。

这样升级状态、临时修改都能正确处理。

---

# 38. Remove / Purge 系统

卡组成长不只是增加卡。

必须支持：

```text
Add
Upgrade
Transform
Remove
Duplicate
Downgrade
Curse
```

因为 Roguelike 卡牌游戏的核心其实是：

> **改变卡组结构。**

---

# 39. Card Pool

每个职业建议有：

```text
Basic Pool
Class Pool
Rare Pool
Special Pool
Curse Pool
Status Pool
```

进一步：

```text
CardPoolManager
```

根据：

```text
Character
Act
Rarity
Event
Shop
Boss
```

决定可以出现哪些卡牌。

---

# 40. 职业专属卡牌

例如一个“鲜血骑士”职业：

```text
Common
├── 流血攻击
├── 自残防御
└── 血祭

Uncommon
├── 血怒
├── 猎杀
└── 血债

Rare
├── 血神降临
├── 千刃血狱
└── 不死血契
```

职业卡的重点不是：

> “这个职业的数字更高。”

而是：

> “这个职业的卡牌定义了这个职业怎么玩。”

---

# 41. 通用卡与职业卡

推荐：

```text
Common Pool
+
Character Pool
+
Rare Pool
```

例如：

```text
通用卡：
格挡
抽牌
小治疗
基础攻击

战士：
怒气
反击
重击
流血

盗贼：
毒
暴击
弃牌
连击

法师：
元素
法力
法术
引爆
```

这样可以形成：

```text
共享规则
+
职业身份
```

---

# 42. 卡牌与位置系统

如果加入《暗黑地牢》式位置：

```text
Position
1
2
3
4
```

那么卡牌需要加入：

```text
AllowedTargetPositions
AllowedSelfPositions
```

例如：

```text
长枪突刺

自身位置：
1 / 2

敌人目标：
1 / 2 / 3
```

而：

```text
后排射击

自身位置：
3 / 4

敌人目标：
2 / 3 / 4
```

这样“位置”本身就成为构筑的一部分。

---

# 43. 卡牌与压力系统

如果有《暗黑地牢》式 Stress：

卡牌可以：

```text
IncreaseStress
DecreaseStress
StressScaling
StressThreshold
```

例如：

```text
恐惧冲击

造成 6 伤害
施加 8 Stress

如果目标 Stress > 50：
额外造成 10 伤害
```

这就形成：

```text
Stress Setup
 ↓
Stress Engine
 ↓
Stress Payoff
```

---

# 44. 卡牌与风险收益

非常适合独立游戏。

例如：

```text
献血

失去 10 HP
获得 2 能量
抽 2 张牌
```

或者：

```text
孤注一掷

本回合所有攻击伤害 +50%

回合结束：
失去 15 HP
```

这种卡牌非常适合构建游戏的核心风格。

---

# 45. 临时卡牌

推荐增加：

```text
TemporaryCard
```

例如：

```text
Combo
Wrath
Counter
Bleed Explosion
```

这些卡牌：

```text
来源：
其他卡牌

特点：
临时
战斗结束消失
```

这样可以减少玩家卡组污染。

---

# 46. Card Lifecycle

一张卡牌从获取到使用：

```text
Create
 ↓
Deck
 ↓
Draw
 ↓
Hand
 ↓
Playable Check
 ↓
Target Selection
 ↓
Pay Cost
 ↓
Execute Effects
 ↓
Trigger Events
 ↓
Discard / Exhaust
```

这是卡牌系统的核心生命周期。

---

# 47. Playability Check

玩家点击卡牌之前，需要：

```text
CanPlayCard
```

判断：

```text
Cost
Target
Position
Conditions
Status
Silence
Card State
Combat State
```

例如：

```text
if Energy < Cost
    CannotPlay

if NoValidTarget
    CannotPlay

if ConditionFailed
    CannotPlay
```

UI 只读取：

```text
CanPlayCardResult
```

不要自己重复写规则。

---

# 48. Card Play Result

建议标准化返回：

```text
CardPlayResult

Success
Failure

FailureReason
```

例如：

```text
NotEnoughEnergy
NoValidTarget
InvalidPosition
ConditionNotMet
Silenced
CardLocked
```

这样 UI 可以直接显示原因。

---

# 49. 卡牌描述生成

不要把描述写死。

推荐：

```text
DescriptionTemplate
+
Resolved Values
```

例如：

```text
造成 {Damage} 点伤害。
施加 {Bleed} 层流血。
```

运行时：

```text
造成 12 点伤害。
施加 3 层流血。
```

这样升级后自动更新描述。

---

# 50. Description 不应该负责游戏逻辑

UI 文本只是展示：

```text
Effect Data
 ↓
Description Generator
 ↓
Card UI
```

真正执行的仍然是：

```text
Effect
```

这样不会出现：

> UI 写“造成 12 点伤害”，实际代码却造成 10 点。

---

# 51. 卡牌设计数据库

建议后续用以下任一种作为数据源：

```text
JSON
CSV
ScriptableObject
YAML
```

例如：

```text
Cards/
├── warrior/
│   ├── strike.json
│   ├── heavy_strike.json
│   └── blood_rage.json
│
├── rogue/
│   ├── poison_knife.json
│   └── backstab.json
│
└── common/
    ├── defend.json
    └── draw.json
```

---

# 52. 一张完整卡牌示例

下面设计一张完整卡牌：

```json
{
    "id": "blood_lance",
    "name": "血枪",
    "type": "Attack",
    "rarity": "Common",
    "cost": {
        "energy": 1
    },
    "target": {
        "type": "SingleEnemy"
    },
    "tags": [
        "Physical",
        "Melee",
        "Bleed"
    ],
    "effects": [
        {
            "type": "Damage",
            "value": 8
        },
        {
            "type": "ApplyStatus",
            "status": "Bleed",
            "value": 2
        }
    ],
    "upgrade": {
        "damage": 11,
        "bleed": 3
    }
}
```

---

# 53. 卡牌设计模板

今后设计任何一张牌，可以固定使用这个模板：

```text
# 卡牌名称

## 基础信息

ID:
职业:
类型:
稀有度:
费用:

## 目标

目标类型:
目标数量:
位置限制:

## 标签

- Attack
- Physical
- Bleed

## 核心效果

1.
2.
3.

## 条件

1.
2.

## 关键词

- Exhaust
- Retain

## 升级

基础:
升级:

## 构筑定位

- Setup
- Engine
- Payoff
- Defense
- Utility

## 风险 / 代价

## 适合的卡组

## 克制关系

## 设计目的
```

---

# 54. 一张卡设计时必须回答的问题

每张卡都建议回答：

### 1. 它解决什么问题？

```text
输出？
防御？
抽牌？
资源？
控制？
构筑？
```

### 2. 它为什么值得加入卡组？

如果答案只是：

> 数值更高

通常设计不够好。

### 3. 它和哪些卡联动？

例如：

```text
Bleed
Poison
Discard
Critical
```

### 4. 它什么时候最强？

### 5. 它什么时候很差？

### 6. 玩家需要做什么决策？

---

# 55. 卡牌设计的黄金标准

理想情况下，一张卡应该满足：

```text
容易理解
+
容易使用
+
有明确用途
+
存在选择空间
+
能够与其他卡联动
+
不会在所有构筑中都最优
```

---

# 56. 推荐的第一套卡牌数量

第一个可玩 Demo：

```text
通用卡：
10~15

职业卡：
20~30

稀有卡：
5~10

诅咒：
3~5

状态卡：
5~10
```

不要第一版本就做 200 张卡。

先验证：

> **20~40 张牌能否形成有趣的构筑。**

---

# 57. 第一套卡组建议

假设第一个职业是“血骑士”。

可以先做 25 张：

```text
基础攻击
4

基础防御
4

流血体系
6

自残体系
4

怒气体系
3

抽牌/资源
2

Rare
2
```

这样足够测试：

```text
基础玩法
+
Build
+
Synergy
+
Scaling
```

---

# 58. 第一版 Card Framework 最小功能

程序层面，第一版只需要实现：

```text
CardDefinition
CardInstance

CardType
CardRarity
CardTag
TargetType

Cost
Condition
Effect

DrawPile
Hand
DiscardPile
ExhaustPile

PlayCard
DrawCard
DiscardCard
ExhaustCard

UpgradeCard
TransformCard
RemoveCard
DuplicateCard
```

等这些稳定后，再加入：

```text
Keyword
Complex Value
Trigger
Modifier
Advanced Target
Position
```

---

# 59. 推荐实现优先级

```text
Phase 1
├── CardDefinition
├── CardInstance
├── CardType
├── Cost
└── Basic Effect

Phase 2
├── Deck
├── Hand
├── Draw
├── Discard
└── Exhaust

Phase 3
├── Target
├── Condition
└── Playability

Phase 4
├── Status
├── Trigger
├── Keyword
└── Modifier

Phase 5
├── Upgrade
├── Transform
├── Duplicate
├── Discover
└── Remove

Phase 6
├── Position
├── Stress
├── Advanced Value
└── Complex Synergy
```

---

# 60. 最终卡牌系统结构

最终建议形成：

```text
                      CardDefinition
                            │
                            ↓
                      CardInstance
                            │
                      ┌─────┴─────┐
                      ↓           ↓
                   Cost       Playability
                                    │
                       ┌────────────┼────────────┐
                       ↓            ↓            ↓
                    Target      Condition      Keyword
                       │            │            │
                       └────────────┼────────────┘
                                    ↓
                                  Effect
                                    │
                                    ↓
                                  Action
                                    │
                                    ↓
                                GameState
                                    │
                                    ↓
                                EventBus
                                    │
                    ┌───────────────┼───────────────┐
                    ↓               ↓               ↓
                  Relic           Status           Trigger
```

---

# 61. 最核心的设计原则

整个卡牌系统可以浓缩成：

> **Card 决定“是什么”，Cost 决定“代价”，Target 决定“对谁”，Condition 决定“何时能用”，Effect 决定“发生什么”，Keyword 决定“特殊规则”，Trigger 决定“如何与其他系统联动”。**

因此：

```text
一张卡
=
Identity
+
Cost
+
Target
+
Condition
+
Effect
+
Keyword
+
Upgrade
+
Tags
```

而不是：

```text
一张卡
=
一个巨大的 C# 类
```

---

# 62. 下一步开发建议

当卡牌系统文档确定之后，实际开发建议按下面顺序实现：

```text
1. CardDefinition
2. CardInstance
3. Deck
4. Hand
5. Draw / Discard / Exhaust
6. Cost System
7. Target System
8. Effect System
9. Condition System
10. Status System
11. Trigger System
12. Upgrade System
13. Reward System
14. Card Pool
15. Card Editor
```

其中最值得优先做的是：

```text
CardDefinition
        ↓
Effect
        ↓
Target
        ↓
Condition
        ↓
CardManager
```

先让：

> **“一张牌可以被定义、被抽到、被选择、被打出、产生效果、进入弃牌堆”**

完整跑通。

之后再加入遗物、状态、位置、压力和复杂构筑。

---

# 63. 最终目标

最终理想状态是：

```text
新增一张卡
=
创建一个 CardData
+
配置几个 Effect
+
选择 Target
+
添加 Tags
+
配置 Upgrade
```

而不是：

```text
新增一张卡
=
写新代码
+
修改 Combat
+
修改 UI
+
修改 Enemy
+
修改 Status
+
修改 Save
```

这才是真正可复用的卡牌游戏框架。
