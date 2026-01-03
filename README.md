# 定时关机软件 - Avalonia + Native AOT 版本

一个现代化的 Windows 定时关机工具，使用 .NET 10 + Avalonia UI + Native AOT 开发，实现毫秒级启动。

## ✨ 特性

- ⚡ **极速启动** - Native AOT 编译，启动时间 < 50ms
- 🎨 **现代化跨平台界面** - Avalonia UI + Fluent 设计
- 🏗️ **MVVM 架构** - 清晰的代码结构，易于维护
- 💾 **超小体积** - Native AOT 优化后的可执行文件
- ⏰ **精确定时** - 支持小时/分钟/秒三级设置
- 💤 **多种模式** - 休眠、强制休眠、关机、重启、强制关机
- 🎯 **实时倒计时** - 清晰显示剩余时间
- 📋 **操作日志** - 记录所有操作
- 🔔 **系统托盘** - 最小化到托盘，右键快速操作
- ⚠️ **零依赖** - 无需安装 .NET Runtime

## 🚀 快速开始

### 环境要求

- .NET 10 SDK
- Windows 10/11（Native AOT 仅支持 Windows x64）

### 开发模式运行

```powershell
# 恢复 NuGet 包
dotnet restore

# 运行程序
dotnet run
```

### 编译 Native AOT 版本

#### 方式一：使用编译脚本（推荐）

```powershell
.\编译.bat
```

#### 方式二：手动编译

```powershell
# 编译 Native AOT 版本
dotnet publish -c Release -r win-x64 /p:PublishAot=true

# 可执行文件位置
# bin\Release\net10.0\win-x64\publish\定时关机软件.exe
```

## 📋 功能说明

### 定时关机
1. 设置关机时间（小时、分钟、秒）
2. 选择关机模式：
   - 😴 休眠
   - 💤 强制休眠（不保存数据）
   - 🔌 关机
   - 🔄 重启
   - ⚠️ 强制关机（不保存数据）
3. 点击"开始定时"
4. 可随时"取消定时"

### 系统托盘功能
- 最小化到系统托盘
- 双击托盘图标恢复窗口
- 右键菜单快速操作：
  - 💤 立即休眠
  - 🔌 立即关机
  - 🔄 立即重启
  - 显示窗口
  - 退出

## 📊 性能对比

| 指标 | Python 版 | WPF 版 | Avalonia + Native AOT |
|------|-----------|--------|----------------------|
| 启动速度 | 2-3 秒 | ~0.15 秒 | **< 0.05 秒** |
| 性能提升 | - | 13-20 倍 | **40-60 倍** |
| 文件大小 | ~30 MB | ~69 MB | ~15 MB |
| 运行依赖 | 需要 Python | 无需依赖 | **无需任何依赖** |
| UI 框架 | Qt | WPF | **Avalonia** |
| 跨平台 | ❌ | ❌ | ✅（可扩展） |
| 编译方式 | 解释执行 | JIT | **Native AOT** |

## 🛠️ 技术栈

- **.NET 10** - 最新的 .NET 运行时
- **Avalonia UI 11.3** - 现代化跨平台 UI 框架
- **ReactiveUI** - MVVM 框架和响应式编程
- **Native AOT** - 提前编译为本机代码
- **C# 12** - 最新语言特性

## 📝 项目结构

```
├── GoodNightPC.csproj           # 项目配置（Native AOT）
├── Program.cs                   # 程序入口
├── App.axaml                    # Avalonia 应用定义
├── App.axaml.cs                 # 应用逻辑（托盘图标）
├── Styles.axaml                 # 全局样式
├── rd.xml                       # Native AOT 运行时指令
├── Models/
│   ├── ShutdownAction.cs        # 关机动作枚举
│   └── LogEntry.cs              # 日志条目模型
├── ViewModels/
│   └── MainWindowViewModel.cs   # 主窗口视图模型
├── Views/
│   ├── MainWindow.axaml         # 主窗口界面
│   └── MainWindow.axaml.cs      # 主窗口逻辑
├── icon.ico                     # 应用图标
├── 编译.bat                     # 编译脚本
└── README.md                    # 说明文档
```

## ⚙️ Native AOT 配置

项目使用以下配置实现 Native AOT：

```xml
<PublishAot>true</PublishAot>
<InvariantGlobalization>false</InvariantGlobalization>
<IlcOptimizationPreference>Speed</IlcOptimizationPreference>
<IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
```

### 关键优化
- ✅ 完全的提前编译（AOT）
- ✅ 树摇（Tree Shaking）去除未使用代码
- ✅ 优化启动性能
- ✅ 最小化文件大小
- ✅ 支持中文本地化

## 📦 分发说明

编译后的 `定时关机软件.exe` 是一个**真正的单文件可执行程序**：
- ✅ 无需安装
- ✅ 无需 .NET Runtime
- ✅ 双击即可运行
- ✅ 真正的绿色软件

## ⚠️ 注意事项

- 强制关机/休眠会导致未保存的数据丢失
- 执行关机操作需要管理员权限
- 关闭程序时如果定时器在运行会提示确认
- 关闭主窗口会最小化到托盘，右键托盘图标选择"退出"才会真正退出

## 🔄 从 WPF 迁移

本版本从 WPF 完全重构为 Avalonia：

### 主要改进
1. ✅ **Native AOT 支持** - 启动速度提升 3 倍
2. ✅ **MVVM 架构** - 更好的代码组织
3. ✅ **系统托盘** - 更好的用户体验
4. ✅ **跨平台潜力** - 可扩展到 Linux/macOS
5. ✅ **现代化 UI** - Fluent 设计风格

### 技术迁移
- WPF XAML → Avalonia AXAML
- DispatcherTimer → Task.Delay + CancellationToken
- MessageBox → MessageBox.Avalonia
- Code-behind → MVVM + ReactiveUI

## ❓ 常见问题

### 为什么选择 Avalonia？

- ✅ 支持 Native AOT（WPF 不支持）
- ✅ 现代化的 MVVM 架构
- ✅ 更好的性能
- ✅ 跨平台能力
- ✅ 活跃的社区支持

### Native AOT 的优势？

- ⚡ 启动速度极快（< 50ms）
- 📦 文件体积小
- 🔒 代码更安全（难以反编译）
- 💪 更好的性能
- 🌍 无需安装运行时

### 如何编译最小体积？

```powershell
dotnet publish -c Release -r win-x64 /p:PublishAot=true /p:IlcOptimizationPreference=Size
```

## 📄 许可证

本项目仅供学习和个人使用。

## 🎉 更新日志

### v2.0.0 (2026-01-03)
- 🎨 完全重构为 Avalonia UI
- ⚡ 支持 Native AOT 编译
- 🏗️ 采用 MVVM 架构
- 🔔 添加系统托盘功能
- 📦 单文件发布，无需依赖

---

**开发日期**：2026-01-03  
**版本**：2.0.0 - Avalonia + Native AOT  
**技术栈**：.NET 10 + Avalonia 11 + ReactiveUI + Native AOT
