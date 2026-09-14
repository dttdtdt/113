# CardSystem

---

## 目标

### CardSystem 开发 v0.2 目标

实现简单的手牌选择器，使玩家能够：

* 悬浮选择手牌
* 点击选择手牌
* 取消选择
* 确认使用卡牌
* 进入已有的打牌逻辑

### 要求

* 光标悬浮在手牌上时，对该卡牌进行简单的视觉强化。

  * 例如：卡牌放大、上移等。
* 左键点击手牌后，进入卡牌选择状态。
* 被选择的卡牌固定放大，或移动到屏幕中央进行展示。
* 选择状态下，需要能够执行：

  * 取消
  * 使用
* 「取消」和「使用」可以通过以下任意方式实现：

  * UI 按钮
  * 键盘按键
  * 其他简单的交互方式
* 触发「取消」后：

  * 取消当前卡牌选择。
  * 返回正常手牌选择状态。
* 触发「使用」后：

  * 将当前选择的卡牌打出。
  * 调用已有的打牌逻辑。
* Demo 阶段不需要过多考虑具体视觉表现，能够清晰表达交互状态即可。

---

## 开发结构

### Class

#### HandManager 手牌管理器

负责管理当前手牌的悬浮与选择状态。

Demo 阶段可以使用单例模式。

```text id="4o7v2d"
|- CurrentHoverCardIndex
|   当前悬浮卡牌的索引
|   无悬浮时为 -1
|
|- CurrentSelectedCardIndex
|   当前选择卡牌的索引
|   无选择时为 -1
|
|————————————————————
|
|- ChangeHoverCard()
|   改变当前悬浮的卡牌
|
|- ChangeSelectedCard()
|   改变当前选择的卡牌
|
|- ClearHoverCard()
|   清除当前悬浮卡牌
|
|- ClearSelectedCard()
|   清除当前选择卡牌
|
|- ConfirmUseCard()
|   确认使用当前选择的卡牌
|   调用已有的打牌逻辑
|
|- CancelSelectedCard()
|   取消当前选择
```

#### HandCardView 单个手牌卡牌视图

负责单张手牌的输入检测与视觉表现。

```text id="j2c8pz"
|- Index
|   当前卡牌索引
|
|————————————————————
|
|- OnHover()
|   鼠标悬浮时调用
|   -> HandManager.ChangeHoverCard()
|
|- OnExit()
|   鼠标离开时调用
|   -> HandManager.ClearHoverCard()
|
|- OnClick()
|   鼠标点击时调用
|   -> HandManager.ChangeSelectedCard()
|
|- SetHoverState()
|   根据悬浮状态更新视觉表现
|
|- SetSelectedState()
|   根据选择状态更新视觉表现
```

---

## 交互流程

### 正常状态

```text id="d1cq9w"
手牌
 ↓
鼠标悬浮
 ↓
HandCardView.OnHover()
 ↓
HandManager.ChangeHoverCard()
 ↓
卡牌视觉强化
```

### 选择卡牌

```text id="e2j8r5"
点击手牌
 ↓
HandCardView.OnClick()
 ↓
HandManager.ChangeSelectedCard()
 ↓
进入卡牌选择状态
 ↓
等待玩家确认「取消」或「使用」
```

### 取消

```text id="n7w4qa"
玩家触发「取消」
 ↓
HandManager.CancelSelectedCard()
 ↓
清除 CurrentSelectedCardIndex
 ↓
返回正常手牌选择状态
```

### 使用

```text id="v9z3kx"
玩家触发「使用」
 ↓
HandManager.ConfirmUseCard()
 ↓
调用 CardSystem 打牌逻辑
 ↓
Hand → DiscardPile
 ↓
清除当前选择状态
 ↓
返回正常手牌状态
```

---

## 开发要求

### 1. HandManager 不直接负责视觉表现

`HandManager` 只负责维护：

* 当前悬浮卡牌
* 当前选择卡牌
* 卡牌选择/取消/确认状态

具体卡牌如何放大、移动、显示，由 `HandCardView` 负责。

### 2. HandCardView 不直接处理卡牌逻辑

`HandCardView` 只负责：

* 接收鼠标输入
* 通知 `HandManager`
* 根据状态改变自身表现

不直接操作：

* `DrawPile`
* `Hand`
* `DiscardPile`

### 3. UI / 按键只负责触发接口

按钮或按键不直接实现卡牌逻辑。

例如：

```text id="f7m2ka"
玩家触发「使用」
 ↓
调用 HandManager.ConfirmUseCard()
 ↓
调用已有 CardSystem 打牌接口
```

以及：

```text id="x4n6qs"
玩家触发「取消」
 ↓
调用 HandManager.CancelSelectedCard()
```

UI / 输入层不直接修改：

```text
Hand
DrawPile
DiscardPile
```

---

## Demo 验收标准

能够完成以下流程：

```text id="q8b4hm"
显示手牌
 ↓
鼠标悬浮卡牌
 ↓
卡牌视觉强化
 ↓
点击卡牌
 ↓
卡牌进入选择状态
 ↓
等待玩家确认
 ├────────────────┐
 ↓                ↓
取消              使用
 ↓                ↓
返回手牌          调用打牌逻辑
状态              ↓
                 手牌 → 弃牌堆
```

「取消」和「使用」的具体触发方式不做强制要求，可以使用按钮或按键。

整个流程能够正常循环使用。
