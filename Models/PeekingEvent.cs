namespace MauiApp.Models;

/// <summary>
/// 엿보기 감지 이벤트
/// </summary>
public class PeekingEvent
{
    public DateTime Timestamp { get; set; }
    public int FaceCount { get; set; }
    public double AngleFromCenter { get; set; }
    public double DurationSeconds { get; set; }
    public string? PhotoPath { get; set; }
    public string Location { get; set; } = "Unknown";
}

