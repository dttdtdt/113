# CardSystem

---

## Enum

### Profession 职业/职能

### CardType 卡牌大分类

### CardTag 卡牌标签

### CardRarity 卡牌稀有度

---

## Class

### CardDefinition 卡牌配置

```text
|- Identity
|   卡牌唯一标识符
|
|- Name
|  卡牌名称
|
|- Image
|   卡牌图片
|
|- Description
|   卡牌描述
|
|- Profession
|   卡牌所属职业/职能
|
|- Characters
|   卡牌所属角色
|   非必须，如果后续角色 ≠ 职业时使用
|
|- Type
|   卡牌大类型
|   例如：常规、技能、能力等
|
|- Tags
|   卡牌标签
|   例如：攻击、防御、辅助等
|
|- Rarity
|   卡牌稀有度
|
|- 【下面的部分都可以先不弄】
|- UsageConditions
|   卡牌使用前需要满足的条件
|
|- Costs
|   卡牌使用时需要支付的资源
|
|- TargetRules
|   卡牌使用时允许选择的目标规则
|
|- EffectConditions
|   卡牌效果生效所需满足的条件
|
|- Effects
|   卡牌打出后产生的效果
```

### CardInstance 运行时卡牌数据

```text
|- Identity（这里也可以直接用【卡牌配置】）
|   卡牌唯一标识符
```

### CharacterDefinition 角色配置

```text
|- Identity
|   角色唯一标识符
|
|- Name
|  角色名称
|
|- Profession
|   角色所属职业/职能
|
|- HandLimit
|  手牌上限
|
|- CardPool
|  角色卡牌候选池
|
|- InitialDeck
|  选择角色时获得的卡牌
```

### CharacterInstance 运行时角色数据

```text
|- Identity（这里也可以直接用【角色配置】）
|   角色唯一标识符
|
|- Deck
|  持有的卡牌
|
|- HandLimit
|  手牌上限
|
|- DrawPile
|  抽牌堆
|
|- Hand
|  手牌
|
|- DiscardPile
|  弃牌堆
```

---

## CardSystem Demo 开发目标

### 目标

需要实现以下核心卡牌流程：

1. **选择角色**

   * 玩家选择一个角色。
   * 获取该角色对应的初始牌堆 `InitialDeck`。

2. **获取新卡牌**

   * 通过按键模拟从角色 `CardPool` 中获取新的卡牌。
   * 获取方式暂时采用随机。
   * 获取的新卡牌加入当前牌组 `Deck`。

3. **开始模拟对局**

   * 通过按键开始模拟对局。
   * 对当前牌组进行洗牌，并生成抽牌堆 `DrawPile`。

4. **抽牌**

   * 当手牌未达到手牌上限时，从抽牌堆依次抽牌。
   * 抽取的卡牌加入手牌 `Hand`。
   * 手牌为空时，需要能够一次补充至手牌上限。

5. **模拟打牌**

   * 通过按键模拟打出卡牌。
   * 暂时不处理卡牌实际效果。
   * 打出的卡牌从手牌 `Hand` 移动至弃牌堆 `DiscardPile`。

6. **弃牌堆循环**

   * 当抽牌堆 `DrawPile` 为空时：

     * 将弃牌堆 `DiscardPile` 洗牌。
     * 将洗牌后的卡牌重新放入抽牌堆 `DrawPile`。
   * 后续继续从抽牌堆抽牌，实现卡牌循环。

---

### 主要接口

CardSystem 需要提供独立的逻辑接口，具体实现不由按键逻辑负责。

#### 角色相关

```text
SelectCharacter()
GetInitialDeck()
```

#### 卡牌获取

```text
GetRandomCardFromPool()
AddCardToDeck()
```

#### 牌组相关

```text
InitializeDeck()
ShuffleDeck()
```

#### 抽牌相关

```text
DrawCard()
DrawCards()
RefillHand()
```

#### 打牌相关

```text
PlayCard()
DiscardCard()
```

#### 牌堆循环

```text
ReshuffleDiscardPile()
```

---

### 开发要求

#### 1. 逻辑独立

卡牌逻辑与输入/按键逻辑分离。

按键只负责调用 CardSystem 提供的接口，不直接修改：

* `Deck`
* `DrawPile`
* `Hand`
* `DiscardPile`

例如：

```text
按键
 ↓
调用 DrawCard()
 ↓
CardSystem 处理抽牌逻辑
```

而不是：

```text
按键
 ↓
直接操作 DrawPile
 ↓
直接把卡牌放入 Hand
```

#### 2. 接口独立

各项核心操作应通过独立方法实现，避免将多个无关操作全部写在一个方法中。

例如：

```text
DrawCard()
PlayCard()
ReshuffleDiscardPile()
```

分别负责各自的逻辑。

#### 3. 低耦合、高内聚

* 卡牌系统负责卡牌及牌堆相关逻辑。
* 输入系统负责按键输入。
* UI 负责显示卡牌和牌堆状态。
* 不同系统之间通过接口进行交互。
* 避免系统之间直接修改彼此内部数据。

#### 4. 按键仅用于模拟

Demo 阶段使用按键模拟玩家操作。

按键逻辑只调用已有接口，不在按键代码中实现具体卡牌逻辑。

例如：

```text
按键获取卡牌 → GetRandomCardFromPool()

按键开始对局 → InitializeDeck()

按键抽牌 → DrawCard() / RefillHand()

按键打牌 → PlayCard()

按键洗牌 → ReshuffleDiscardPile()
```

---

### Demo 验收标准

能够通过按键完整运行以下流程：

```text
选择角色
 ↓
获得初始牌组
 ↓
从 CardPool 随机获得新卡
 ↓
开始模拟对局
 ↓
牌组洗牌
 ↓
抽牌
 ↓
获得手牌
 ↓
模拟打牌
 ↓
手牌 → 弃牌堆
 ↓
抽牌堆耗尽
 ↓
弃牌堆洗牌
 ↓
重新生成抽牌堆
 ↓
继续抽牌
```

整个流程可以循环运行，且卡牌不会因为牌堆切换而丢失或重复产生。
