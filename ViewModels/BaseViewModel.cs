using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp.ViewModels;

/// <summary>
/// 모든 ViewModel의 기본 클래스
/// CommunityToolkit.Mvvm의 ObservableObject를 상속하여 INotifyPropertyChanged 자동 구현
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    /// <summary>
    /// 로딩 상태를 나타내는 속성
    /// [ObservableProperty] 특성을 사용하여 자동으로 IsBusy 속성과 OnIsBusyChanged 메서드 생성
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 페이지 타이틀
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;
}

