using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GoodNightPC.Models;

public enum AppLanguage
{
    Auto,
    Zh,
    En
}

public class LocalizationService
{
    private const string SettingsFileName = "settings.json";
    private const string SettingsFolderName = "GoodNightPC";
    private readonly Dictionary<string, (string zh, string en)> _resources;
    private AppLanguage _language;

    public event Action? LanguageChanged;

    private LocalizationService()
    {
        _resources = BuildResources();
        _language = LoadSavedLanguage();
    }

    public static LocalizationService Instance { get; } = new();

    public AppLanguage Language
    {
        get => _language;
        set
        {
            if (_language == value) return;
            _language = value;
            SaveLanguage(value);
            LanguageChanged?.Invoke();
        }
    }

    public string GetString(string key)
    {
        var lang = ResolveLanguage();
        if (_resources.TryGetValue(key, out var pair))
        {
            return lang == AppLanguage.En ? pair.en : pair.zh;
        }

        return key;
    }

    public IEnumerable<LanguageOption> GetLanguagesForDisplay()
    {
        return new[]
        {
            new LanguageOption(AppLanguage.Auto, GetString("Lang_Auto")),
            new LanguageOption(AppLanguage.Zh, GetString("Lang_Zh")),
            new LanguageOption(AppLanguage.En, GetString("Lang_En"))
        };
    }

    private AppLanguage ResolveLanguage()
    {
        if (_language == AppLanguage.Auto)
        {
            var sys = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
            return sys == "zh" ? AppLanguage.Zh : AppLanguage.En;
        }

        return _language;
    }

    private AppLanguage LoadSavedLanguage()
    {
        try
        {
            var path = GetSettingsPath();
            if (!File.Exists(path))
            {
                return AppLanguage.Auto;
            }

            var langText = File.ReadAllText(path).Trim().ToLowerInvariant();
            return langText switch
            {
                "zh" => AppLanguage.Zh,
                "en" => AppLanguage.En,
                _ => AppLanguage.Auto
            };
        }
        catch
        {
            return AppLanguage.Auto;
        }
    }

    private void SaveLanguage(AppLanguage language)
    {
        try
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                SettingsFolderName);
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, SettingsFileName);
            File.WriteAllText(path, language.ToString().ToLowerInvariant());
        }
        catch
        {
            // 保存失败不影响运行
        }
    }

    private static string GetSettingsPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            SettingsFolderName);
        return Path.Combine(folder, SettingsFileName);
    }

    private static Dictionary<string, (string zh, string en)> BuildResources()
    {
        return new Dictionary<string, (string zh, string en)>
        {
            ["AppName"] = ("GoodNight PC", "GoodNight PC"),
            ["Header_Title"] = ("GoodNight PC", "GoodNight PC"),
            ["Header_SetTime"] = ("⏱️ 设置关机时间", "⏱️ Set Shutdown Time"),
            ["Header_ShutdownOptions"] = ("🔌 关机选项", "🔌 Shutdown Options"),
            ["Header_Logs"] = ("📋 操作日志", "📋 Action Logs"),
            ["Label_Hours"] = ("小时", "Hours"),
            ["Label_Minutes"] = ("分钟", "Minutes"),
            ["Label_Seconds"] = ("秒", "Seconds"),
            ["Btn_Start"] = ("开始定时", "Start Timer"),
            ["Btn_Stop"] = ("取消定时", "Cancel Timer"),
            ["Opt_Hibernate"] = ("😴 休眠", "😴 Hibernate"),
            ["Opt_ForceHibernate"] = ("💤 强制休眠（不保存数据）", "💤 Force Hibernate (unsaved data lost)"),
            ["Opt_Shutdown"] = ("🔌 关机", "🔌 Shutdown"),
            ["Opt_Restart"] = ("🔄 重启", "🔄 Restart"),
            ["Opt_ForceShutdown"] = ("⚠️ 强制关机（不保存数据）", "⚠️ Force Shutdown (unsaved data lost)"),
            ["Countdown_Label"] = ("倒计时", "Countdown"),
            ["Status_Waiting"] = ("状态：等待设置", "Status: waiting"),
            ["Status_Canceled"] = ("状态：定时器已取消", "Status: timer cancelled"),
            ["Status_Running_Format"] = ("状态：定时器运行中 - 还有{0}后执行{1}", "Status: timer running - {0} until {1}"),
            ["Countdown_Prefix"] = ("倒计时：", "Countdown: "),
            ["Log_Startup"] = ("程序启动成功", "Application started"),
            ["Log_StartTask"] = ("定时任务已开始：{0}小时{1}分{2}秒后执行【{3}】", "Timer started: {0}h {1}m {2}s later do [{3}]"),
            ["Log_Cancel"] = ("定时任务已取消", "Timer cancelled"),
            ["Log_CountdownDone"] = ("倒计时结束，正在执行：{0}", "Countdown finished, executing: {0}"),
            ["Log_CommandExecuted"] = ("命令已执行：{0}", "Command executed: {0}"),
            ["Log_ExecuteFailed"] = ("执行失败：{0}", "Execute failed: {0}"),
            ["Log_QuickAction"] = ("快速执行：{0}", "Quick action: {0}"),
            ["Msg_InvalidHour"] = ("小时数必须在 0-23 之间！", "Hour must be between 0 and 23!"),
            ["Msg_InvalidMinute"] = ("分钟数必须在 0-59 之间！", "Minute must be between 0 and 59!"),
            ["Msg_InvalidSecond"] = ("秒数必须在 0-59 之间！", "Second must be between 0 and 59!"),
            ["Msg_TimeRequired"] = ("请设置有效的时间！", "Please set a valid time!"),
            ["Msg_Title_InputError"] = ("输入错误", "Input error"),
            ["Msg_Title_Warning"] = ("警告", "Warning"),
            ["Msg_Title_Error"] = ("错误", "Error"),
            ["Msg_Title_Confirm"] = ("确认", "Confirm"),
            ["Msg_Confirm_ExitWhileRunning"] = ("定时器正在运行，确定要退出吗？", "Timer is running. Exit anyway?"),
            ["Tray_Tooltip"] = ("GoodNight PC", "GoodNight PC"),
            ["Tray_Show"] = ("显示窗口", "Show Window"),
            ["Tray_QuickHibernate"] = ("💤 立即休眠", "💤 Hibernate Now"),
            ["Tray_QuickShutdown"] = ("🔌 立即关机", "🔌 Shutdown Now"),
            ["Tray_QuickRestart"] = ("🔄 立即重启", "🔄 Restart Now"),
            ["Tray_Exit"] = ("退出", "Exit"),
            ["Lang_Label"] = ("语言 / Language", "Language"),
            ["Lang_Auto"] = ("跟随系统", "Follow system"),
            ["Lang_Zh"] = ("简体中文", "简体中文"),
            ["Lang_En"] = ("English", "English")
        };
    }
}

public record LanguageOption(AppLanguage Language, string DisplayName);

