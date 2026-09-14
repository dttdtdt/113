# Roguelike 卡牌独立游戏可复用框架设计

## 一、项目定位

目标是制作一个结合：

- 《杀戮尖塔》的卡牌构筑、随机地图、遗物、事件、Boss
- 《暗黑地牢》的队伍、角色位置、状态、压力、职业体系
- Roguelike 的随机生成、Build 构筑、永久成长

的独立游戏。

核心设计原则：

> **数据驱动 + 状态机 + 事件总线 + Effect 效果系统**

最终目标：

> **框架负责“规则怎么运行”，数据负责“游戏里有什么”。**

这样增加新角色、新卡牌、新敌人、新遗物、新事件时，尽可能只增加数据，而不是修改核心代码。

---

# 二、总体架构

整个游戏建议拆成 8 层：

```text
┌────────────────────────────────────┐
│          Presentation Layer        │
│ UI / Animation / VFX / Audio       │
├────────────────────────────────────┤
│             Game Flow              │
│ MainMenu / Map / Battle / Event    │
├────────────────────────────────────┤
│          Roguelike System          │
│ Run / Map / Reward / Shop / Event  │
├────────────────────────────────────┤
│            Combat System           │
│ Turn / Action / Target / Queue     │
├────────────────────────────────────┤
│             Effect System          │
│ Damage / Heal / Buff / Debuff      │
├────────────────────────────────────┤
│              Rule System           │
│ Status / Trigger / Condition       │
├────────────────────────────────────┤
│              Data System           │
│ Card / Enemy / Relic / Character   │
├────────────────────────────────────┤
│        Save / Random / Utility     │
└────────────────────────────────────┘
```

核心数据流：

```text
Data
  ↓
Rules
  ↓
Effects
  ↓
Combat
  ↓
GameState
  ↓
Presentation
```

---

# 三、整体游戏循环

```text
启动
 ↓
主菜单
 ↓
新建 Run
 ↓
生成地图
 ↓
选择节点
 ↓
┌───────────────┐
│ 普通战斗       │
│ 精英战斗       │
│ Boss           │
│ 随机事件       │
│ 商店           │
│ 休息点         │
│ 宝藏           │
└───────────────┘
 ↓
奖励
 ↓
继续地图
 ↓
章节 Boss
 ↓
下一章节
 ↓
最终 Boss
 ↓
结算
 ↓
死亡 / 胜利
 ↓
Meta Progression
 ↓
重新开始
```

整个 Run 建议由 `RunManager` 统一管理。

---

# 四、核心设计原则：卡牌不要自己执行逻辑

## 4.1 错误设计

不要让每一张卡牌拥有大量独立逻辑：

```csharp
class Fireball : Card
{
    void Play()
    {
        enemy.hp -= 10;
        player.energy -= 1;
        enemy.ApplyBurn();
    }
}
```

这种设计在卡牌数量增加以后很快会失控。

## 4.2 推荐设计

卡牌应该只是：

- 数据
- Target
- Cost
- Effect 列表
- Tags
- Upgrade 数据

例如：

```text
Fireball
├── Cost = 1
├── Type = Attack
├── Target = SingleEnemy
│
└── Effects
    ├── DealDamage(10)
    └── ApplyBurn(2)
```

逻辑统一交给：

```text
Card
 ↓
Effect[]
 ↓
EffectSystem
 ↓
GameState
```

---

# 五、Card 数据结构

卡牌建议至少包含：

```text
CardDefinition

ID
Name
Description

Cost
CardType
TargetType

Rarity
Class
Tags

Effects[]

UpgradeData

Animation
Sound
Art
```

例如：

```json
{
    "id": "warrior_slash",
    "name": "重击",
    "cost": 1,
    "type": "Attack",
    "target": "SingleEnemy",
    "rarity": "Common",
    "tags": [
        "Physical",
        "Melee"
    ],
    "effects": [
        {
            "type": "Damage",
            "value": 8
        }
    ]
}
```

升级可以写成：

```json
{
    "upgrade": {
        "damage": 12,
        "cost": 1
    }
}
```

这样可以做到真正的数据驱动。

---

# 六、Effect 系统

Effect 是整个框架最核心的部分。

所有游戏行为尽量统一抽象为 Effect。

## 6.1 基础 Effect

```text
Damage
Heal
Block
DrawCard
GainEnergy
LoseEnergy

ApplyBuff
ApplyDebuff
RemoveStatus

AddCard
RemoveCard
DiscardCard
ExhaustCard

Move
Pull
Push

ChangeStress
ChangeSanity

Summon
Kill

ModifyCost
ModifyDamage

AddPower
RemovePower
```

## 6.2 卡牌示例

### 火球

```text
Fireball

Effects:
├── Damage(10)
└── ApplyBurn(2)
```

### 吸血攻击

```text
Vampire Strike

Effects:
├── Damage(8)
└── Heal(Self, DamageDealt × 0.5)
```

### 恐惧

```text
Terrify

Effects:
├── Damage(4)
└── ApplyStress(3)
```

---

# 七、Action 系统

建议 Effect 不直接修改 GameState。

采用：

```text
Card
 ↓
Effect
 ↓
Action
 ↓
GameState
```

例如：

```text
DealDamageEffect

输入：
Source
Target
Amount

产生：
DamageAction
```

然后由 `ActionResolver` 统一处理。

这样可以方便加入：

- 护甲
- 格挡
- 易伤
- 减伤
- 暴击
- 抗性
- 反伤
- 死亡触发
- 遗物效果

---

# 八、Event Bus 事件系统

建议建立统一的 `EventBus`，用于广播游戏事件。

## 8.1 常见事件

```text
CombatStarted
CombatEnded

TurnStarted
TurnEnded

CardPlayed
CardDiscarded
CardDrawn

DamageBefore
DamageAfter

UnitHealed

UnitDied

StatusApplied
StatusRemoved

EnemyIntentChanged

PlayerHealthChanged

NodeEntered
RewardGenerated

ShopOpened
```

---

# 九、Trigger 系统

Trigger 可以统一抽象为：

```text
WHEN
+
CONDITION
+
EFFECT
```

例如：

```text
WHEN CardPlayed
IF Card.Tag == Poison
THEN DrawCard(1)
```

或者：

```text
WHEN UnitDamaged
IF Damage >= 10
THEN ApplyBuff("Rage", 1)
```

这样：

- 遗物
- 被动技能
- 装备
- 敌人被动
- 地图机制
- 角色天赋

都可以共用同一个系统。

## 9.1 Trigger 数据结构

```json
{
    "trigger": "CardPlayed",
    "condition": {
        "tag": "Poison"
    },
    "effects": [
        {
            "type": "DrawCard",
            "value": 1
        }
    ]
}
```

核心思想：

> **Trigger 是“什么时候”，Condition 是“满足什么条件”，Effect 是“发生什么”。**

---

# 十、Status 状态系统

状态系统是《杀戮尖塔》和《暗黑地牢》类型游戏的核心。

统一建立 `Status`。

基础状态例如：

```text
Poison
Bleed
Burn
Weak
Vulnerable
Strength
Armor
Dodge
Stun
Fear
Stress
Mark
Root
Silence
```

每个状态包含：

```text
ID
Stacks
Duration
Owner
Tags
Triggers
```

例如：

```text
Poison

Stacks = 6

TurnEnd
 ↓
DealDamage(6)
 ↓
Stacks -= 1
```

## 10.1 Status 不要使用大量 if/else

错误：

```csharp
if (poison) ...
if (bleed) ...
if (burn) ...
if (weak) ...
if (fear) ...
```

推荐：

```text
StatusManager
    ↓
Status
    ├── Poison
    ├── Bleed
    ├── Burn
    ├── Weak
    └── Fear
```

每个 Status 自己声明自己监听哪些事件，以及事件发生时产生什么效果。

---

# 十一、Combat System

战斗必须设计成明确的状态机。

## 11.1 传统回合制

```text
CombatStart
 ↓
PlayerTurnStart
 ↓
PlayerAction
 ↓
PlayerTurnEnd
 ↓
EnemyTurnStart
 ↓
EnemyAction
 ↓
EnemyTurnEnd
 ↓
CheckBattleEnd
 ↓
PlayerTurnStart
```

## 11.2 多角色队伍模式

如果采用《暗黑地牢》式 4 人队伍：

```text
Round
 ↓
Calculate Initiative
 ↓
Unit 1
 ↓
Unit 2
 ↓
Unit 3
 ↓
Unit 4
 ↓
Enemy 1
 ↓
Enemy 2
 ↓
Enemy 3
 ↓
End Round
```

也可以根据速度生成 `TurnQueue`。

---

# 十二、Character 系统

角色和战斗系统分开。

角色：

```text
Character
├── Stats
├── Skills
├── Statuses
├── Equipment
└── Resources
```

战斗：

```text
CombatManager
├── CombatState
├── TurnQueue
├── ActionResolver
├── TargetResolver
└── EventBus
```

这样角色不会和某一个战斗场景强耦合。

---

# 十三、角色属性系统

基础属性：

```text
HP
MaxHP

Energy
MaxEnergy

Strength
Dexterity
Speed

Defense
Resistance

CritChance
CritDamage
```

最终数值通过 Modifier 计算：

```text
Base Value
 ↓
Equipment Modifier
 ↓
Buff Modifier
 ↓
Debuff Modifier
 ↓
Temporary Modifier
 ↓
Final Value
```

例如：

```text
基础攻击力 = 10

力量 +3
遗物 +2
虚弱 -20%

最终：
(10 + 3 + 2) × 0.8 = 12
```

不要在各个系统里直接修改数值。

统一使用 `StatSystem`。

---

# 十四、Modifier 系统

建议单独建立 `ModifierSystem`。

支持：

```text
Add
Multiply
Override
Min
Max
```

基本流程：

```text
Base
 ↓
Add
 ↓
Multiply
 ↓
Clamp
 ↓
Final
```

这样可以处理：

- 伤害 +20%
- 受到伤害 -25%
- 治疗 +50%
- 卡牌费用 -1
- 第一张攻击免费
- 毒伤害翻倍

等复杂机制。

---

# 十五、Deck 卡组系统

建议：

```text
Deck
├── DrawPile
├── Hand
├── DiscardPile
└── ExhaustPile
```

由 `DeckManager` 管理：

```text
Draw
Shuffle
Discard
Exhaust
Add
Remove
Transform
Upgrade
Duplicate
```

例如：

```text
Draw 3 Cards
```

本质上只是：

```text
DrawCardsEffect(3)
```

---

# 十六、Enemy 系统

敌人：

```text
Enemy
├── Stats
├── Intent
├── AI
├── Skills
└── Statuses
```

敌人的行为不要直接硬编码到 Enemy 类。

建议：

```text
EnemyAI
 ↓
SelectAction
 ↓
Action
 ↓
Effect[]
```

---

# 十七、Enemy AI

最简单可以采用 `Weighted Action Table`。

例如：

```text
Goblin AI

HP > 50%

70% → Attack
30% → Defend

HP < 50%

50% → Enrage
50% → Attack
```

后期可以升级到 `Behavior Tree`。

---

# 十八、Enemy Intent

统一建立 `Intent`。

例如：

```text
Attack
Defend
Buff
Debuff
Summon
Special
Unknown
```

敌人只负责告诉系统：

> 下一回合准备做什么。

UI 负责显示：

```text
👊 12
🛡 8
☠ 中毒
```

这样战斗系统和 UI 不需要绑定具体敌人。

---

# 十九、Roguelike Map 系统

核心单位：`Node`

Node 类型：

```text
Combat
Elite
Boss
Event
Shop
Rest
Treasure
Unknown
```

地图结构：

```text
Map
 ├── Layer 0
 ├── Layer 1
 ├── Layer 2
 ├── Layer 3
 ├── Layer 4
 └── Boss
```

每个 Node：

```text
NodeID
Type
Position
Connections[]
Visited
Locked
```

统一由 `MapGenerator` 生成。

---

# 二十、Map 与 UI 分离

不要：

```text
MapButton
 ↓
生成敌人
```

应该：

```text
MapNodeData
 ↓
RunManager
 ↓
进入 Node
 ↓
根据 NodeType
 ↓
Battle / Event / Shop / Rest
```

这样以后即使改成：

- 环形地图
- 六边形地图
- 世界地图
- 地牢地图
- 航线地图

核心系统也无需大改。

---

# 二十一、Event 系统

推荐统一 `EventDefinition`。

结构：

```text
Event
├── Description
└── Choices[]
```

每个 Choice：

```text
Choice
├── Requirements[]
└── Effects[]
```

例如：

```text
古老祭坛

选择 A
需要 HP > 50
→ 最大生命 +5

选择 B
→ 获得诅咒

选择 C
需要某个 Relic
→ 获得稀有卡
```

这样所有随机事件都可以数据驱动。

---

# 二十二、Relic 遗物系统

遗物本质上就是：

```text
Trigger
+
Condition
+
Effect
```

因此：

```text
Relic
├── ID
├── Name
├── Description
└── Triggers[]
```

例如：

```text
遗物：鲜血王冠

WHEN CombatStart
→ LoseHP(5)

WHEN EnemyDied
→ GainStrength(1)
```

不需要直接修改 CombatManager。

---

# 二十三、Resource 系统

建议统一建立：

```text
ResourcePool
```

然后使用：

```text
ResourceType
```

支持：

```text
HP
Energy
Gold
Stress
Rage
Mana
Ammo
Combo
Soul
```

这样以后换题材也可以复用。

---

# 二十四、GameState

整个 Run 最终应该由一个 `GameState` 描述。

例如：

```text
GameState

Run
├── Seed
├── Floor
├── Gold
├── Deck
├── Relics
├── Characters
├── Map
├── Inventory
└── Flags
```

战斗：

```text
CombatState
├── Characters
├── Enemies
├── Hand
├── DrawPile
├── DiscardPile
├── Turn
└── CombatModifiers
```

因此：

> **保存游戏 = 保存 GameState。**

而不是保存一堆 UI 和 GameObject。

---

# 二十五、RNG 随机系统

Roguelike 必须有独立 RNG 系统。

不要整个项目到处：

```csharp
Random.Range(...)
```

而应该使用 `RNGService`。

核心：

```text
Run Seed
 ↓
Map RNG
Combat RNG
Reward RNG
Event RNG
Shop RNG
```

推荐使用：

```text
Master Seed
    ↓
    ├── Map Seed
    ├── Combat Seed
    ├── Event Seed
    └── Reward Seed
```

这样可以实现：

- Bug 复现
- Replay
- Debug
- 固定 Seed 挑战
- Daily Run

---

# 二十六、Save 系统

保存数据应该只保存逻辑数据：

```json
{
    "version": 12,
    "run": {
        "seed": 839102,
        "floor": 7,
        "gold": 124
    },
    "deck": [
        "strike",
        "strike",
        "defend",
        "fireball"
    ],
    "relics": [
        "blood_ring"
    ]
}
```

不要直接保存：

```text
GameObject
Transform
Animator
UI
```

否则后续版本升级会非常麻烦。

---

# 二十七、项目目录结构

推荐：

```text
Game
│
├── Core
│   ├── GameState
│   ├── EventBus
│   ├── RNG
│   ├── Save
│   └── Utility
│
├── Data
│   ├── Cards
│   ├── Characters
│   ├── Enemies
│   ├── Relics
│   ├── Statuses
│   ├── Events
│   └── Maps
│
├── Combat
│   ├── CombatManager
│   ├── TurnSystem
│   ├── ActionSystem
│   ├── EffectSystem
│   ├── StatusSystem
│   ├── TargetSystem
│   └── DamageSystem
│
├── Roguelike
│   ├── RunManager
│   ├── MapGenerator
│   ├── RewardSystem
│   ├── ShopSystem
│   └── EventSystem
│
├── AI
│   ├── EnemyAI
│   ├── BehaviorTree
│   └── IntentSystem
│
├── UI
│   ├── CardUI
│   ├── CombatUI
│   ├── MapUI
│   ├── RewardUI
│   └── EventUI
│
├── Presentation
│   ├── Animation
│   ├── VFX
│   ├── Audio
│   └── Camera
│
└── Content
    ├── Cards
    ├── Enemies
    ├── Relics
    ├── Characters
    └── Events
```

---

# 二十八、核心接口

框架可以围绕以下接口构建。

## Effect

```csharp
public interface IEffect
{
    void Execute(GameContext context);
}
```

## Condition

```csharp
public interface ICondition
{
    bool Evaluate(GameContext context);
}
```

## Trigger

```csharp
public interface ITrigger
{
    GameEventType EventType { get; }

    void OnEvent(
        GameEvent gameEvent,
        GameContext context
    );
}
```

## Action

```csharp
public interface IAction
{
    void Execute(GameContext context);
}
```

## Target Selector

```csharp
public interface ITargetSelector
{
    IEnumerable<Entity> Select(GameContext context);
}
```

整体关系：

```text
Card
 ↓
Condition
 ↓
Effect
 ↓
Action
 ↓
GameState
 ↓
Event
 ↓
Trigger
 ↓
Effect
```

---

# 二十九、关键词系统

建议建立 `Keyword System`。

常见关键词：

```text
Exhaust
Retain
Ethereal
Innate
Combo
Poison
Bleed
Stress
Power
Curse
Consume
```

关键词本质上是：

```text
Tag + Rule
```

例如：

```text
Exhaust

Card Play
 ↓
Card does not enter DiscardPile
 ↓
Card enters ExhaustPile
```

以后设计卡牌时就可以直接组合关键词。

---

# 三十、一个完整卡牌的运行过程

例如：

```text
穿刺

Cost: 1

造成 6 点伤害
施加 2 层流血
如果目标已有流血，则抽 1 张牌
```

程序流程：

```text
Player
 ↓
PlayCard
 ↓
CheckCost
 ↓
ResolveTarget
 ↓
Card Effects
 ↓
DealDamage(6)
 ↓
ApplyBleed(2)
 ↓
EventBus
 ↓
StatusApplied
 ↓
Check Trigger
 ↓
Target has Bleed
 ↓
DrawCard(1)
 ↓
CardPlayed
 ↓
Discard
```

关键点：

> Card 本身不应该知道遗物、流血触发、抽牌机制具体如何工作。

---

# 三十一、推荐的《杀戮尖塔》+《暗黑地牢》组合方式

## 战斗层

借鉴《杀戮尖塔》：

```text
Card
Energy
Draw
Discard
Exhaust
Relic
Buff
Debuff
Enemy Intent
```

## 角色层

借鉴《暗黑地牢》：

```text
4 人队伍
Position
Speed
Stress
Class
Skill
Equipment
Relationship
Status
```

## Roguelike 层

借鉴《杀戮尖塔》：

```text
Node Map
Random Event
Elite
Shop
Rest
Treasure
Boss
```

## Meta 层

借鉴《暗黑地牢》：

```text
Permanent Unlock
Character Unlock
Equipment Unlock
Event Unlock
Town Upgrade
```

---

# 三十二、推荐开发顺序

不要先做 UI 和美术。

建议：

```text
① GameState
      ↓
② Entity / Character
      ↓
③ Effect System
      ↓
④ Status System
      ↓
⑤ Combat
      ↓
⑥ Card
      ↓
⑦ Enemy AI
      ↓
⑧ Reward
      ↓
⑨ Map
      ↓
⑩ Event
      ↓
⑪ Shop
      ↓
⑫ Save
      ↓
⑬ UI
      ↓
⑭ Animation / VFX / Audio
```

---

# 三十三、MVP 版本

第一版不需要很多内容。

建议：

```text
1 个角色
5 种敌人
20 张卡牌
5 个状态
5 个遗物
1 个 Boss
1 套地图
1 个商店
3 个随机事件
```

但框架必须已经支持：

```text
Card
Effect
Status
Trigger
Relic
Enemy
AI
Map
Event
Reward
Save
RNG
```

这样以后主要工作就变成：

> **增加内容，而不是重写系统。**

---

# 三十四、最终三层架构

最终建议把整个项目分成：

```text
┌─────────────────────────────┐
│         Content Layer       │
│ 卡牌 / 敌人 / 遗物 / 事件    │
├─────────────────────────────┤
│          Game Layer         │
│ Combat / Run / Map / AI     │
├─────────────────────────────┤
│       Framework Layer       │
│ State / Event / Effect      │
│ RNG / Save / Data / Rules   │
└─────────────────────────────┘
```

其中 Framework Layer 不应该知道具体游戏内容。

它只负责：

```text
GameState
Event
Effect
Condition
Trigger
RNG
Save
Data
Rules
```

---

# 三十五、推荐的最终核心模块

如果正式开发，可以搭建以下模块：

```text
GameContext
GameState
Entity

CardDefinition
EffectDefinition
StatusDefinition
TriggerDefinition
RelicDefinition
EnemyDefinition
EventDefinition

CardManager
CombatManager
TurnManager
EffectManager
StatusManager
TargetManager
DamageManager
ResourceManager
DeckManager

RunManager
MapManager
RewardManager
ShopManager
EventManager

EnemyAI
RNGManager
SaveManager
```

最终希望达到的效果：

```text
创建新卡牌
    ↓
创建 CardData
    ↓
配置 Effect
    ↓
配置 Condition
    ↓
配置 Keyword
    ↓
完成
```

而不是：

```text
创建新卡牌
    ↓
写一套新的 C# 逻辑
    ↓
修改 CombatManager
    ↓
修改 UI
    ↓
修改 Status
    ↓
修改 Enemy
    ↓
开始出现 Bug
```

---

# 三十六、核心设计理念总结

整个框架可以浓缩成下面这句话：

> **Card、Relic、Enemy、Event 都只是“内容”；Effect、Status、Trigger、Condition、Action 才是“规则”；GameState 是整个游戏的唯一真实状态。**

最终结构：

```text
             ┌──────────────┐
             │   Content    │
             │Card/Enemy... │
             └──────┬───────┘
                    ↓
             ┌──────────────┐
             │    Rules     │
             │Condition     │
             │Trigger       │
             │Status        │
             └──────┬───────┘
                    ↓
             ┌──────────────┐
             │    Effect    │
             │Damage/Heal   │
             │Draw/Move...  │
             └──────┬───────┘
                    ↓
             ┌──────────────┐
             │    Action    │
             └──────┬───────┘
                    ↓
             ┌──────────────┐
             │  GameState   │
             └──────┬───────┘
                    ↓
             ┌──────────────┐
             │ Presentation │
             │ UI/VFX/Audio │
             └──────────────┘
```

这套结构可以作为一个通用的：

**Roguelike Card Game Framework v1.0**

用于开发《杀戮尖塔》+《暗黑地牢》类型的独立游戏。
