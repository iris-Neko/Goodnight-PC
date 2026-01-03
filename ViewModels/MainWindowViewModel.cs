using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using GoodNightPC.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace GoodNightPC.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private int _hours;
    private int _minutes;
    private int _seconds = 5;
    private ShutdownAction _selectedAction = ShutdownAction.Hibernate;
    private bool _isRunning;
    private int _countdownSeconds;
    private string _statusText = "状态：等待设置";
    private string _countdownText = "倒计时：--:--:--";
    private CancellationTokenSource? _cancellationTokenSource;

    public MainWindowViewModel()
    {
        Logs = new ObservableCollection<string>();
        
        StartCommand = ReactiveCommand.CreateFromTask(StartCountdownAsync, 
            this.WhenAnyValue(x => x.IsRunning, running => !running));
        
        StopCommand = ReactiveCommand.Create(StopCountdown, 
            this.WhenAnyValue(x => x.IsRunning));
        
        AddLog("程序启动成功");
    }

    public int Hours
    {
        get => _hours;
        set => this.RaiseAndSetIfChanged(ref _hours, value);
    }

    public int Minutes
    {
        get => _minutes;
        set => this.RaiseAndSetIfChanged(ref _minutes, value);
    }

    public int Seconds
    {
        get => _seconds;
        set => this.RaiseAndSetIfChanged(ref _seconds, value);
    }

    public ShutdownAction SelectedAction
    {
        get => _selectedAction;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAction, value);
            this.RaisePropertyChanged(nameof(IsHibernate));
            this.RaisePropertyChanged(nameof(IsForceHibernate));
            this.RaisePropertyChanged(nameof(IsShutdown));
            this.RaisePropertyChanged(nameof(IsRestart));
            this.RaisePropertyChanged(nameof(IsForceShutdown));
        }
    }

    public bool IsHibernate
    {
        get => SelectedAction == ShutdownAction.Hibernate;
        set { if (value) SelectedAction = ShutdownAction.Hibernate; }
    }

    public bool IsForceHibernate
    {
        get => SelectedAction == ShutdownAction.ForceHibernate;
        set { if (value) SelectedAction = ShutdownAction.ForceHibernate; }
    }

    public bool IsShutdown
    {
        get => SelectedAction == ShutdownAction.Shutdown;
        set { if (value) SelectedAction = ShutdownAction.Shutdown; }
    }

    public bool IsRestart
    {
        get => SelectedAction == ShutdownAction.Restart;
        set { if (value) SelectedAction = ShutdownAction.Restart; }
    }

    public bool IsForceShutdown
    {
        get => SelectedAction == ShutdownAction.ForceShutdown;
        set { if (value) SelectedAction = ShutdownAction.ForceShutdown; }
    }

    public bool IsRunning
    {
        get => _isRunning;
        set => this.RaiseAndSetIfChanged(ref _isRunning, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    public string CountdownText
    {
        get => _countdownText;
        set => this.RaiseAndSetIfChanged(ref _countdownText, value);
    }

    public ObservableCollection<string> Logs { get; }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    private async Task StartCountdownAsync()
    {
        // 验证输入
        if (Hours < 0 || Hours > 23)
        {
            await ShowMessageBox("小时数必须在 0-23 之间！", "输入错误");
            return;
        }

        if (Minutes < 0 || Minutes > 59)
        {
            await ShowMessageBox("分钟数必须在 0-59 之间！", "输入错误");
            return;
        }

        if (Seconds < 0 || Seconds > 59)
        {
            await ShowMessageBox("秒数必须在 0-59 之间！", "输入错误");
            return;
        }

        if (Hours == 0 && Minutes == 0 && Seconds == 0)
        {
            await ShowMessageBox("请设置有效的时间！", "警告");
            return;
        }

        _countdownSeconds = Hours * 3600 + Minutes * 60 + Seconds;
        IsRunning = true;
        
        UpdateCountdownDisplay();
        
        var action = SelectedAction.GetDisplayName();
        AddLog($"定时任务已开始：{Hours}小时{Minutes}分{Seconds}秒后执行【{action}】");
        
        var timeStr = FormatTimeString(_countdownSeconds);
        StatusText = $"状态：定时器运行中 - 还有{timeStr}后执行{action}";

        // 启动倒计时任务
        _cancellationTokenSource = new CancellationTokenSource();
        await RunCountdownAsync(_cancellationTokenSource.Token);
    }

    private void StopCountdown()
    {
        _cancellationTokenSource?.Cancel();
        IsRunning = false;
        
        StatusText = "状态：定时器已取消";
        CountdownText = "倒计时：--:--:--";
        
        AddLog("定时任务已取消");
    }

    private async Task RunCountdownAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (_countdownSeconds > 0 && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                _countdownSeconds--;
                UpdateCountdownDisplay();
                
                var action = SelectedAction.GetDisplayName();
                var timeStr = FormatTimeString(_countdownSeconds);
                StatusText = $"状态：定时器运行中 - 还有{timeStr}后执行{action}";
            }

            if (!cancellationToken.IsCancellationRequested && _countdownSeconds == 0)
            {
                await ExecuteShutdownAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消，不需要处理
        }
        catch (Exception ex)
        {
            AddLog($"倒计时错误：{ex.Message}");
        }
    }

    private void UpdateCountdownDisplay()
    {
        int hours = _countdownSeconds / 3600;
        int minutes = (_countdownSeconds % 3600) / 60;
        int seconds = _countdownSeconds % 60;
        CountdownText = $"倒计时：{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    private string FormatTimeString(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            return $"{hours}小时{minutes}分{seconds}秒";
        else if (minutes > 0)
            return $"{minutes}分{seconds}秒";
        else
            return $"{seconds}秒";
    }

    private async Task ExecuteShutdownAsync()
    {
        IsRunning = false;

        var action = SelectedAction.GetDisplayName();
        var command = SelectedAction.GetCommand();

        try
        {
            AddLog($"倒计时结束，正在执行：{action}");

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(processInfo);
            AddLog($"命令已执行：{command}");
        }
        catch (Exception ex)
        {
            AddLog($"执行失败：{ex.Message}");
            await ShowMessageBox($"执行失败：{ex.Message}", "错误");
            ResetUI();
        }
    }

    public void QuickHibernate()
    {
        ExecuteQuickAction(ShutdownAction.Hibernate);
    }

    public void QuickShutdown()
    {
        ExecuteQuickAction(ShutdownAction.Shutdown);
    }

    public void QuickRestart()
    {
        ExecuteQuickAction(ShutdownAction.Restart);
    }

    private void ExecuteQuickAction(ShutdownAction action)
    {
        var actionName = action.GetDisplayName();
        var command = action.GetCommand();

        try
        {
            AddLog($"快速执行：{actionName}");

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(processInfo);
            AddLog($"命令已执行：{command}");
        }
        catch (Exception ex)
        {
            AddLog($"执行失败：{ex.Message}");
        }
    }

    private void ResetUI()
    {
        StatusText = "状态：等待设置";
        CountdownText = "倒计时：--:--:--";
    }

    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var logMessage = $"[{timestamp}] {message}";
        
        Dispatcher.UIThread.Post(() => 
        {
            Logs.Add(logMessage);
            // 保持最多 100 条日志
            if (Logs.Count > 100)
            {
                Logs.RemoveAt(0);
            }
        });
    }

    private async Task ShowMessageBox(string message, string title)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok);
        await box.ShowAsync();
    }

    public async Task<bool> OnClosingAsync()
    {
        if (IsRunning)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "确认", 
                "定时器正在运行，确定要退出吗？", 
                ButtonEnum.YesNo);
            
            var result = await box.ShowAsync();
            return result == ButtonResult.Yes;
        }
        
        return true;
    }
}

