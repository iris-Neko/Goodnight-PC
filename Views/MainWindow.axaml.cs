using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Layout;
using GoodNightPC.ViewModels;

namespace GoodNightPC.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadIcon();
        AddTitleIcon();
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

    private void AddTitleIcon()
    {
        try
        {
            // 标题区域是第一个 Border
            var scrollViewer = this.FindControl<ScrollViewer>("PART_ScrollViewer");
            if (scrollViewer?.Content is StackPanel mainStack && mainStack.Children.Count > 0)
            {
                if (mainStack.Children[0] is Border titleBorder)
                {
                    var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "icon.png");
                    if (File.Exists(iconPath))
                    {
                        var stackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        var image = new Image
                        {
                            Source = new Bitmap(iconPath),
                            Width = 32,
                            Height = 32,
                            Margin = new Thickness(0, 0, 12, 0)
                        };

                        var textBlock = new TextBlock
                        {
                            Text = "定时关机软件",
                            FontSize = 20,
                            FontWeight = Avalonia.Media.FontWeight.Bold,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        stackPanel.Children.Add(image);
                        stackPanel.Children.Add(textBlock);
                        titleBorder.Child = stackPanel;
                    }
                }
            }
        }
        catch
        {
            // 添加图标失败不影响运行
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

