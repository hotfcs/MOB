using MauiApp.Models;

namespace MauiApp.Services;

/// <summary>
/// 화면 보호 서비스 구현
/// </summary>
public class ProtectionService : IProtectionService
{
    public bool IsProtectionActive { get; private set; }
    
    public event EventHandler<ProtectionAction>? ProtectionActivated;
    public event EventHandler? ProtectionDeactivated;

    public Task ActivateProtectionAsync(ProtectionAction action)
    {
        IsProtectionActive = true;
        ProtectionActivated?.Invoke(this, action);
        
        System.Diagnostics.Debug.WriteLine($"Protection activated: {action}");
        return Task.CompletedTask;
    }

    public Task DeactivateProtectionAsync()
    {
        IsProtectionActive = false;
        ProtectionDeactivated?.Invoke(this, EventArgs.Empty);
        
        System.Diagnostics.Debug.WriteLine("Protection deactivated");
        return Task.CompletedTask;
    }

    public async Task VibrateAsync()
    {
        try
        {
            var duration = TimeSpan.FromMilliseconds(200);
            Vibration.Default.Vibrate(duration);
            await Task.Delay(duration);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Vibration error: {ex.Message}");
        }
    }

    public Task PlayAlertSoundAsync()
    {
        // 실제로는 사운드 재생
        System.Diagnostics.Debug.WriteLine("Alert sound played");
        return Task.CompletedTask;
    }
}

