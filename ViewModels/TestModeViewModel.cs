using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

/// <summary>
/// 테스트 모드 ViewModel
/// 개발 및 디버깅을 위한 테스트 기능 제공
/// </summary>
public partial class TestModeViewModel : BaseViewModel
{
    private readonly IFaceDetectionService _faceDetectionService;
    private readonly IProtectionService _protectionService;
    private readonly ICameraService _cameraService;

    [ObservableProperty]
    private string testLog = "테스트 로그:";

    [ObservableProperty]
    private bool isServiceInitialized;

    [ObservableProperty]
    private int detectionCount;

    [ObservableProperty]
    private ProtectionAction selectedTestAction = ProtectionAction.Blur;

    [ObservableProperty]
    private DisguiseType selectedTestDisguise = DisguiseType.News;

    public TestModeViewModel(
        IFaceDetectionService faceDetectionService,
        IProtectionService protectionService,
        ICameraService cameraService)
    {
        _faceDetectionService = faceDetectionService;
        _protectionService = protectionService;
        _cameraService = cameraService;

        Title = "테스트 모드";
        CheckServiceStatus();
    }

    #region Commands

    /// <summary>
    /// 엿보기 감지 시뮬레이션
    /// </summary>
    [RelayCommand]
    private async Task SimulatePeekingAsync()
    {
        try
        {
            DetectionCount++;
            AddLog($"[{DateTime.Now:HH:mm:ss}] 엿보기 감지 시뮬레이션 #{DetectionCount}");
            
            // 진동
            await _protectionService.VibrateAsync();
            AddLog("- 진동 알림 실행");

            // 보호 활성화
            await _protectionService.ActivateProtectionAsync(SelectedTestAction, SelectedTestDisguise);
            AddLog($"- 보호 활성화: {SelectedTestAction}, 위장: {SelectedTestDisguise}");

            await Shell.Current.DisplayAlert(
                "테스트 완료",
                $"엿보기 감지 시뮬레이션이 실행되었습니다.\n" +
                $"동작: {GetActionName(SelectedTestAction)}\n" +
                $"위장: {GetDisguiseName(SelectedTestDisguise)}",
                "확인");
        }
        catch (Exception ex)
        {
            AddLog($"❌ 오류: {ex.Message}");
            await Shell.Current.DisplayAlert("오류", ex.Message, "확인");
        }
    }

    /// <summary>
    /// 특정 위장 화면 미리보기
    /// </summary>
    [RelayCommand]
    private async Task PreviewDisguiseAsync(string disguiseType)
    {
        if (Enum.TryParse<DisguiseType>(disguiseType, out var type))
        {
            AddLog($"[{DateTime.Now:HH:mm:ss}] {GetDisguiseName(type)} 미리보기");
            
            await _protectionService.ActivateProtectionAsync(ProtectionAction.Disguise, type);
            
            await Shell.Current.DisplayAlert(
                "미리보기",
                $"{GetDisguiseName(type)} 화면이 표시되었습니다.",
                "확인");
        }
    }

    /// <summary>
    /// 보호 해제
    /// </summary>
    [RelayCommand]
    private async Task DeactivateProtectionAsync()
    {
        await _protectionService.DeactivateProtectionAsync();
        AddLog($"[{DateTime.Now:HH:mm:ss}] 보호 해제");
    }

    /// <summary>
    /// 얼굴 감지 테스트
    /// </summary>
    [RelayCommand]
    private async Task TestFaceDetectionAsync()
    {
        try
        {
            IsBusy = true;
            AddLog($"[{DateTime.Now:HH:mm:ss}] 얼굴 감지 서비스 테스트 시작...");

            if (!_faceDetectionService.IsInitialized)
            {
                AddLog("- 서비스 초기화 중...");
                var success = await _faceDetectionService.InitializeAsync();
                if (success)
                {
                    AddLog("✅ 서비스 초기화 성공");
                    IsServiceInitialized = true;
                }
                else
                {
                    AddLog("❌ 서비스 초기화 실패");
                    return;
                }
            }

            // 더미 이미지 데이터로 테스트
            var dummyData = new byte[320 * 240 * 3];
            AddLog("- 테스트 이미지 생성 (320x240)");

            var result = await _faceDetectionService.DetectFacesAsync(dummyData);
            AddLog($"✅ 감지 완료: {result.FaceCount}개 얼굴");
            AddLog($"- 엿보기 감지: {(result.HasPeekingDetected ? "예" : "아니오")}");

            await Shell.Current.DisplayAlert(
                "테스트 결과",
                $"얼굴 감지 테스트 완료\n" +
                $"감지된 얼굴: {result.FaceCount}개\n" +
                $"엿보기 감지: {(result.HasPeekingDetected ? "예" : "아니오")}",
                "확인");
        }
        catch (Exception ex)
        {
            AddLog($"❌ 오류: {ex.Message}");
            await Shell.Current.DisplayAlert("오류", ex.Message, "확인");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// 카메라 권한 테스트
    /// </summary>
    [RelayCommand]
    private async Task TestCameraPermissionAsync()
    {
        try
        {
            AddLog($"[{DateTime.Now:HH:mm:ss}] 카메라 권한 확인...");
            
            var hasPermission = await _cameraService.RequestPermissionsAsync();
            
            if (hasPermission)
            {
                AddLog("✅ 카메라 권한 허용됨");
                await Shell.Current.DisplayAlert("성공", "카메라 권한이 허용되었습니다.", "확인");
            }
            else
            {
                AddLog("❌ 카메라 권한 거부됨");
                await Shell.Current.DisplayAlert("실패", "카메라 권한이 거부되었습니다.", "확인");
            }
        }
        catch (Exception ex)
        {
            AddLog($"❌ 오류: {ex.Message}");
            await Shell.Current.DisplayAlert("오류", ex.Message, "확인");
        }
    }

    /// <summary>
    /// 모든 보호 동작 테스트
    /// </summary>
    [RelayCommand]
    private async Task TestAllActionsAsync()
    {
        try
        {
            AddLog($"[{DateTime.Now:HH:mm:ss}] 모든 보호 동작 테스트 시작");

            var actions = new[]
            {
                (ProtectionAction.Blur, DisguiseType.News),
                (ProtectionAction.Cover, DisguiseType.News),
                (ProtectionAction.Disguise, DisguiseType.News),
                (ProtectionAction.Disguise, DisguiseType.StockMarket),
                (ProtectionAction.Disguise, DisguiseType.EBook),
                (ProtectionAction.Disguise, DisguiseType.Calculator),
                (ProtectionAction.Disguise, DisguiseType.Calendar),
                (ProtectionAction.VibrateOnly, DisguiseType.News)
            };

            foreach (var (action, disguise) in actions)
            {
                AddLog($"- 테스트: {GetActionName(action)} / {GetDisguiseName(disguise)}");
                await _protectionService.ActivateProtectionAsync(action, disguise);
                await Task.Delay(2000); // 2초 대기
                await _protectionService.DeactivateProtectionAsync();
                await Task.Delay(500); // 0.5초 대기
            }

            AddLog("✅ 모든 테스트 완료");
            await Shell.Current.DisplayAlert("완료", "모든 보호 동작 테스트가 완료되었습니다.", "확인");
        }
        catch (Exception ex)
        {
            AddLog($"❌ 오류: {ex.Message}");
            await Shell.Current.DisplayAlert("오류", ex.Message, "확인");
        }
    }

    /// <summary>
    /// 로그 초기화
    /// </summary>
    [RelayCommand]
    private void ClearLog()
    {
        TestLog = "테스트 로그:";
        DetectionCount = 0;
        AddLog($"[{DateTime.Now:HH:mm:ss}] 로그 초기화");
    }

    /// <summary>
    /// 뒤로 가기
    /// </summary>
    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    #endregion

    #region Private Methods

    private void CheckServiceStatus()
    {
        IsServiceInitialized = _faceDetectionService.IsInitialized;
        AddLog($"[{DateTime.Now:HH:mm:ss}] 테스트 모드 시작");
        AddLog($"- 얼굴 감지 서비스: {(IsServiceInitialized ? "초기화됨" : "미초기화")}");
        AddLog($"- 카메라 사용 가능: {(_cameraService.IsCameraAvailable ? "예" : "아니오")}");
    }

    private void AddLog(string message)
    {
        TestLog += $"\n{message}";
    }

    private string GetActionName(ProtectionAction action) => action switch
    {
        ProtectionAction.Blur => "블러 효과",
        ProtectionAction.Cover => "화면 가리기",
        ProtectionAction.Disguise => "위장 화면",
        ProtectionAction.VibrateOnly => "진동만",
        _ => "알 수 없음"
    };

    private string GetDisguiseName(DisguiseType type) => type switch
    {
        DisguiseType.News => "뉴스",
        DisguiseType.StockMarket => "주식 시장",
        DisguiseType.EBook => "전자책",
        DisguiseType.Calculator => "계산기",
        DisguiseType.Calendar => "캘린더",
        _ => "알 수 없음"
    };

    #endregion
}

