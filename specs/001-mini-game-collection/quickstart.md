# Quickstart: 小游戏集合平台

**Feature**: 001-mini-game-collection  
**Date**: 2025-12-10

## 开发环境要求

- **Unity**: 2022.3 LTS 或更高版本
- **Android SDK**: API Level 26+ (Android 8.0+)
- **JDK**: 11 或更高版本
- **IDE**: Visual Studio / VS Code / Rider

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/whalesmeng/unityPawzy.git
cd unityPawzy/PawzyPop
git checkout 001-mini-game-collection
```

### 2. 打开 Unity 项目

1. 启动 Unity Hub
2. 点击 "Add" 添加项目
3. 选择 `PawzyPop` 目录
4. 使用 Unity 2022.3 LTS 打开

### 3. 项目结构

```
Assets/
├── Scripts/
│   ├── Core/           # 核心逻辑
│   ├── Games/          # 游戏模块
│   │   ├── Common/     # 通用接口
│   │   ├── Match3/     # 消消乐
│   │   └── Gomoku/     # 五子棋
│   ├── UI/             # UI 脚本
│   ├── Data/           # 数据管理
│   ├── Audio/          # 音频管理
│   └── Effects/        # 特效管理
├── Scenes/
│   ├── MainMenu.unity  # 主菜单
│   ├── Match3Game.unity
│   └── GomokuGame.unity
└── Prefabs/            # 预制体
```

### 4. 运行项目

1. 打开 `Assets/Scenes/MainMenu.unity`
2. 点击 Play 按钮运行
3. 在主菜单选择游戏进入

### 5. 构建 APK

1. **File → Build Settings**
2. 选择 **Android** 平台
3. 点击 **Switch Platform**
4. 配置 Player Settings:
   - Company Name: MengsCompany
   - Product Name: PawzyPop
   - Package Name: com.meng.pawzy
   - Minimum API Level: 26
5. 点击 **Build** 生成 APK

## 开发指南

### 添加新游戏

1. 创建游戏脚本目录：`Assets/Scripts/Games/YourGame/`

2. 实现 `IGame` 接口：

```csharp
namespace PawzyPop.Games.YourGame
{
    public class YourGameController : GameBase
    {
        public override string GameId => "your_game";
        public override string GameName => "你的游戏";
        
        public override void Initialize()
        {
            // 初始化逻辑
        }
        
        public override void Pause()
        {
            // 暂停逻辑
        }
        
        public override void Resume()
        {
            // 恢复逻辑
        }
    }
}
```

3. 创建游戏场景：`Assets/Scenes/YourGame.unity`

4. 注册游戏到 GameConfig

### 修改现有游戏

- 消消乐：`Assets/Scripts/Games/Match3/`
- 五子棋：`Assets/Scripts/Games/Gomoku/`

### 调试技巧

```csharp
// 使用调试日志
Debug.Log($"[YourGame] 状态: {gameState}");

// 条件编译
#if UNITY_EDITOR
    // 仅编辑器执行的代码
#endif
```

## 测试

### 编辑器测试

1. 打开对应场景
2. 点击 Play 按钮
3. 使用鼠标模拟触摸

### 真机测试

1. 连接 Android 设备
2. 开启 USB 调试
3. **File → Build And Run**

### 查看日志

```bash
# 使用 adb logcat 查看日志
adb logcat -s Unity
```

## 常见问题

### Q: 场景切换卡顿？
A: 检查是否有大资源未异步加载，使用 `SceneManager.LoadSceneAsync`

### Q: UI 在某些设备上显示异常？
A: 检查 Canvas Scaler 设置，确保使用 "Scale With Screen Size"

### Q: 五子棋 AI 响应慢？
A: 降低 AI 难度或减少搜索深度

## 相关文档

- [项目概述](../../doc/项目概述.md)
- [技术方案](../../doc/技术方案.md)
- [CHANGELOG](../../doc/CHANGELOG.md)
