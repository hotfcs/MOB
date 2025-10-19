using MauiApp.Models;

namespace MauiApp.Services;

/// <summary>
/// 얼굴 감지 서비스 인터페이스
/// </summary>
public interface IFaceDetectionService
{
    Task<bool> InitializeAsync();
    Task<FaceDetectionResult> DetectFacesAsync(byte[] imageData);
    bool IsInitialized { get; }
}

public class FaceDetectionResult
{
    public int FaceCount { get; set; }
    public List<FaceInfo> Faces { get; set; } = new();
    public bool HasPeekingDetected { get; set; }
}

public class FaceInfo
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double AngleFromCenter { get; set; }
    public bool IsOwner { get; set; }
}

