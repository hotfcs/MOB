namespace MauiApp.Models;

/// <summary>
/// 감지 모드 열거형
/// </summary>
public enum DetectionMode
{
    /// <summary>
    /// 출퇴근 모드 - 최고 민감도 (지하철, 버스)
    /// </summary>
    Commute,
    
    /// <summary>
    /// 사무실 모드 - 측면/후면 감지만
    /// </summary>
    Office,
    
    /// <summary>
    /// 회의 모드 - 진동만, 화면 전환 없음
    /// </summary>
    Meeting,
    
    /// <summary>
    /// 사용자 정의 모드
    /// </summary>
    Custom
}

/// <summary>
/// 보호 동작 유형
/// </summary>
public enum ProtectionAction
{
    /// <summary>
    /// 화면 블러 처리
    /// </summary>
    Blur,
    
    /// <summary>
    /// 위장 화면 표시 (뉴스, 전자책 등)
    /// </summary>
    Disguise,
    
    /// <summary>
    /// 화면 가리기 (검은색 오버레이)
    /// </summary>
    Cover,
    
    /// <summary>
    /// 진동만
    /// </summary>
    VibrateOnly
}

/// <summary>
/// 위장 화면 유형
/// </summary>
public enum DisguiseType
{
    News,
    StockMarket,
    EBook,
    Calculator,
    Calendar
}

