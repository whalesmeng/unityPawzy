# Unity 配置指南

本文档指导你在 Unity 编辑器中完成小游戏集合平台的配置和验证。

## 前置条件

- Unity 2022.x LTS 已安装
- 项目已在 Unity 中打开
- 代码已编译无错误

---

## 第一步：创建游戏配置文件

### 1.1 自动创建配置

1. 打开 Unity 编辑器
2. 等待脚本编译完成
3. 点击菜单栏：**PawzyPop > Create Game Configs**
4. 检查 `Assets/Resources/GameConfigs/` 目录下是否生成了：
   - `Match3Config.asset`
   - `GomokuConfig.asset`

### 1.2 验证配置

选中 `Match3Config.asset`，在 Inspector 中确认：
- Game Id: `match3`
- Game Name: `消消乐`
- Scene Name: `SampleScene`
- Is Unlocked: ✅

选中 `GomokuConfig.asset`，确认：
- Game Id: `gomoku`
- Game Name: `五子棋`
- Scene Name: `GomokuGame`
- Is Unlocked: ✅

---

## 第二步：创建主菜单场景

### 2.1 新建场景

1. **File > New Scene** (选择 Basic 2D)
2. **File > Save As** 保存为 `Assets/Scenes/MainMenu.unity`

### 2.2 创建 UI Canvas

1. 右键 Hierarchy > **UI > Canvas**
2. 选中 Canvas，设置 Canvas Scaler：
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1080 x 1920`
   - Match: `0.5`

### 2.3 创建主菜单 UI

#### 标题文本
1. 右键 Canvas > **UI > Text - TextMeshPro**
2. 命名为 `TitleText`
3. 设置：
   - Text: `小游戏集合`
   - Font Size: `72`
   - Alignment: Center
   - Rect Transform:
     - Anchor: Top Center
     - Pos Y: `-150`
     - Width: `600`, Height: `100`

#### 游戏列表容器
1. 右键 Canvas > **UI > Scroll View**
2. 命名为 `GameListScrollView`
3. 设置 Rect Transform：
   - Anchor: Stretch
   - Left: `50`, Right: `50`
   - Top: `300`, Bottom: `100`
4. 删除 `Scrollbar Horizontal`（不需要横向滚动）
5. 选中 `Content`，添加组件：
   - **Vertical Layout Group**
     - Spacing: `20`
     - Child Alignment: Upper Center
     - Child Force Expand Width: ✅
     - Child Force Expand Height: ❌
   - **Content Size Fitter**
     - Vertical Fit: `Preferred Size`

### 2.4 创建游戏卡片预制体

#### 创建卡片 UI
1. 右键 `Content` > **UI > Button**
2. 命名为 `GameCard`
3. 设置 Rect Transform：
   - Width: `900`, Height: `200`
4. 添加子元素：

**图标 (Icon)**
```
右键 GameCard > UI > Image
命名: IconImage
Rect Transform:
  - Anchor: Left
  - Pos X: 100, Width: 150, Height: 150
```

**名称文本 (Name)**
```
右键 GameCard > UI > Text - TextMeshPro
命名: NameText
Text: 游戏名称
Font Size: 48
Rect Transform:
  - Anchor: Left
  - Pos X: 250, Pos Y: 30
  - Width: 400, Height: 60
```

**描述文本 (Description)**
```
右键 GameCard > UI > Text - TextMeshPro
命名: DescriptionText
Text: 游戏描述
Font Size: 28
Color: Gray
Rect Transform:
  - Anchor: Left
  - Pos X: 250, Pos Y: -30
  - Width: 500, Height: 40
```

**高分文本 (HighScore)**
```
右键 GameCard > UI > Text - TextMeshPro
命名: HighScoreText
Text: 最高分: 0
Font Size: 24
Rect Transform:
  - Anchor: Right
  - Pos X: -100
  - Width: 200, Height: 40
```

#### 添加 GameCard 脚本
1. 选中 `GameCard`
2. **Add Component > GameCard** (在 PawzyPop.UI 命名空间)
3. 拖拽引用：
   - Icon Image → `IconImage`
   - Name Text → `NameText`
   - Description Text → `DescriptionText`
   - High Score Text → `HighScoreText`
   - Card Button → `GameCard` 自身的 Button 组件
   - Background Image → `GameCard` 自身的 Image 组件

#### 保存为预制体
1. 将 `GameCard` 从 Hierarchy 拖到 `Assets/Prefabs/UI/`
2. 删除 Hierarchy 中的 `GameCard`（保留预制体即可）

### 2.5 添加 MainMenuUI 脚本

1. 创建空 GameObject，命名为 `MainMenuManager`
2. **Add Component > MainMenuUI**
3. 拖拽引用：
   - Game List Container → `Content` (ScrollView 下的)
   - Game Card Prefab → `Assets/Prefabs/UI/GameCard.prefab`
   - Title Text → `TitleText`

### 2.6 添加必要的管理器

创建空 GameObject `Managers`，添加以下组件：
- **SceneLoader**
- **GameConfigManager**
- **SaveManager**

---

## 第三步：创建五子棋场景

### 3.1 新建场景

1. **File > New Scene** (选择 Basic 2D)
2. **File > Save As** 保存为 `Assets/Scenes/GomokuGame.unity`

### 3.2 创建游戏管理器

1. 创建空 GameObject `GomokuManager`
2. **Add Component > GomokuGame**
3. 设置：
   - Game Id: `gomoku`
   - Board Size: `15`
   - AI Difficulty: `2`

### 3.3 创建棋盘

1. 创建空 GameObject `GomokuBoard`
2. **Add Component > GomokuBoard**
3. 设置：
   - Size: `15`
   - Cell Size: `0.5`
4. 将 `GomokuBoard` 拖到 `GomokuGame` 的 Board 字段

### 3.4 创建 UI

#### Canvas 设置
同主菜单场景的 Canvas 设置。

#### 回合显示
```
UI > Text - TextMeshPro
命名: TurnText
Text: 你的回合 (黑)
Font Size: 36
Anchor: Top Center
Pos Y: -80
```

#### 按钮区域
```
创建空 GameObject: ButtonPanel
Anchor: Bottom Center
添加 Horizontal Layout Group

子元素 (3个 Button):
- RestartButton (文本: 重新开始)
- PauseButton (文本: 暂停)
- BackButton (文本: 返回)
```

#### 结果弹窗
```
创建 Panel: WinPanel
- 默认隐藏 (取消勾选 Active)
- 添加半透明背景
- 子元素:
  - TitleText: "游戏结束"
  - MessageText: "恭喜获胜!"
  - RestartButton
  - BackToMenuButton
```

### 3.5 添加 GomokuUI 脚本

1. 创建空 GameObject `GomokuUI`
2. **Add Component > GomokuUI**
3. 拖拽所有 UI 引用

### 3.6 添加管理器

同主菜单场景，添加 `SceneLoader` 和 `SaveManager`。

---

## 第四步：配置消消乐场景

### 4.1 打开现有场景

打开 `Assets/Scenes/SampleScene.unity`

### 4.2 添加 Match3Game（可选）

如果想使用新架构：
1. 创建空 GameObject `Match3Game`
2. **Add Component > Match3Game**
3. 设置：
   - Game Id: `match3`
   - Target Score: `1000`
   - Max Moves: `20`

### 4.3 更新 GameUI

1. 选中现有的 GameUI 对象
2. 添加 `Back To Menu Button` 引用（需要在 UI 中创建返回按钮）

### 4.4 创建返回按钮

在游戏 UI 中添加一个返回主菜单的按钮：
```
UI > Button
命名: BackToMenuButton
Text: 返回
位置: 左上角或暂停面板中
```

---

## 第五步：配置 Build Settings

1. **File > Build Settings**
2. 点击 **Add Open Scenes** 或拖拽场景：
   - `Scenes/MainMenu` (Index 0 - 启动场景)
   - `Scenes/SampleScene` (Index 1)
   - `Scenes/GomokuGame` (Index 2)
3. 确保 `MainMenu` 在最上面（Index 0）

---

## 第六步：验证测试

### 6.1 测试主菜单

1. 打开 `MainMenu` 场景
2. 点击 Play
3. 验证：
   - [ ] 标题显示正确
   - [ ] 游戏卡片列表显示（消消乐、五子棋）
   - [ ] 点击卡片能跳转到对应场景

### 6.2 测试消消乐

1. 从主菜单点击消消乐卡片
2. 验证：
   - [ ] 棋盘正常显示
   - [ ] 可以交换方块
   - [ ] 消除和得分正常
   - [ ] 返回按钮能回到主菜单

### 6.3 测试五子棋

1. 从主菜单点击五子棋卡片
2. 验证：
   - [ ] 15x15 棋盘显示正确
   - [ ] 点击能落子（黑子）
   - [ ] AI 能自动落子（白子）
   - [ ] 五子连线能判定胜负
   - [ ] 返回按钮能回到主菜单

---

## 常见问题

### Q: 游戏配置加载失败
**A:** 确保配置文件在 `Assets/Resources/GameConfigs/` 目录下，且文件名正确。

### Q: 场景跳转失败
**A:** 检查 Build Settings 中是否添加了所有场景，且场景名称与配置中的 `sceneName` 一致。

### Q: UI 不显示
**A:** 检查 Canvas 的 Render Mode 是否为 Screen Space - Overlay，以及 EventSystem 是否存在。

### Q: 五子棋点击无响应
**A:** 确保 Camera 是 Orthographic 模式，且 GomokuBoard 的 Update 方法能接收输入。

### Q: 编译错误
**A:** 确保所有命名空间引用正确，特别是：
- `PawzyPop.Core`
- `PawzyPop.Data`
- `PawzyPop.Games`
- `PawzyPop.Games.Match3`
- `PawzyPop.Games.Gomoku`
- `PawzyPop.UI`

---

## 快速验证清单

```
□ 游戏配置文件已创建 (PawzyPop > Create Game Configs)
□ MainMenu 场景已创建并配置
□ GomokuGame 场景已创建并配置
□ GameCard 预制体已创建
□ Build Settings 已添加所有场景
□ 主菜单能显示游戏列表
□ 能从主菜单进入消消乐
□ 能从主菜单进入五子棋
□ 能从游戏中返回主菜单
□ 五子棋 AI 能正常工作
```

---

## 下一步

完成以上配置后，你可以：
1. 在 Android 设备上测试
2. 调整 UI 样式和动画
3. 添加音效和背景音乐
4. 优化 AI 难度
