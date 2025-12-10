# Changelog

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
