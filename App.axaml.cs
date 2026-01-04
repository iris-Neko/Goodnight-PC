using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GoodNightPC.Models;
using GoodNightPC.ViewModels;
using GoodNightPC.Views;

namespace GoodNightPC;

public partial class App : Application
{
    private TrayIcon? _trayIcon;
    private EventWaitHandle? _showSignal;
    private NativeMenuItem? _showMenuItem;
    private NativeMenuItem? _hibernateMenuItem;
    private NativeMenuItem? _shutdownMenuItem;
    private NativeMenuItem? _restartMenuItem;
    private NativeMenuItem? _exitMenuItem;
    private readonly LocalizationService _loc = LocalizationService.Instance;

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

            _loc.LanguageChanged += UpdateTrayTexts;
            UpdateTrayTexts();
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
        
        _trayIcon.ToolTipText = _loc.GetString("Tray_Tooltip");

        var trayMenu = new NativeMenu();
        
        // 显示主窗口
        _showMenuItem = new NativeMenuItem
        {
            Header = _loc.GetString("Tray_Show")
        };
        _showMenuItem.Click += (s, e) =>
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
        };
        trayMenu.Add(_showMenuItem);
        
        trayMenu.Add(new NativeMenuItemSeparator());
        
        // 快速休眠
        _hibernateMenuItem = new NativeMenuItem
        {
            Header = _loc.GetString("Tray_QuickHibernate")
        };
        _hibernateMenuItem.Click += (s, e) =>
        {
            if (mainWindow.DataContext is MainWindowViewModel vm)
            {
                vm.QuickHibernate();
            }
        };
        trayMenu.Add(_hibernateMenuItem);
        
        // 快速关机
        _shutdownMenuItem = new NativeMenuItem
        {
            Header = _loc.GetString("Tray_QuickShutdown")
        };
        _shutdownMenuItem.Click += (s, e) =>
        {
            if (mainWindow.DataContext is MainWindowViewModel vm)
            {
                vm.QuickShutdown();
            }
        };
        trayMenu.Add(_shutdownMenuItem);
        
        // 快速重启
        _restartMenuItem = new NativeMenuItem
        {
            Header = _loc.GetString("Tray_QuickRestart")
        };
        _restartMenuItem.Click += (s, e) =>
        {
            if (mainWindow.DataContext is MainWindowViewModel vm)
            {
                vm.QuickRestart();
            }
        };
        trayMenu.Add(_restartMenuItem);
        
        trayMenu.Add(new NativeMenuItemSeparator());
        
        // 退出
        _exitMenuItem = new NativeMenuItem
        {
            Header = _loc.GetString("Tray_Exit")
        };
        _exitMenuItem.Click += (s, e) =>
        {
            _trayIcon?.Dispose();
            desktop.Shutdown();
        };
        trayMenu.Add(_exitMenuItem);
        
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

    private void UpdateTrayTexts()
    {
        if (_trayIcon != null)
        {
            _trayIcon.ToolTipText = _loc.GetString("Tray_Tooltip");
        }

        if (_showMenuItem != null) _showMenuItem.Header = _loc.GetString("Tray_Show");
        if (_hibernateMenuItem != null) _hibernateMenuItem.Header = _loc.GetString("Tray_QuickHibernate");
        if (_shutdownMenuItem != null) _shutdownMenuItem.Header = _loc.GetString("Tray_QuickShutdown");
        if (_restartMenuItem != null) _restartMenuItem.Header = _loc.GetString("Tray_QuickRestart");
        if (_exitMenuItem != null) _exitMenuItem.Header = _loc.GetString("Tray_Exit");
    }
}

