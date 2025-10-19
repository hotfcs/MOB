namespace MauiApp.Services;

/// <summary>
/// 카메라 서비스 인터페이스
/// </summary>
public interface ICameraService
{
    Task<bool> RequestPermissionsAsync();
    Task StartCameraAsync();
    Task StopCameraAsync();
    bool IsCameraAvailable { get; }
    event EventHandler<byte[]>? FrameCaptured;
}

