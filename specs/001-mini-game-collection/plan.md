# Implementation Plan: 小游戏集合平台

**Branch**: `001-mini-game-collection` | **Date**: 2025-12-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-mini-game-collection/spec.md`

## Summary

将现有的消消乐游戏扩展为小游戏集合平台，新增主菜单导航系统和五子棋游戏，支持游戏进度保存，适配安卓设备。

## Technical Context

**Language/Version**: C# (Unity 2022.x LTS)  
**Primary Dependencies**: Unity Engine, TextMeshPro, Unity UI  
**Storage**: PlayerPrefs (本地持久化)  
**Testing**: Unity Test Framework (Play Mode Tests)  
**Target Platform**: Android 8.0+ (API Level 26+), Portrait 模式  
**Project Type**: Mobile Game (Unity)  
**Performance Goals**: 60 FPS 稳定运行，场景切换 < 3秒  
**Constraints**: 内存 < 200MB，APK 大小 < 50MB，离线可用  
**Scale/Scope**: 2-5 个小游戏，单机本地存储

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. CHANGELOG-First Commits | ✅ PASS | 所有提交将先更新 doc/CHANGELOG.md |
| II. Commit Message 规范 | ✅ PASS | 使用 `<type>: <subject>` 格式 |
| III. Unity 项目结构 | ✅ PASS | 遵循现有目录结构，新增 Games/ 子目录 |
| IV. 代码质量 | ✅ PASS | 单例模式、XML 注释、调试日志、空引用检查 |
| V. 跨平台兼容 | ✅ PASS | UI 适配、触摸输入、相机自适应 |

## Project Structure

### Documentation (this feature)

```text
specs/001-mini-game-collection/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (N/A for Unity game)
└── tasks.md             # Phase 2 output
```

### Source Code (repository root)

```text
Assets/
├── Scripts/
│   ├── Core/                    # 核心游戏逻辑 (现有)
│   │   ├── Board.cs
│   │   ├── CameraScaler.cs
│   │   ├── GameManager.cs
│   │   ├── InputManager.cs
│   │   ├── MatchFinder.cs
│   │   ├── MatchProcessor.cs
│   │   └── Tile.cs
│   ├── Games/                   # 新增：各游戏模块
│   │   ├── Common/              # 游戏通用接口和基类
│   │   │   ├── IGame.cs
│   │   │   └── GameBase.cs
│   │   ├── Match3/              # 消消乐游戏 (重构现有代码)
│   │   │   ├── Match3Game.cs
│   │   │   └── Match3Board.cs
│   │   └── Gomoku/              # 五子棋游戏 (新增)
│   │       ├── GomokuGame.cs
│   │       ├── GomokuBoard.cs
│   │       └── GomokuAI.cs
│   ├── UI/                      # UI 管理 (现有 + 扩展)
│   │   ├── MainMenu/            # 新增：主菜单
│   │   │   ├── MainMenuUI.cs
│   │   │   └── GameCard.cs
│   │   └── ...                  # 现有 UI 脚本
│   ├── Data/                    # 数据管理 (现有 + 扩展)
│   │   ├── SaveManager.cs       # 扩展支持多游戏
│   │   └── GameConfig.cs        # 新增：游戏配置
│   ├── Audio/                   # 音频管理 (现有)
│   └── Effects/                 # 特效管理 (现有)
├── Prefabs/
│   ├── Games/                   # 新增：游戏预制体
│   │   ├── Match3/
│   │   └── Gomoku/
│   └── UI/                      # 新增：UI 预制体
│       └── GameCard.prefab
├── Scenes/
│   ├── MainMenu.unity           # 新增：主菜单场景
│   ├── Match3Game.unity         # 重命名现有场景
│   └── GomokuGame.unity         # 新增：五子棋场景
└── Resources/
    └── GameConfigs/             # 新增：游戏配置资源
```

**Structure Decision**: 采用模块化游戏架构，每个游戏独立场景和脚本目录，通过 `IGame` 接口统一管理，便于后续扩展更多游戏。

## Complexity Tracking

> 无 Constitution 违规，无需记录
