using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

/// <summary>
/// 메인 페이지의 ViewModel
/// MVVM 패턴: Model-View-ViewModel
/// - Model: AppSettings, PeekingEvent 등
/// - View: MainPage.xaml
/// - ViewModel: MainViewModel (현재 클래스)
/// 
/// CommunityToolkit.Mvvm 사용:
/// - [ObservableProperty]: private 필드에서 자동으로 public 프로퍼티 생성
/// - [RelayCommand]: 메서드에서 자동으로 ICommand 생성
/// - partial class: Source Generator가 코드를 생성할 수 있도록 함
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    #region Fields

    private readonly ICameraService _cameraService;
    private readonly IFaceDetectionService _faceDetectionService;
    private readonly IProtectionService _protectionService;
    private readonly AppSettings _settings;
    private DateTime? _lastDetectionTime;
    private int _consecutiveDetections;

    #endregion

    #region Observable Properties

    /// <summary>
    /// 감지 활성화 상태
    /// [ObservableProperty]로 IsDetectionActive 프로퍼티 자동 생성
    /// </summary>
    [ObservableProperty]
    private bool isDetectionActive;

    /// <summary>
    /// 상태 메시지
    /// Source Generator가 StatusMessage 프로퍼티와 OnStatusMessageChanged 메서드를 자동 생성
    /// </summary>
    [ObservableProperty]
    private string statusMessage = "감지 중지됨";

    /// <summary>
    /// 엿보기 감지 횟수
    /// </summary>
    [ObservableProperty]
    private int peekingCount;

    /// <summary>
    /// 선택된 감지 모드
    /// partial 메서드 OnSelectedModeChanged가 자동 생성되어 오버라이드 가능
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModeDescription))]
    private DetectionMode selectedMode = DetectionMode.Office;

    /// <summary>
    /// 보호 오버레이 표시 여부
    /// </summary>
    [ObservableProperty]
    private bool isProtectionOverlayVisible;

    /// <summary>
    /// 현재 보호 동작
    /// </summary>
    [ObservableProperty]
    private ProtectionAction currentProtectionAction;

    #endregion

    #region Computed Properties

    /// <summary>
    /// 모드 설명 (Computed Property)
    /// SelectedMode가 변경되면 [NotifyPropertyChangedFor]로 인해 자동으로 UI 업데이트
    /// </summary>
    public string ModeDescription => SelectedMode switch
    {
        DetectionMode.Commute => "🚇 출퇴근 모드 (최고 민감도)",
        DetectionMode.Office => "💼 사무실 모드 (측면/후면)",
        DetectionMode.Meeting => "👥 회의 모드 (진동만)",
        DetectionMode.Custom => "⚙️ 사용자 정의 모드",
        _ => "알 수 없음"
    };

    #endregion

    #region Events

    /// <summary>
    /// 보호 활성화 이벤트
    /// </summary>
    public event Action<ProtectionAction, DisguiseType>? ProtectionActivatedEvent;

    /// <summary>
    /// 보호 비활성화 이벤트
    /// </summary>
    public event Action? ProtectionDeactivatedEvent;

    #endregion

    #region Constructor

    public MainViewModel(
        ICameraService cameraService,
        IFaceDetectionService faceDetectionService,
        IProtectionService protectionService)
    {
        _cameraService = cameraService;
        _faceDetectionService = faceDetectionService;
        _protectionService = protectionService;
        _settings = new AppSettings();

        Title = "화면 엿보기 방지";

        // 서비스 이벤트 구독
        _cameraService.FrameCaptured += OnFrameCaptured;

        // 초기화
        _ = InitializeAsync();
    }

    #endregion

    #region Commands

    /// <summary>
    /// 감지 토글 커맨드
    /// [RelayCommand]가 ToggleDetectionCommand 프로퍼티를 자동 생성
    /// 비동기 메서드의 경우 자동으로 CancellationToken 지원
    /// </summary>
    [RelayCommand]
    private async Task ToggleDetectionAsync()
    {
        if (IsDetectionActive)
        {
            await StopDetectionAsync();
        }
        else
        {
            await StartDetectionAsync();
        }
    }

    /// <summary>
    /// 모드 변경 커맨드
    /// [RelayCommand]로 ChangeModeCommand 자동 생성
    /// 파라미터가 있는 경우도 자동 처리
    /// </summary>
    [RelayCommand]
    private void ChangeMode(DetectionMode mode)
    {
        SelectedMode = mode;
        _settings.CurrentMode = mode;
        UpdateStatusMessage();
    }

    /// <summary>
    /// 설정 열기 커맨드
    /// [RelayCommand]로 OpenSettingsCommand 자동 생성
    /// </summary>
    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.SettingsPage));
    }

    /// <summary>
    /// 보호 해제 커맨드
    /// [RelayCommand]로 DismissProtectionCommand 자동 생성
    /// </summary>
    [RelayCommand]
    private Task DismissProtectionAsync()
    {
        _ = _protectionService.DeactivateProtectionAsync();
        IsProtectionOverlayVisible = false;
        
        // 이벤트 발생
        ProtectionDeactivatedEvent?.Invoke();
        
        return Task.CompletedTask;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 초기화 (비동기)
    /// </summary>
    private async Task InitializeAsync()
    {
        StatusMessage = "초기화 중...";
        await _faceDetectionService.InitializeAsync();
        StatusMessage = "감지 준비 완료";
    }

    /// <summary>
    /// 감지 시작
    /// </summary>
    private async Task StartDetectionAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "권한 확인 중...";
            
            var hasPermission = await _cameraService.RequestPermissionsAsync();
            
            if (!hasPermission)
            {
                StatusMessage = "카메라 권한이 필요합니다";
                await Shell.Current.DisplayAlert(
                    "권한 필요",
                    "엿보기 감지를 위해 카메라 권한이 필요합니다.",
                    "확인");
                return;
            }

            await _cameraService.StartCameraAsync();
            IsDetectionActive = true;
            _settings.IsDetectionEnabled = true;
            UpdateStatusMessage();
        }
        catch (Exception ex)
        {
            StatusMessage = $"오류: {ex.Message}";
            await Shell.Current.DisplayAlert("오류", ex.Message, "확인");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// 감지 중지
    /// </summary>
    private async Task StopDetectionAsync()
    {
        await _cameraService.StopCameraAsync();
        IsDetectionActive = false;
        _settings.IsDetectionEnabled = false;
        StatusMessage = "감지 중지됨";
        _consecutiveDetections = 0;
    }

    /// <summary>
    /// 프레임 캡처 이벤트 핸들러
    /// </summary>
    private async void OnFrameCaptured(object? sender, byte[] frameData)
    {
        if (!IsDetectionActive || !_faceDetectionService.IsInitialized)
            return;

        try
        {
            var result = await _faceDetectionService.DetectFacesAsync(frameData);

            if (result.HasPeekingDetected)
            {
                _consecutiveDetections++;
                
                // 연속 감지 횟수가 임계값을 넘으면 보호 활성화
                var threshold = (int)(_settings.PeekingThresholdSeconds * _settings.DetectionFrequency);
                
                if (_consecutiveDetections >= threshold)
                {
                    await HandlePeekingDetectedAsync(result);
                    _consecutiveDetections = 0;
                }
            }
            else
            {
                _consecutiveDetections = 0;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Detection error: {ex.Message}");
        }
    }

    /// <summary>
    /// 엿보기 감지 처리
    /// </summary>
    private async Task HandlePeekingDetectedAsync(FaceDetectionResult result)
    {
        PeekingCount++;
        _lastDetectionTime = DateTime.Now;

        // MainThread에서 UI 업데이트 (MVVM 패턴의 중요한 부분)
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            StatusMessage = $"⚠️ 엿보기 감지됨! ({result.FaceCount}명)";

            // 진동
            if (_settings.VibrateOnDetection)
            {
                await _protectionService.VibrateAsync();
            }

            // 소리
            if (_settings.SoundOnDetection)
            {
                await _protectionService.PlayAlertSoundAsync();
            }

            // 보호 화면 활성화 (회의 모드가 아닌 경우)
            if (SelectedMode != DetectionMode.Meeting)
            {
                CurrentProtectionAction = _settings.ProtectionAction;
                IsProtectionOverlayVisible = true;
                await _protectionService.ActivateProtectionAsync(_settings.ProtectionAction, _settings.DisguiseType);
                
                // 이벤트 발생
                ProtectionActivatedEvent?.Invoke(_settings.ProtectionAction, _settings.DisguiseType);
            }
        });
    }

    /// <summary>
    /// 상태 메시지 업데이트
    /// </summary>
    private void UpdateStatusMessage()
    {
        if (IsDetectionActive)
        {
            StatusMessage = $"🟢 감지 활성화 - {ModeDescription}";
        }
        else
        {
            StatusMessage = "감지 중지됨";
        }
    }

    #endregion

    #region Partial Methods (Source Generator Callbacks)

    /// <summary>
    /// SelectedMode 변경 시 자동 호출되는 콜백
    /// Source Generator가 자동으로 호출 코드를 생성
    /// </summary>
    partial void OnSelectedModeChanged(DetectionMode value)
    {
        _settings.CurrentMode = value;
        UpdateStatusMessage();
    }

    #endregion
}
