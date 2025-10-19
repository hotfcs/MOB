using MauiApp.Models;

namespace MauiApp.Services;

/// <summary>
/// 화면 보호 서비스 인터페이스
/// </summary>
public interface IProtectionService
{
    Task ActivateProtectionAsync(ProtectionAction action);
    Task DeactivateProtectionAsync();
    Task VibrateAsync();
    Task PlayAlertSoundAsync();
    bool IsProtectionActive { get; }
}

