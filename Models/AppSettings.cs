namespace MauiApp.Models;

/// <summary>
/// 앱 설정
/// </summary>
public class AppSettings
{
    /// <summary>
    /// 감지 활성화 여부
    /// </summary>
    public bool IsDetectionEnabled { get; set; } = false;
    
    /// <summary>
    /// 현재 모드
    /// </summary>
    public DetectionMode CurrentMode { get; set; } = DetectionMode.Office;
    
    /// <summary>
    /// 보호 동작
    /// </summary>
    public ProtectionAction ProtectionAction { get; set; } = ProtectionAction.Blur;
    
    /// <summary>
    /// 위장 화면 유형
    /// </summary>
    public DisguiseType DisguiseType { get; set; } = DisguiseType.News;
    
    /// <summary>
    /// 지속 주시 시간 임계값 (초)
    /// </summary>
    public double PeekingThresholdSeconds { get; set; } = 1.5;
    
    /// <summary>
    /// 감지 빈도 (초당 체크 횟수)
    /// </summary>
    public int DetectionFrequency { get; set; } = 3;
    
    /// <summary>
    /// 진동 활성화
    /// </summary>
    public bool VibrateOnDetection { get; set; } = true;
    
    /// <summary>
    /// 소리 활성화
    /// </summary>
    public bool SoundOnDetection { get; set; } = false;
    
    /// <summary>
    /// 사진 캡처 활성화
    /// </summary>
    public bool CapturePhoto { get; set; } = false;
    
    /// <summary>
    /// 민감도 (1-10)
    /// </summary>
    public int Sensitivity
    {
        get
        {
            return CurrentMode switch
            {
                DetectionMode.Commute => 10,
                DetectionMode.Office => 6,
                DetectionMode.Meeting => 4,
                DetectionMode.Custom => _customSensitivity,
                _ => 5
            };
        }
        set => _customSensitivity = value;
    }
    
    private int _customSensitivity = 5;
}

