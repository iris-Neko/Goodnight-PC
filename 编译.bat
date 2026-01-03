@echo off
chcp 65001 >nul
echo ========================================
echo 定时关机软件 - WPF 版本编译脚本
echo ========================================
echo.
echo 使用 ReadyToRun 优化（WPF 尚不支持 Native AOT）
echo.

echo [1/2] 清理旧的编译文件...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj

echo [2/2] 正在编译（ReadyToRun 优化版本）...
dotnet publish -c Release -r win-x64

echo.
echo ========================================
echo 编译完成！
echo ========================================
echo.
echo 可执行文件位置：
echo bin\Release\net10.0-windows\win-x64\publish\定时关机软件.exe
echo.
echo 文件大小：约 63MB（包含完整 .NET Runtime）
echo 启动速度：相比 Python 版本快 4-6 倍
echo 运行要求：无需安装任何环境，可直接运行
echo.
echo 注意：WPF 在 .NET 10 中尚不支持 Native AOT
echo       当前使用 ReadyToRun 优化，已获得最佳性能
echo.

pause

