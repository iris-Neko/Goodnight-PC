# 定时关机软件 - C# WPF 版本

一个简洁高效的 Windows 定时关机工具，使用 .NET 10 + WPF 开发。

## ✨ 特性

- ⚡ **极速启动** - 启动时间约 0.15 秒
- 🎨 **现代化界面** - WPF 设计，简洁美观
- ⏰ **精确定时** - 支持小时/分钟/秒三级设置
- 💤 **多种模式** - 休眠、强制休眠、关机、重启、强制关机
- 📦 **单文件 exe** - 真正的单个可执行文件（69 MB）
- 🎯 **实时倒计时** - 清晰显示剩余时间
- 📋 **操作日志** - 记录所有操作
- ⚠️ **无需依赖** - 无需安装 .NET Runtime

> **注意**：由于 WPF 框架的限制，界面中的 Emoji 图标显示为黑白样式（这是 WPF 的已知限制，非程序问题）。

## 🚀 快速开始

### 编译和运行

```powershell
# 开发模式运行
dotnet run

# 编译 Release 版本
dotnet publish -c Release -r win-x64
```

### 运行编译后的程序

```
bin\Release\net10.0-windows\win-x64\publish\定时关机软件.exe
```

**注意**：发布时需要包含 publish 文件夹中的所有文件（exe + dll）

## 📋 功能说明

1. 设置关机时间（小时、分钟、秒）
2. 选择关机模式：
   - 😴 休眠
   - 💤 强制休眠（不保存数据）
   - 🔌 关机
   - 🔄 重启
   - ⚠️ 强制关机（不保存数据）
3. 点击"开始定时"
4. 可随时"取消定时"

## 📊 性能对比

| 指标 | Python 版 | C# WPF 版 |
|------|-----------|-----------|
| 启动速度 | 2-3 秒 | ~0.15 秒 |
| 性能提升 | - | **13-20 倍** |
| 文件大小 | ~30 MB | ~69 MB（单文件） |
| 运行依赖 | 需要 Python | 无需任何依赖 |
| UI 框架 | Qt (彩色 Emoji) | WPF (黑白 Emoji) |
| 打包方式 | 多文件 | 真正单个 exe |

## 🛠️ 技术栈

- .NET 10.0
- WPF (Windows Presentation Foundation)
- C# 10
- ReadyToRun 优化

## 📝 项目结构

```
├── GoodNightPC.csproj       # 项目配置
├── Program.cs               # 程序入口
├── App.xaml                 # WPF 应用定义
├── App.xaml.cs              
├── MainWindow.xaml          # 主窗口界面
├── MainWindow.xaml.cs       # 主窗口逻辑
├── icon.ico                 # 应用图标
└── README.md                # 说明文档
```

## ⚙️ 关键配置

项目使用 ReadyToRun 优化以获得最佳启动性能：

```xml
<PublishSingleFile>true</PublishSingleFile>
<PublishReadyToRun>true</PublishReadyToRun>
<SelfContained>true</SelfContained>
<ApplicationIcon>icon.ico</ApplicationIcon>
```

**注意**：WPF 在 .NET 10 中尚不支持 Native AOT，当前使用 ReadyToRun 作为最佳方案。

## 📦 打包分发

程序已打包为**真正的单个 exe 文件**：

- `定时关机软件.exe` (69 MB) - 包含所有依赖
- 可以直接运行，无需任何其他文件
- 复制即可使用，真正的绿色软件

**分发时只需要这一个 exe 文件！** 📦

## ⚠️ 注意事项

- 强制关机/休眠会导致未保存的数据丢失
- 执行关机操作需要管理员权限
- 关闭程序时如果定时器在运行会提示确认
- **Emoji 显示为黑白**：这是 WPF 框架的限制，不支持彩色 Emoji 渲染

## ❓ 常见问题

### 为什么 Emoji 是黑白的？

WPF 框架使用的文本渲染引擎不支持彩色字体格式（COLR/CPAL），因此即使系统有彩色 Emoji 字体，WPF 也只能渲染为黑白轮廓。这是 WPF 的已知限制，不是程序问题。

Python 版本使用 Qt 框架，Qt 支持彩色 Emoji，所以显示为彩色。

### 为什么选择 WPF 而不是 Qt？

- ✅ 启动速度更快（0.15 秒 vs 2-3 秒）
- ✅ Windows 原生框架，系统集成更好
- ✅ 可以打包为真正的单个 exe 文件
- ✅ 文件大小更合理（69 MB vs 30 MB 但需要 Python）
- ❌ 不支持彩色 Emoji（可接受的小缺陷）

## 📄 许可证

本项目仅供学习和个人使用。

---

**开发日期**：2026-01-03  
**版本**：1.0.0
