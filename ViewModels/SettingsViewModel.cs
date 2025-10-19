using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp.Models;

namespace MauiApp.ViewModels;

/// <summary>
/// 설정 페이지의 ViewModel
/// 
/// MVVM 패턴의 장점:
/// 1. UI와 비즈니스 로직 분리
/// 2. 테스트 가능한 코드
/// 3. 재사용 가능한 컴포넌트
/// 4. 유지보수 용이
/// 
/// CommunityToolkit.Mvvm의 Source Generator 사용:
/// - 컴파일 타임에 코드 자동 생성
/// - 런타임 오버헤드 없음
/// - IntelliSense 지원
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    #region Fields

    private readonly AppSettings _settings = new AppSettings();

    #endregion

    #region Observable Properties

    /// <summary>
    /// 선택된 보호 동작
    /// [ObservableProperty]로 SelectedProtectionAction 프로퍼티 자동 생성
    /// </summary>
    [ObservableProperty]
    private ProtectionAction selectedProtectionAction;

    /// <summary>
    /// 선택된 위장 화면 타입
    /// </summary>
    [ObservableProperty]
    private DisguiseType selectedDisguiseType;

    /// <summary>
    /// 지속 주시 시간 임계값 (초)
    /// UI의 Slider와 양방향 바인딩
    /// </summary>
    [ObservableProperty]
    private double thresholdSeconds;

    /// <summary>
    /// 진동 활성화 여부
    /// UI의 Switch와 양방향 바인딩
    /// </summary>
    [ObservableProperty]
    private bool vibrateEnabled;

    /// <summary>
    /// 소리 활성화 여부
    /// </summary>
    [ObservableProperty]
    private bool soundEnabled;

    /// <summary>
    /// 사진 캡처 활성화 여부
    /// </summary>
    [ObservableProperty]
    private bool capturePhotoEnabled;

    #endregion

    #region Constructor

    public SettingsViewModel()
    {
        Title = "설정";
        LoadSettings();
    }

    #endregion

    #region Commands

    /// <summary>
    /// 설정 저장 커맨드
    /// [RelayCommand]가 자동으로 다음을 생성:
    /// - SaveCommand (ICommand 타입)
    /// - CanExecute 로직 (필요시)
    /// - CommandManager.InvalidateRequerySuggested() 호출
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            IsBusy = true;
            
            // 모델에 설정 저장 (MVVM의 Model 부분)
            _settings.ProtectionAction = SelectedProtectionAction;
            _settings.DisguiseType = SelectedDisguiseType;
            _settings.PeekingThresholdSeconds = ThresholdSeconds;
            _settings.VibrateOnDetection = VibrateEnabled;
            _settings.SoundOnDetection = SoundEnabled;
            _settings.CapturePhoto = CapturePhotoEnabled;

            // 실제로는 여기서 파일이나 데이터베이스에 저장
            // await _settingsService.SaveAsync(_settings);

            await Shell.Current.DisplayAlert(
                "설정 저장",
                "설정이 저장되었습니다.",
                "확인");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "오류",
                $"설정 저장 중 오류가 발생했습니다: {ex.Message}",
                "확인");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// 뒤로 가기 커맨드
    /// [RelayCommand]로 BackCommand 자동 생성
    /// </summary>
    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// 설정 초기화 커맨드
    /// </summary>
    [RelayCommand]
    private async Task ResetSettingsAsync()
    {
        var result = await Shell.Current.DisplayAlert(
            "설정 초기화",
            "모든 설정을 기본값으로 되돌리시겠습니까?",
            "초기화",
            "취소");

        if (result)
        {
            // 기본값으로 초기화
            SelectedProtectionAction = ProtectionAction.Blur;
            SelectedDisguiseType = DisguiseType.News;
            ThresholdSeconds = 1.5;
            VibrateEnabled = true;
            SoundEnabled = false;
            CapturePhotoEnabled = false;

            await Shell.Current.DisplayAlert(
                "완료",
                "설정이 초기화되었습니다.",
                "확인");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 설정 로드
    /// 실제 앱에서는 로컬 저장소나 데이터베이스에서 로드
    /// </summary>
    private void LoadSettings()
    {
        // Model에서 ViewModel로 데이터 로드 (MVVM의 데이터 흐름)
        SelectedProtectionAction = _settings.ProtectionAction;
        SelectedDisguiseType = _settings.DisguiseType;
        ThresholdSeconds = _settings.PeekingThresholdSeconds;
        VibrateEnabled = _settings.VibrateOnDetection;
        SoundEnabled = _settings.SoundOnDetection;
        CapturePhotoEnabled = _settings.CapturePhoto;
    }

    #endregion

    #region Partial Methods (Source Generator Callbacks)

    /// <summary>
    /// ThresholdSeconds 변경 시 콜백
    /// Source Generator가 자동으로 프로퍼티 변경 시 이 메서드를 호출
    /// </summary>
    partial void OnThresholdSecondsChanged(double value)
    {
        // 유효성 검사
        if (value < 0.5)
        {
            ThresholdSeconds = 0.5;
        }
        else if (value > 5.0)
        {
            ThresholdSeconds = 5.0;
        }
    }

    /// <summary>
    /// SelectedProtectionAction 변경 시 콜백
    /// </summary>
    partial void OnSelectedProtectionActionChanged(ProtectionAction value)
    {
        System.Diagnostics.Debug.WriteLine($"보호 동작 변경: {value}");
        
        // 위장 화면 옵션은 Disguise 모드일 때만 유효
        // 실제로는 UI에서 IsEnabled 바인딩으로 처리 가능
    }

    #endregion
}
