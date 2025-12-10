<!--
  Sync Impact Report
  ==================
  Version change: 0.0.0 → 1.0.0 (Initial constitution)
  
  Modified principles: N/A (initial creation)
  
  Added sections:
  - Core Principles (5 principles)
  - Development Workflow
  - Quality Standards
  - Governance
  
  Removed sections: N/A
  
  Templates requiring updates:
  - .specify/templates/plan-template.md ✅ (no changes needed - Constitution Check section compatible)
  - .specify/templates/spec-template.md ✅ (no changes needed)
  - .specify/templates/tasks-template.md ✅ (no changes needed)
  
  Follow-up TODOs: None
-->

# PawzyPop Constitution

## Core Principles

### I. CHANGELOG-First Commits (NON-NEGOTIABLE)

每次 Git 提交之前，**必须**先更新 `doc/CHANGELOG.md` 文件，记录本次变更内容。

**规则**:
- 任何代码提交前 MUST 先更新 CHANGELOG
- CHANGELOG 格式 MUST 遵循 Keep a Changelog 规范
- 版本号 MUST 遵循语义化版本 (SemVer)

**CHANGELOG 格式**:
```markdown
## [版本号] - YYYY-MM-DD

### 新增
- 新功能描述

### 修复
- Bug 修复描述

### 变更
- 变更内容描述
```

### II. Commit Message 规范

所有 Git 提交信息 MUST 遵循规范格式。

**格式**: `<type>: <subject>`

**类型 (type)**:
- `feat`: 新功能
- `fix`: Bug 修复
- `docs`: 文档变更
- `style`: 代码格式（不影响功能）
- `refactor`: 重构
- `perf`: 性能优化
- `test`: 测试相关
- `chore`: 构建/工具变更

### III. Unity 项目结构

项目结构 MUST 遵循 Unity 最佳实践。

**目录规范**:
- `Assets/Scripts/` - 按功能模块划分子目录 (Core, UI, Data, Effects, Audio)
- `Assets/Prefabs/` - 预制体文件
- `Assets/Resources/` - 运行时加载资源
- `Assets/Scenes/` - 场景文件
- `doc/` - 项目文档

**命名空间**:
- 所有脚本 MUST 使用 `PawzyPop` 命名空间
- 子模块使用对应子命名空间 (PawzyPop.Core, PawzyPop.UI 等)

### IV. 代码质量

代码 MUST 保持可读性和可维护性。

**规则**:
- 单例模式管理类 MUST 使用 `Instance` 属性
- 公共方法 MUST 添加 XML 文档注释
- 关键逻辑 MUST 添加调试日志 (Debug.Log)
- 空引用检查 MUST 在使用前执行

### V. 跨平台兼容

游戏 MUST 支持多平台运行。

**规则**:
- UI 布局 MUST 适配不同屏幕比例
- 输入处理 MUST 同时支持鼠标和触摸
- 相机设置 MUST 使用自适应缩放

## Development Workflow

### 提交流程

1. 完成代码修改
2. **更新 `doc/CHANGELOG.md`** (必须)
3. 运行 Unity 编译检查
4. `git add -A`
5. `git commit -m "<type>: <subject>"`
6. `git push`

### 版本发布流程

1. 更新 CHANGELOG 中的版本号
2. 更新 ProjectSettings 中的版本信息
3. 构建目标平台包
4. 创建 Git tag
5. 推送到远程仓库

## Quality Standards

### 构建检查

- 代码 MUST 通过 Unity 编译无错误
- 场景 MUST 能够正常运行
- 目标平台构建 MUST 成功

### 文档要求

- 新功能 MUST 更新 `doc/项目概述.md`
- 架构变更 MUST 更新 `doc/技术方案.md`
- 所有变更 MUST 记录在 `doc/CHANGELOG.md`

## Governance

- Constitution 优先于所有其他实践
- 修改 Constitution 需要文档记录和审批
- 所有 PR/代码审查 MUST 验证是否符合 Constitution
- 违反 Constitution 的代码 MUST NOT 合并

**Amendment Process**:
1. 提出修改建议
2. 记录修改原因
3. 更新 Constitution 版本号
4. 同步更新相关模板和文档

**Version**: 1.0.0 | **Ratified**: 2025-12-10 | **Last Amended**: 2025-12-10
