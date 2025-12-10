# Data Model: 小游戏集合平台

**Feature**: 001-mini-game-collection  
**Date**: 2025-12-10

## 核心实体

### 1. GameInfo（游戏信息）

代表一个可玩的小游戏的元数据。

| 字段 | 类型 | 描述 | 约束 |
|------|------|------|------|
| GameId | string | 游戏唯一标识 | 必填，唯一 |
| GameName | string | 游戏显示名称 | 必填 |
| Description | string | 游戏简介 | 可选 |
| IconSprite | Sprite | 游戏图标 | 必填 |
| SceneName | string | 游戏场景名称 | 必填 |
| IsUnlocked | bool | 是否已解锁 | 默认 true |
| SortOrder | int | 排序顺序 | 默认 0 |

**状态转换**: 无（静态配置）

---

### 2. PlayerData（玩家数据）

代表玩家的持久化数据。

| 字段 | 类型 | 描述 | 约束 |
|------|------|------|------|
| HighScores | Dictionary<string, int> | 各游戏最高分 | Key = GameId |
| TotalPlayTime | float | 总游戏时长（秒） | >= 0 |
| SoundEnabled | bool | 音效开关 | 默认 true |
| MusicEnabled | bool | 音乐开关 | 默认 true |
| LastPlayedGame | string | 上次玩的游戏 | GameId |
| FirstLaunchDate | string | 首次启动日期 | ISO 8601 |

**持久化**: PlayerPrefs (JSON 序列化)

---

### 3. GameSession（游戏会话）

代表一次游戏过程的运行时状态。

| 字段 | 类型 | 描述 | 约束 |
|------|------|------|------|
| GameId | string | 当前游戏标识 | 必填 |
| CurrentScore | int | 当前分数 | >= 0 |
| GameState | GameState | 游戏状态 | 枚举 |
| StartTime | DateTime | 开始时间 | 自动记录 |
| PauseTime | float | 暂停累计时长 | >= 0 |

**状态转换**:
```
[NotStarted] -> [Playing] -> [Paused] -> [Playing]
                    |            |
                    v            v
               [GameOver]   [GameOver]
                    |
                    v
               [NotStarted] (重新开始)
```

---

### 4. Match3GameData（消消乐游戏数据）

消消乐游戏的特定数据。

| 字段 | 类型 | 描述 | 约束 |
|------|------|------|------|
| BoardWidth | int | 棋盘宽度 | 5-10 |
| BoardHeight | int | 棋盘高度 | 5-10 |
| TileTypes | int | 方块类型数量 | 4-8 |
| MovesRemaining | int | 剩余步数 | >= 0 |
| TargetScore | int | 目标分数 | > 0 |
| CurrentLevel | int | 当前关卡 | >= 1 |
| Tiles | Tile[,] | 棋盘状态 | 二维数组 |

---

### 5. GomokuGameData（五子棋游戏数据）

五子棋游戏的特定数据。

| 字段 | 类型 | 描述 | 约束 |
|------|------|------|------|
| BoardSize | int | 棋盘大小 | 固定 15x15 |
| Board | int[,] | 棋盘状态 | 0=空, 1=黑, 2=白 |
| CurrentPlayer | int | 当前玩家 | 1=玩家(黑), 2=AI(白) |
| MoveHistory | List<Vector2Int> | 落子历史 | 有序列表 |
| AIDifficulty | int | AI 难度 | 1-3 |
| Winner | int | 获胜方 | 0=未结束, 1=玩家, 2=AI, 3=平局 |

---

## 枚举类型

### GameState

```csharp
public enum GameState
{
    NotStarted = 0,
    Playing = 1,
    Paused = 2,
    GameOver = 3
}
```

### GameResult

```csharp
public enum GameResult
{
    None = 0,
    Win = 1,
    Lose = 2,
    Draw = 3
}
```

---

## 实体关系

```
┌─────────────┐       ┌─────────────┐
│  GameInfo   │       │ PlayerData  │
│  (配置)     │       │  (持久化)   │
└──────┬──────┘       └──────┬──────┘
       │                     │
       │  1:N                │ 1:1
       │                     │
       v                     v
┌─────────────┐       ┌─────────────┐
│ GameSession │<------│  HighScore  │
│  (运行时)   │       │  (per game) │
└──────┬──────┘       └─────────────┘
       │
       │ 1:1
       v
┌─────────────────────────────────┐
│     Game-Specific Data          │
│  (Match3GameData / GomokuData)  │
└─────────────────────────────────┘
```

---

## 验证规则

### GameInfo
- GameId 必须唯一且非空
- SceneName 必须对应存在的场景
- IconSprite 必须非空

### PlayerData
- HighScores 的 Key 必须是有效的 GameId
- 日期格式必须符合 ISO 8601

### GameSession
- GameState 转换必须遵循状态机规则
- CurrentScore 只能增加，不能减少（游戏内）

### Match3GameData
- BoardWidth 和 BoardHeight 必须在 5-10 范围内
- TileTypes 必须在 4-8 范围内
- MovesRemaining 不能为负数

### GomokuGameData
- BoardSize 固定为 15
- Board 数组值只能是 0, 1, 2
- CurrentPlayer 只能是 1 或 2
