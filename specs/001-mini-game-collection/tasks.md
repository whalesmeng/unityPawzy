# Tasks: 小游戏集合平台

**Input**: Design documents from `/specs/001-mini-game-collection/`  
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅

**Tests**: 未明确要求，本任务列表不包含测试任务。

**Organization**: 任务按用户故事分组，支持独立实现和测试。

## Format: `[ID] [P?] [Story] Description`

- **[P]**: 可并行执行（不同文件，无依赖）
- **[Story]**: 所属用户故事 (US1, US2, US3, US4)
- 描述中包含精确文件路径

## Path Conventions

- **Unity 项目**: `Assets/Scripts/`, `Assets/Scenes/`, `Assets/Prefabs/`
- 基于 plan.md 中定义的项目结构

---

## Phase 1: Setup (项目初始化)

**Purpose**: 创建新的目录结构和基础文件

- [ ] T001 创建游戏模块目录结构 `Assets/Scripts/Games/Common/`
- [ ] T002 [P] 创建消消乐模块目录 `Assets/Scripts/Games/Match3/`
- [ ] T003 [P] 创建五子棋模块目录 `Assets/Scripts/Games/Gomoku/`
- [ ] T004 [P] 创建主菜单 UI 目录 `Assets/Scripts/UI/MainMenu/`
- [ ] T005 [P] 创建游戏预制体目录 `Assets/Prefabs/Games/` 和 `Assets/Prefabs/UI/`
- [ ] T006 [P] 创建游戏配置资源目录 `Assets/Resources/GameConfigs/`

---

## Phase 2: Foundational (基础架构)

**Purpose**: 所有用户故事依赖的核心基础设施

**⚠️ CRITICAL**: 必须完成此阶段才能开始用户故事开发

- [ ] T007 创建游戏接口 `IGame.cs` in `Assets/Scripts/Games/Common/IGame.cs`
- [ ] T008 创建游戏基类 `GameBase.cs` in `Assets/Scripts/Games/Common/GameBase.cs`
- [ ] T009 创建游戏信息数据类 `GameInfo.cs` in `Assets/Scripts/Data/GameInfo.cs`
- [ ] T010 [P] 创建游戏状态枚举 `GameState.cs` in `Assets/Scripts/Data/GameState.cs`
- [ ] T011 [P] 创建游戏结果枚举 `GameResult.cs` in `Assets/Scripts/Data/GameResult.cs`
- [ ] T012 扩展 `SaveManager.cs` 支持多游戏最高分保存 in `Assets/Scripts/Data/SaveManager.cs`
- [ ] T013 创建游戏配置管理器 `GameConfigManager.cs` in `Assets/Scripts/Data/GameConfigManager.cs`
- [ ] T014 创建场景管理工具类 `SceneLoader.cs` in `Assets/Scripts/Core/SceneLoader.cs`

**Checkpoint**: 基础架构就绪，可以开始用户故事开发

---

## Phase 3: User Story 1 - 游戏主菜单导航 (Priority: P1) 🎯 MVP

**Goal**: 用户打开应用后看到主菜单，可以选择并进入任意游戏

**Independent Test**: 启动应用 → 显示主菜单 → 点击游戏卡片 → 进入游戏场景 → 点击返回 → 回到主菜单

### Implementation for User Story 1

- [ ] T015 [US1] 创建主菜单场景 `MainMenu.unity` in `Assets/Scenes/MainMenu.unity`
- [ ] T016 [US1] 创建主菜单 UI 控制器 `MainMenuUI.cs` in `Assets/Scripts/UI/MainMenu/MainMenuUI.cs`
- [ ] T017 [P] [US1] 创建游戏卡片组件 `GameCard.cs` in `Assets/Scripts/UI/MainMenu/GameCard.cs`
- [ ] T018 [P] [US1] 创建游戏卡片预制体 `GameCard.prefab` in `Assets/Prefabs/UI/GameCard.prefab`
- [ ] T019 [US1] 配置主菜单 Canvas 和 UI 布局（标题、滚动列表、底部导航）
- [ ] T020 [US1] 创建消消乐游戏配置 `Match3Config.asset` in `Assets/Resources/GameConfigs/Match3Config.asset`
- [ ] T021 [P] [US1] 创建五子棋游戏配置 `GomokuConfig.asset` in `Assets/Resources/GameConfigs/GomokuConfig.asset`
- [ ] T022 [US1] 实现主菜单到游戏场景的切换逻辑
- [ ] T023 [US1] 实现游戏场景返回主菜单的通用逻辑
- [ ] T024 [US1] 配置 Build Settings 添加所有场景

**Checkpoint**: 主菜单功能完整，可以导航到游戏场景（即使游戏内容未完成）

---

## Phase 4: User Story 2 - 消消乐游戏 (Priority: P1)

**Goal**: 用户可以玩完整的消消乐游戏，包括方块交换、消除、连消、得分

**Independent Test**: 进入消消乐 → 交换方块 → 消除得分 → 连消 → 步数用完 → 显示结果

### Implementation for User Story 2

- [ ] T025 [US2] 重命名现有场景为 `Match3Game.unity` in `Assets/Scenes/Match3Game.unity`
- [ ] T026 [US2] 创建消消乐游戏控制器 `Match3Game.cs` in `Assets/Scripts/Games/Match3/Match3Game.cs`
- [ ] T027 [US2] 重构 `Board.cs` 为 `Match3Board.cs` in `Assets/Scripts/Games/Match3/Match3Board.cs`
- [ ] T028 [US2] 将现有 Core 脚本适配到 Match3 模块（保持兼容）
- [ ] T029 [US2] 实现 `IGame` 接口的 Initialize/Pause/Resume/Restart 方法
- [ ] T030 [US2] 添加返回主菜单按钮到游戏 UI
- [ ] T031 [US2] 实现游戏结束时的结果弹窗（重新开始/返回主菜单）
- [ ] T032 [US2] 集成最高分保存功能

**Checkpoint**: 消消乐游戏完整可玩，可独立测试

---

## Phase 5: User Story 3 - 五子棋游戏 (Priority: P2)

**Goal**: 用户可以与 AI 进行五子棋对弈，正确判定胜负

**Independent Test**: 进入五子棋 → 用户落子 → AI 落子 → 连成五子 → 显示胜负

### Implementation for User Story 3

- [ ] T033 [US3] 创建五子棋场景 `GomokuGame.unity` in `Assets/Scenes/GomokuGame.unity`
- [ ] T034 [US3] 创建五子棋游戏控制器 `GomokuGame.cs` in `Assets/Scripts/Games/Gomoku/GomokuGame.cs`
- [ ] T035 [US3] 创建五子棋棋盘 `GomokuBoard.cs` in `Assets/Scripts/Games/Gomoku/GomokuBoard.cs`
- [ ] T036 [P] [US3] 创建棋子预制体（黑子/白子）in `Assets/Prefabs/Games/Gomoku/`
- [ ] T037 [US3] 实现棋盘渲染（15x15 网格）
- [ ] T038 [US3] 实现用户落子输入处理
- [ ] T039 [US3] 创建五子棋 AI `GomokuAI.cs` in `Assets/Scripts/Games/Gomoku/GomokuAI.cs`
- [ ] T040 [US3] 实现 Minimax + Alpha-Beta 剪枝算法
- [ ] T041 [US3] 实现棋局评估函数（五连、活四、冲四、活三等）
- [ ] T042 [US3] 实现胜负判定逻辑（五子连线检测）
- [ ] T043 [US3] 添加游戏 UI（当前回合、返回按钮、重新开始）
- [ ] T044 [US3] 实现游戏结束弹窗（显示胜负、重新开始/返回主菜单）
- [ ] T045 [US3] 集成最高分/战绩保存功能

**Checkpoint**: 五子棋游戏完整可玩，可独立测试

---

## Phase 6: User Story 4 - 游戏进度保存 (Priority: P3)

**Goal**: 用户的游戏进度和最高分自动保存，重启应用后可查看

**Independent Test**: 玩游戏得分 → 关闭应用 → 重新打开 → 查看最高分

### Implementation for User Story 4

- [ ] T046 [US4] 扩展 `PlayerData` 数据结构支持多游戏进度 in `Assets/Scripts/Data/SaveManager.cs`
- [ ] T047 [US4] 实现消消乐最高分保存和显示
- [ ] T048 [P] [US4] 实现五子棋战绩保存和显示
- [ ] T049 [US4] 在主菜单游戏卡片上显示最高分/战绩
- [ ] T050 [US4] 实现应用暂停时自动保存（OnApplicationPause）
- [ ] T051 [US4] 实现设置保存（音效/音乐开关）

**Checkpoint**: 进度保存功能完整，数据持久化可靠

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: 全局优化和完善

- [ ] T052 [P] 添加加载界面/过渡动画
- [ ] T053 [P] 优化场景切换性能（异步加载）
- [ ] T054 [P] 添加/调整游戏音效和背景音乐
- [ ] T055 统一 UI 风格和配色
- [ ] T056 [P] 安卓设备适配测试和修复
- [ ] T057 [P] 更新项目文档 `doc/项目概述.md` 和 `doc/技术方案.md`
- [ ] T058 更新 `doc/CHANGELOG.md` 记录版本变更
- [ ] T059 构建并测试 APK

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: 无依赖，可立即开始
- **Phase 2 (Foundational)**: 依赖 Phase 1 完成 - **阻塞所有用户故事**
- **Phase 3-6 (User Stories)**: 依赖 Phase 2 完成
  - US1 (主菜单) 和 US2 (消消乐) 都是 P1，可并行开发
  - US3 (五子棋) 是 P2，可在 US1 完成后开始
  - US4 (进度保存) 是 P3，可在 US2/US3 完成后开始
- **Phase 7 (Polish)**: 依赖所有用户故事完成

### User Story Dependencies

| Story | Priority | 依赖 | 可并行 |
|-------|----------|------|--------|
| US1 (主菜单) | P1 | Phase 2 | ✅ |
| US2 (消消乐) | P1 | Phase 2 | ✅ (与 US1 并行) |
| US3 (五子棋) | P2 | Phase 2, US1 (场景切换) | ⚠️ 需要 US1 的导航功能 |
| US4 (进度保存) | P3 | Phase 2, US2, US3 | ❌ 需要游戏完成 |

### Within Each User Story

- 场景文件先于脚本
- 数据模型先于控制器
- 核心逻辑先于 UI
- 功能完成后集成测试

### Parallel Opportunities

**Phase 1 并行**:
```
T002 (Match3目录) | T003 (Gomoku目录) | T004 (MainMenu目录) | T005 (Prefabs目录) | T006 (Resources目录)
```

**Phase 2 并行**:
```
T010 (GameState枚举) | T011 (GameResult枚举)
```

**Phase 3 (US1) 并行**:
```
T017 (GameCard.cs) | T018 (GameCard.prefab)
T020 (Match3Config) | T021 (GomokuConfig)
```

**Phase 5 (US3) 并行**:
```
T036 (棋子预制体) 可与其他任务并行
```

---

## Implementation Strategy

### MVP First (User Story 1 + 2)

1. ✅ Complete Phase 1: Setup
2. ✅ Complete Phase 2: Foundational
3. ✅ Complete Phase 3: User Story 1 (主菜单)
4. ✅ Complete Phase 4: User Story 2 (消消乐)
5. **STOP and VALIDATE**: 测试主菜单导航和消消乐游戏
6. Deploy/Demo: 可发布 MVP 版本

### Incremental Delivery

| 版本 | 包含内容 | 可交付 |
|------|----------|--------|
| v1.1.0 | Setup + Foundational + US1 + US2 | ✅ MVP |
| v1.2.0 | + US3 (五子棋) | ✅ 双游戏版本 |
| v1.3.0 | + US4 (进度保存) | ✅ 完整版本 |
| v1.4.0 | + Polish | ✅ 发布版本 |

---

## Notes

- [P] 任务 = 不同文件，无依赖，可并行
- [Story] 标签映射到具体用户故事，便于追踪
- 每个用户故事应可独立完成和测试
- 每个任务或逻辑组完成后提交代码
- 在任何 Checkpoint 处可停下验证功能
- 避免：模糊任务、同文件冲突、跨故事依赖
