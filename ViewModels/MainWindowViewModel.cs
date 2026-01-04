using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
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
    private readonly LocalizationService _loc = LocalizationService.Instance;
    private int _hours;
    private int _minutes;
    private int _seconds = 5;
    private ShutdownAction _selectedAction = ShutdownAction.Hibernate;
    private bool _isRunning;
    private int _countdownSeconds;
    private string _statusText = string.Empty;
    private string _countdownText = string.Empty;
    private CancellationTokenSource? _cancellationTokenSource;
    private string _windowTitle = string.Empty;
    private string _appTitle = string.Empty;
    private string _headerSetTime = string.Empty;
    private string _headerShutdownOptions = string.Empty;
    private string _headerLogs = string.Empty;
    private string _labelHours = string.Empty;
    private string _labelMinutes = string.Empty;
    private string _labelSeconds = string.Empty;
    private string _optionHibernate = string.Empty;
    private string _optionForceHibernate = string.Empty;
    private string _optionShutdown = string.Empty;
    private string _optionRestart = string.Empty;
    private string _optionForceShutdown = string.Empty;
    private string _startButtonText = string.Empty;
    private string _stopButtonText = string.Empty;
    private string _countdownLabel = string.Empty;
    private string _languageLabel = string.Empty;
    private ObservableCollection<LanguageOption> _languages = new();
    private LanguageOption? _selectedLanguage;

    public MainWindowViewModel()
    {
        Logs = new ObservableCollection<string>();
        
        StartCommand = ReactiveCommand.CreateFromTask(StartCountdownAsync, 
            this.WhenAnyValue(x => x.IsRunning, running => !running));
        
        StopCommand = ReactiveCommand.Create(StopCountdown, 
            this.WhenAnyValue(x => x.IsRunning));
        
        _loc.LanguageChanged += UpdateLocalizedTexts;
        UpdateLocalizedTexts();
        AddLog(_loc.GetString("Log_Startup"));
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
    
    public string WindowTitle
    {
        get => _windowTitle;
        private set => this.RaiseAndSetIfChanged(ref _windowTitle, value);
    }

    public string AppTitle
    {
        get => _appTitle;
        private set => this.RaiseAndSetIfChanged(ref _appTitle, value);
    }

    public string HeaderSetTime
    {
        get => _headerSetTime;
        private set => this.RaiseAndSetIfChanged(ref _headerSetTime, value);
    }

    public string HeaderShutdownOptions
    {
        get => _headerShutdownOptions;
        private set => this.RaiseAndSetIfChanged(ref _headerShutdownOptions, value);
    }

    public string HeaderLogs
    {
        get => _headerLogs;
        private set => this.RaiseAndSetIfChanged(ref _headerLogs, value);
    }

    public string LabelHours
    {
        get => _labelHours;
        private set => this.RaiseAndSetIfChanged(ref _labelHours, value);
    }

    public string LabelMinutes
    {
        get => _labelMinutes;
        private set => this.RaiseAndSetIfChanged(ref _labelMinutes, value);
    }

    public string LabelSeconds
    {
        get => _labelSeconds;
        private set => this.RaiseAndSetIfChanged(ref _labelSeconds, value);
    }

    public string OptionHibernate
    {
        get => _optionHibernate;
        private set => this.RaiseAndSetIfChanged(ref _optionHibernate, value);
    }

    public string OptionForceHibernate
    {
        get => _optionForceHibernate;
        private set => this.RaiseAndSetIfChanged(ref _optionForceHibernate, value);
    }

    public string OptionShutdown
    {
        get => _optionShutdown;
        private set => this.RaiseAndSetIfChanged(ref _optionShutdown, value);
    }

    public string OptionRestart
    {
        get => _optionRestart;
        private set => this.RaiseAndSetIfChanged(ref _optionRestart, value);
    }

    public string OptionForceShutdown
    {
        get => _optionForceShutdown;
        private set => this.RaiseAndSetIfChanged(ref _optionForceShutdown, value);
    }

    public string StartButtonText
    {
        get => _startButtonText;
        private set => this.RaiseAndSetIfChanged(ref _startButtonText, value);
    }

    public string StopButtonText
    {
        get => _stopButtonText;
        private set => this.RaiseAndSetIfChanged(ref _stopButtonText, value);
    }

    public string CountdownLabel
    {
        get => _countdownLabel;
        private set => this.RaiseAndSetIfChanged(ref _countdownLabel, value);
    }

    public string LanguageLabel
    {
        get => _languageLabel;
        private set => this.RaiseAndSetIfChanged(ref _languageLabel, value);
    }

    public ObservableCollection<LanguageOption> Languages
    {
        get => _languages;
        private set => this.RaiseAndSetIfChanged(ref _languages, value);
    }

    public LanguageOption? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (value == null || _selectedLanguage == value) return;
            this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
            _loc.Language = value.Language;
        }
    }

    private async Task StartCountdownAsync()
    {
        // 验证输入
        if (Hours < 0 || Hours > 23)
        {
            await ShowMessageBox(_loc.GetString("Msg_InvalidHour"), _loc.GetString("Msg_Title_InputError"));
            return;
        }

        if (Minutes < 0 || Minutes > 59)
        {
            await ShowMessageBox(_loc.GetString("Msg_InvalidMinute"), _loc.GetString("Msg_Title_InputError"));
            return;
        }

        if (Seconds < 0 || Seconds > 59)
        {
            await ShowMessageBox(_loc.GetString("Msg_InvalidSecond"), _loc.GetString("Msg_Title_InputError"));
            return;
        }

        if (Hours == 0 && Minutes == 0 && Seconds == 0)
        {
            await ShowMessageBox(_loc.GetString("Msg_TimeRequired"), _loc.GetString("Msg_Title_Warning"));
            return;
        }

        _countdownSeconds = Hours * 3600 + Minutes * 60 + Seconds;
        IsRunning = true;
        
        UpdateCountdownDisplay();
        
        var action = SelectedAction.GetDisplayName();
        AddLog(string.Format(_loc.GetString("Log_StartTask"), Hours, Minutes, Seconds, action));
        
        var timeStr = FormatTimeString(_countdownSeconds);
        StatusText = string.Format(_loc.GetString("Status_Running_Format"), timeStr, action);

        // 启动倒计时任务
        _cancellationTokenSource = new CancellationTokenSource();
        await RunCountdownAsync(_cancellationTokenSource.Token);
    }

    private void StopCountdown()
    {
        _cancellationTokenSource?.Cancel();
        IsRunning = false;
        
        StatusText = _loc.GetString("Status_Canceled");
        CountdownText = $"{_loc.GetString("Countdown_Label")}: --:--:--";
        
        AddLog(_loc.GetString("Log_Cancel"));
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
                StatusText = string.Format(_loc.GetString("Status_Running_Format"), timeStr, action);
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
        CountdownText = $"{_loc.GetString("Countdown_Prefix")}{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    private string FormatTimeString(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            return $"{hours}{_loc.GetString("Label_Hours")}{minutes}{_loc.GetString("Label_Minutes")}{seconds}{_loc.GetString("Label_Seconds")}";
        else if (minutes > 0)
            return $"{minutes}{_loc.GetString("Label_Minutes")}{seconds}{_loc.GetString("Label_Seconds")}";
        else
            return $"{seconds}{_loc.GetString("Label_Seconds")}";
    }

    private async Task ExecuteShutdownAsync()
    {
        IsRunning = false;

        var action = SelectedAction.GetDisplayName();
        var command = SelectedAction.GetCommand();

        try
        {
            AddLog(string.Format(_loc.GetString("Log_CountdownDone"), action));

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(processInfo);
            AddLog(string.Format(_loc.GetString("Log_CommandExecuted"), command));
        }
        catch (Exception ex)
        {
            AddLog(string.Format(_loc.GetString("Log_ExecuteFailed"), ex.Message));
            await ShowMessageBox(string.Format(_loc.GetString("Log_ExecuteFailed"), ex.Message), _loc.GetString("Msg_Title_Error"));
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
            AddLog(string.Format(_loc.GetString("Log_QuickAction"), actionName));

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(processInfo);
            AddLog(string.Format(_loc.GetString("Log_CommandExecuted"), command));
        }
        catch (Exception ex)
        {
            AddLog(string.Format(_loc.GetString("Log_ExecuteFailed"), ex.Message));
        }
    }

    private void ResetUI()
    {
        StatusText = _loc.GetString("Status_Waiting");
        CountdownText = $"{_loc.GetString("Countdown_Label")}: --:--:--";
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
                _loc.GetString("Msg_Title_Confirm"), 
                _loc.GetString("Msg_Confirm_ExitWhileRunning"), 
                ButtonEnum.YesNo);
            
            var result = await box.ShowAsync();
            return result == ButtonResult.Yes;
        }
        
        return true;
    }

    private void UpdateLocalizedTexts()
    {
        WindowTitle = _loc.GetString("AppName");
        AppTitle = _loc.GetString("Header_Title");
        HeaderSetTime = _loc.GetString("Header_SetTime");
        HeaderShutdownOptions = _loc.GetString("Header_ShutdownOptions");
        HeaderLogs = _loc.GetString("Header_Logs");
        LabelHours = _loc.GetString("Label_Hours");
        LabelMinutes = _loc.GetString("Label_Minutes");
        LabelSeconds = _loc.GetString("Label_Seconds");
        OptionHibernate = _loc.GetString("Opt_Hibernate");
        OptionForceHibernate = _loc.GetString("Opt_ForceHibernate");
        OptionShutdown = _loc.GetString("Opt_Shutdown");
        OptionRestart = _loc.GetString("Opt_Restart");
        OptionForceShutdown = _loc.GetString("Opt_ForceShutdown");
        StartButtonText = _loc.GetString("Btn_Start");
        StopButtonText = _loc.GetString("Btn_Stop");
        CountdownLabel = _loc.GetString("Countdown_Label");
        LanguageLabel = _loc.GetString("Lang_Label");

        if (!IsRunning)
        {
            StatusText = _loc.GetString("Status_Waiting");
            CountdownText = $"{_loc.GetString("Countdown_Label")}: --:--:--";
        }

        Languages = new ObservableCollection<LanguageOption>(_loc.GetLanguagesForDisplay());
        SelectedLanguage = Languages.FirstOrDefault(l => l.Language == _loc.Language) 
                           ?? Languages.FirstOrDefault();

        this.RaisePropertyChanged(nameof(Languages));
    }
}

