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
        var loc = LocalizationService.Instance;
        return action switch
        {
            ShutdownAction.Hibernate => loc.GetString("Opt_Hibernate"),
            ShutdownAction.ForceHibernate => loc.GetString("Opt_ForceHibernate"),
            ShutdownAction.Shutdown => loc.GetString("Opt_Shutdown"),
            ShutdownAction.Restart => loc.GetString("Opt_Restart"),
            ShutdownAction.ForceShutdown => loc.GetString("Opt_ForceShutdown"),
            _ => "Unknown"
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

