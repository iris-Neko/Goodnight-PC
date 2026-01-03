namespace GoodNightPC.Models;

public enum ShutdownAction
{
    Hibernate,
    ForceHibernate,
    Shutdown,
    Restart,
    ForceShutdown
}

public static class ShutdownActionExtensions
{
    public static string GetDisplayName(this ShutdownAction action)
    {
        return action switch
        {
            ShutdownAction.Hibernate => "休眠",
            ShutdownAction.ForceHibernate => "强制休眠",
            ShutdownAction.Shutdown => "关机",
            ShutdownAction.Restart => "重启",
            ShutdownAction.ForceShutdown => "强制关机",
            _ => "未知"
        };
    }
    
    public static string GetCommand(this ShutdownAction action)
    {
        return action switch
        {
            ShutdownAction.Hibernate => "shutdown /h",
            ShutdownAction.ForceHibernate => "shutdown /f /h",
            ShutdownAction.Shutdown => "shutdown /s /t 0",
            ShutdownAction.Restart => "shutdown /r /t 0",
            ShutdownAction.ForceShutdown => "shutdown /s /f /t 0",
            _ => "shutdown /h"
        };
    }
}

