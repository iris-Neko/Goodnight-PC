using System;

namespace GoodNightPC.Models;

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    
    public string FormattedLog => $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message}";
}

