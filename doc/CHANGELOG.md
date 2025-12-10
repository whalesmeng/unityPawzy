# Changelog

## [1.2.0] - 2025-12-10

### 新增
- **小游戏集合平台基础架构**：实现 Phase 1-5 的核心功能
  
#### Phase 1: 项目结构
- 创建游戏模块目录结构：`Assets/Scripts/Games/Common/`, `Match3/`, `Gomoku/`
- 创建主菜单UI目录：`Assets/Scripts/UI/MainMenu/`
- 创建预制体目录：`Assets/Prefabs/Games/`, `Assets/Prefabs/UI/`
- 创建配置资源目录：`Assets/Resources/GameConfigs/`

#### Phase 2: 基础架构
- `IGame.cs` - 游戏通用接口，定义游戏生命周期方法
- `GameBase.cs` - 游戏基类，提供 IGame 接口的默认实现
- `GameInfo.cs` - 游戏信息 ScriptableObject，用于配置游戏元数据
- `GameState.cs` - 游戏状态枚举（NotStarted, Playing, Paused, GameOver）
- `GameResult.cs` - 游戏结果枚举（None, Win, Lose, Draw）
- `SceneLoader.cs` - 场景加载管理器，支持同步/异步加载
- `GameConfigManager.cs` - 游戏配置管理器，从 Resources 加载配置
- `SaveManager.cs` 扩展 - 新增多游戏高分保存、设置保存功能

#### Phase 3: 主菜单导航 (US1)
- `MainMenuUI.cs` - 主菜单UI控制器
- `GameCard.cs` - 游戏卡片组件，显示游戏入口
- `BackToMenuButton.cs` - 返回主菜单按钮组件
- `GameConfigCreator.cs` - Unity 编辑器工具，用于创建游戏配置

#### Phase 4: 消消乐游戏重构 (US2)
- `Match3Game.cs` - 消消乐游戏控制器，实现 IGame 接口
- `GameUI.cs` - 更新支持新旧两套游戏管理器，新增返回主菜单按钮
- `MatchProcessor.cs` - 更新支持 Match3Game，兼容旧 GameManager

#### Phase 5: 五子棋游戏 (US3)
- `GomokuGame.cs` - 五子棋游戏控制器，实现 IGame 接口
- `GomokuBoard.cs` - 五子棋棋盘，支持 15x15 网格、落子、胜负判定
- `GomokuAI.cs` - 五子棋 AI，使用 Minimax + Alpha-Beta 剪枝算法
- `GomokuUI.cs` - 五子棋游戏UI，显示回合、胜负结果

### 技术说明
- 使用 ScriptableObject 管理游戏配置
- 支持异步场景加载，最小加载时间 0.5s
- 多游戏高分独立保存，支持游玩次数和胜利次数统计
- 保持向后兼容：GameUI 和 MatchProcessor 同时支持新旧系统
- 五子棋 AI：Minimax + Alpha-Beta 剪枝，支持 3 级难度

---

## [1.1.1] - 2025-12-10

### 变更
- **Constitution 更新 (v1.1.0)**：
  - 新增「调试日志规范」原则：开发过程中必须补充充足日志用于 Bug 分析
  - 简化开发流程：单人开发模式，直接在 main 分支开发
  - 移除 PR/代码审查相关要求

---

## [1.1.0-spec] - 2025-12-10

### 新增
- **小游戏集合平台规格设计**：完成功能规格和实施计划
  - `specs/001-mini-game-collection/spec.md` - 功能规格文档
  - `specs/001-mini-game-collection/plan.md` - 实施计划
  - `specs/001-mini-game-collection/research.md` - 技术研究
  - `specs/001-mini-game-collection/data-model.md` - 数据模型设计
  - `specs/001-mini-game-collection/quickstart.md` - 快速开始指南
  - `specs/001-mini-game-collection/tasks.md` - 任务列表（59个任务）
  - `CODEBUDDY.md` - AI 助手上下文配置

### 规划内容
- 主菜单导航系统 (P1)
- 消消乐游戏重构 (P1)
- 五子棋游戏 (P2)
- 游戏进度保存 (P3)

---

## [1.0.2] - 2025-12-10

### 新增
- **项目 Constitution**：初始化 speckit，建立项目规范
  - `.specify/memory/constitution.md` - 项目宪法，定义核心原则
  - CHANGELOG-First Commits 原则：每次提交前必须更新 CHANGELOG
  - Commit Message 规范：使用 `<type>: <subject>` 格式
  - Unity 项目结构规范
  - 代码质量和跨平台兼容规范

### 变更
- `.codebuddy/rules/git-commit-rules.mdc` - Git 提交规则

---

## [1.0.1] - 2025-12-10

### 修复
- **安卓屏幕适配问题**：修复了游戏在安卓设备上显示位置偏移的问题
  - 新增 `CameraScaler.cs`：自动根据屏幕比例调整相机正交大小，确保游戏内容在不同屏幕尺寸上完整显示并居中
  - 修改 `Board.cs`：使用 `localPosition` 设置棋盘位置，运行时自动添加 CameraScaler 组件
  
- **MatchFinder 空引用异常**：修复了游戏启动时 `MatchFinder.FindAllMatches()` 报空引用错误的问题
  - 添加 `EnsureBoardReference()` 方法，确保在使用 Board 引用前已正确初始化

### 新增
- `Assets/Scripts/Core/CameraScaler.cs` - 相机自适应缩放组件

### 变更
- `Assets/Scripts/Core/Board.cs` - 优化棋盘居中逻辑，添加调试日志
- `Assets/Scripts/Core/MatchFinder.cs` - 添加空引用保护

---

## [1.0.0] - 初始版本

### 功能
- 6x6 消消乐游戏棋盘
- 6 种不同类型的 Tile（Corgi, Golden, Husky, Samoyed, Shiba, Teddy）
- 滑动交换 Tile 机制
- 三消匹配检测
- 关卡系统（5 个关卡）
- 分数和步数系统
- 胜利/失败弹窗
