@echo off
chcp 65001 >nul
echo ========================================
echo    定时关机软件 - Avalonia Native AOT
echo    编译脚本 v2.0
echo ========================================
echo.

REM 检查 .NET SDK
echo [1/5] 检查 .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ 错误：未找到 .NET SDK
    echo 请安装 .NET 10 SDK: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✓ .NET SDK 已安装
echo.

REM 清理旧的编译文件
echo [2/5] 清理旧的编译文件...
if exist bin\Release rmdir /s /q bin\Release
if exist obj\Release rmdir /s /q obj\Release
echo ✓ 清理完成
echo.

REM 恢复 NuGet 包
echo [3/5] 恢复 NuGet 包...
dotnet restore
if %errorlevel% neq 0 (
    echo ❌ 包恢复失败
    pause
    exit /b 1
)
echo ✓ 包恢复完成
echo.

REM 编译 Native AOT 版本
echo [4/5] 编译 Native AOT 版本（这可能需要几分钟）...
echo 正在编译，请稍候...
dotnet publish -c Release -r win-x64 /p:PublishAot=true
if %errorlevel% neq 0 (
    echo ❌ 编译失败
    pause
    exit /b 1
)
echo ✓ 编译完成
echo.

REM 显示结果
echo [5/5] 编译结果
echo ========================================
echo.

set OUTPUT_DIR=bin\Release\net10.0\win-x64\publish
set EXE_NAME=定时关机软件.exe

if exist "%OUTPUT_DIR%\%EXE_NAME%" (
    echo ✓ 编译成功！
    echo.
    echo 📦 可执行文件位置：
    echo    %OUTPUT_DIR%\%EXE_NAME%
    echo.
    
    REM 获取文件大小
    for %%A in ("%OUTPUT_DIR%\%EXE_NAME%") do (
        set SIZE=%%~zA
    )
    
    REM 转换为 MB
    set /a SIZE_MB=!SIZE! / 1048576
    echo 📊 文件大小：约 !SIZE_MB! MB
    echo.
    
    echo 🚀 特性：
    echo    ⚡ Native AOT 编译
    echo    💾 单文件可执行
    echo    🔔 系统托盘支持
    echo    ⚠️  无需 .NET Runtime
    echo    🎯 毫秒级启动
    echo.
    
    REM 询问是否运行
    set /p RUN="是否立即运行程序？(Y/N): "
    if /i "%RUN%"=="Y" (
        echo.
        echo 正在启动程序...
        start "" "%OUTPUT_DIR%\%EXE_NAME%"
    )
) else (
    echo ❌ 错误：未找到编译输出文件
    echo 预期位置：%OUTPUT_DIR%\%EXE_NAME%
)

echo.
echo ========================================
echo 编译脚本执行完成
echo ========================================
pause
