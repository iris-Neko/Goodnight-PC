using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GoodNightPC.ViewModels;
using GoodNightPC.Views;

namespace GoodNightPC;

public partial class App : Application
{
    private TrayIcon? _trayIcon;
    private EventWaitHandle? _showSignal;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            
            desktop.MainWindow = mainWindow;
            
            // 设置托盘图标
            SetupTrayIcon(desktop, mainWindow);

            // 单实例唤醒信号监听
            SetupShowSignal(desktop, mainWindow);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupTrayIcon(IClassicDesktopStyleApplicationLifetime desktop, MainWindow mainWindow)
    {
        _trayIcon = new TrayIcon();
        
        // 使用内置图标或从文件加载
        try
        {
            var iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "icon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                _trayIcon.Icon = new WindowIcon(iconPath);
            }
        }
        catch
        {
            // 如果加载失败，继续使用默认图标
        }
        
        _trayIcon.ToolTipText = "定时关机软件";

        var trayMenu = new NativeMenu();
        
        // 显示主窗口
        var showMenuItem = new NativeMenuItem
        {
            Header = "显示窗口"
        };
        showMenuItem.Click += (s, e) =>
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
        };
        trayMenu.Add(showMenuItem);
        
        trayMenu.Add(new NativeMenuItemSeparator());
        
        // 快速休眠
        var hibernateMenuItem = new NativeMenuItem
        {
            Header = "💤 立即休眠"
        };
        hibernateMenuItem.Click += (s, e) =>
        {
            if (mainWindow.DataContext is MainWindowViewModel vm)
            {
                vm.QuickHibernate();
            }
        };
        trayMenu.Add(hibernateMenuItem);
        
        // 快速关机
        var shutdownMenuItem = new NativeMenuItem
        {
            Header = "🔌 立即关机"
        };
        shutdownMenuItem.Click += (s, e) =>
        {
            if (mainWindow.DataContext is MainWindowViewModel vm)
            {
                vm.QuickShutdown();
            }
        };
        trayMenu.Add(shutdownMenuItem);
        
        // 快速重启
        var restartMenuItem = new NativeMenuItem
        {
            Header = "🔄 立即重启"
        };
        restartMenuItem.Click += (s, e) =>
        {
            if (mainWindow.DataContext is MainWindowViewModel vm)
            {
                vm.QuickRestart();
            }
        };
        trayMenu.Add(restartMenuItem);
        
        trayMenu.Add(new NativeMenuItemSeparator());
        
        // 退出
        var exitMenuItem = new NativeMenuItem
        {
            Header = "退出"
        };
        exitMenuItem.Click += (s, e) =>
        {
            _trayIcon?.Dispose();
            desktop.Shutdown();
        };
        trayMenu.Add(exitMenuItem);
        
        _trayIcon.Menu = trayMenu;
        
        // 双击托盘图标显示窗口
        _trayIcon.Clicked += (s, e) =>
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
        };
        
        _trayIcon.IsVisible = true;
    }

    private void SetupShowSignal(IClassicDesktopStyleApplicationLifetime desktop, MainWindow mainWindow)
    {
        try
        {
            _showSignal = new EventWaitHandle(false, EventResetMode.AutoReset, Program.ShowSignalName);
            var thread = new Thread(() =>
            {
                while (true)
                {
                    _showSignal.WaitOne();
                    // 回到 UI 线程显示窗口
                    Dispatcher.UIThread.Post(() =>
                    {
                        mainWindow.Show();
                        mainWindow.WindowState = WindowState.Normal;
                        mainWindow.Activate();
                    });
                }
            })
            {
                IsBackground = true
            };
            thread.Start();
        }
        catch
        {
            // 信号创建失败不影响主流程
        }
    }
}

