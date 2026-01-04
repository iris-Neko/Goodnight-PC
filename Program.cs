using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.ReactiveUI;

namespace GoodNightPC;

class Program
{
    private static Mutex? _singleInstanceMutex;
    private const string MutexName = @"Global\GoodNightPC_SingleInstance_Mutex";
    internal const string ShowSignalName = @"Global\GoodNightPC_ShowSignal";

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindowW(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_RESTORE = 9;

    [STAThread]
    public static void Main(string[] args)
    {
        // 使用命名互斥锁确保仅单实例运行
        _singleInstanceMutex = new Mutex(initiallyOwned: true, MutexName, out bool createdNew);

        if (!createdNew)
        {
            BringExistingToFront();
            SignalExistingInstance();
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    private static void BringExistingToFront()
    {
        try
        {
            // 窗口标题与 XAML 中的 Title 保持一致
            var hWnd = FindWindowW(null, "GoodNight PC");
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }
        }
        catch
        {
            // 忽略唤醒失败，直接退出
        }
    }

    private static void SignalExistingInstance()
    {
        try
        {
#pragma warning disable CA1416
            using var evt = EventWaitHandle.OpenExisting(ShowSignalName);
            evt.Set();
#pragma warning restore CA1416
        }
        catch
        {
            // 如果唤醒信号失败，继续退出即可
        }
    }
}
