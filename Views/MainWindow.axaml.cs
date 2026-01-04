using System;
using System.IO;
using Avalonia.Controls;
using GoodNightPC.ViewModels;

namespace GoodNightPC.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadIcon();
    }

    private void LoadIcon()
    {
        try
        {
            // 尝试从文件系统加载窗口图标
            var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
            if (File.Exists(iconPath))
            {
                Icon = new WindowIcon(iconPath);
            }
        }
        catch
        {
            // 图标加载失败不影响程序运行
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            e.Cancel = true;
            var shouldClose = await viewModel.OnClosingAsync();
            
            if (shouldClose)
            {
                // 隐藏窗口而不是关闭（托盘模式）
                Hide();
            }
        }
        
        base.OnClosing(e);
    }
}

