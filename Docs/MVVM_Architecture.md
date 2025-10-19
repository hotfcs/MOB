# MVVM 아키텍처 가이드

이 문서는 프로젝트에서 사용하는 MVVM (Model-View-ViewModel) 아키텍처 패턴에 대해 설명합니다.

## 목차

1. [MVVM이란?](#mvvm이란)
2. [MVVM의 구성요소](#mvvm의-구성요소)
3. [프로젝트 구조](#프로젝트-구조)
4. [CommunityToolkit.Mvvm 사용](#communitytoolkitmvvm-사용)
5. [실전 예제](#실전-예제)
6. [베스트 프랙티스](#베스트-프랙티스)

---

## MVVM이란?

**MVVM (Model-View-ViewModel)**은 UI와 비즈니스 로직을 분리하는 소프트웨어 아키텍처 패턴입니다.

### MVVM의 장점

✅ **관심사의 분리** (Separation of Concerns)
- UI 코드와 비즈니스 로직이 분리되어 유지보수가 쉬움

✅ **테스트 가능성** (Testability)
- ViewModel은 UI 없이도 단위 테스트 가능
- View는 ViewModel에 의존하지만, ViewModel은 View를 알 필요 없음

✅ **재사용성** (Reusability)
- ViewModel은 여러 View에서 재사용 가능
- Model은 다른 프로젝트에서도 재사용 가능

✅ **병렬 개발** (Parallel Development)
- 디자이너는 View(XAML) 작업
- 개발자는 ViewModel과 Model 작업
- 동시에 진행 가능

---

## MVVM의 구성요소

### 1. Model (모델)

**역할**: 데이터와 비즈니스 로직을 담당

```csharp
// Models/DetectionMode.cs
public enum DetectionMode
{
    Commute,    // 출퇴근 모드
    Office,     // 사무실 모드
    Meeting,    // 회의 모드
    Custom      // 사용자 정의
}

// Models/AppSettings.cs
public class AppSettings
{
    public DetectionMode CurrentMode { get; set; }
    public bool IsDetectionEnabled { get; set; }
    public double PeekingThresholdSeconds { get; set; }
    // ... 기타 설정
}
```

**특징**:
- 순수한 데이터 클래스
- UI에 대한 의존성 없음
- 다른 프로젝트에서도 재사용 가능

### 2. View (뷰)

**역할**: UI 표시 및 사용자 입력 처리

```xml
<!-- MainPage.xaml -->
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             x:DataType="vm:MainViewModel">
    
    <!-- 데이터 바인딩 -->
    <Label Text="{Binding StatusMessage}" />
    
    <!-- 커맨드 바인딩 -->
    <Button Text="시작" 
            Command="{Binding ToggleDetectionCommand}" />
    
    <!-- 양방향 바인딩 -->
    <Switch IsToggled="{Binding IsDetectionActive}" />
    
</ContentPage>
```

**특징**:
- XAML로 UI 정의
- Code-behind는 최소화
- ViewModel과 데이터 바인딩으로 연결

### 3. ViewModel (뷰모델)

**역할**: View와 Model 사이의 중재자

```csharp
// ViewModels/MainViewModel.cs
public partial class MainViewModel : BaseViewModel
{
    // Observable Property - UI 자동 업데이트
    [ObservableProperty]
    private string statusMessage = "대기 중";
    
    // Command - UI 이벤트 처리
    [RelayCommand]
    private async Task StartDetectionAsync()
    {
        // 비즈니스 로직
        StatusMessage = "감지 시작...";
        await _cameraService.StartAsync();
    }
}
```

**특징**:
- INotifyPropertyChanged 구현 (자동 UI 업데이트)
- ICommand 구현 (사용자 동작 처리)
- View에 대한 직접적인 참조 없음

---

## 프로젝트 구조

```
MauiApp/
│
├── Models/                          # 📦 Model (데이터)
│   ├── DetectionMode.cs            # 감지 모드 열거형
│   ├── AppSettings.cs              # 앱 설정 데이터
│   └── PeekingEvent.cs             # 이벤트 데이터
│
├── ViewModels/                      # 🎯 ViewModel (로직)
│   ├── BaseViewModel.cs            # 기본 ViewModel
│   ├── MainViewModel.cs            # 메인 화면 로직
│   └── SettingsViewModel.cs        # 설정 화면 로직
│
├── Views/                           # 🎨 View (UI)
│   ├── MainPage.xaml               # 메인 화면 UI
│   └── SettingsPage.xaml           # 설정 화면 UI
│
└── Services/                        # ⚙️ Business Services
    ├── ICameraService.cs           # 카메라 서비스 인터페이스
    ├── CameraService.cs            # 카메라 서비스 구현
    └── ...
```

### 데이터 흐름

```
┌─────────┐         ┌──────────────┐         ┌─────────┐
│  View   │ ◄─────► │  ViewModel   │ ◄─────► │  Model  │
│ (XAML)  │ Binding │   (Logic)    │  Uses   │ (Data)  │
└─────────┘         └──────────────┘         └─────────┘
     │                      │
     │                      ▼
     │              ┌─────────────┐
     └──────────────┤  Services   │
        Commands    └─────────────┘
```

---

## CommunityToolkit.Mvvm 사용

### 설치

```xml
<!-- MauiApp.csproj -->
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
```

### 주요 기능

#### 1. [ObservableProperty]

**Before (전통적인 방식)**:
```csharp
private string _statusMessage;
public string StatusMessage
{
    get => _statusMessage;
    set
    {
        if (_statusMessage != value)
        {
            _statusMessage = value;
            OnPropertyChanged(nameof(StatusMessage));
        }
    }
}
```

**After (Source Generator 사용)**:
```csharp
[ObservableProperty]
private string statusMessage;

// Source Generator가 자동으로 다음을 생성:
// - public string StatusMessage { get; set; }
// - INotifyPropertyChanged 구현
// - OnStatusMessageChanged() partial 메서드
```

#### 2. [RelayCommand]

**Before**:
```csharp
private ICommand? _startCommand;
public ICommand StartCommand => _startCommand ??= new Command(async () => await StartAsync());

private async Task StartAsync()
{
    // 로직
}
```

**After**:
```csharp
[RelayCommand]
private async Task StartAsync()
{
    // 로직
}

// Source Generator가 자동으로 생성:
// - public IAsyncRelayCommand StartCommand { get; }
```

#### 3. [NotifyPropertyChangedFor]

```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(FullName))]
private string firstName;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(FullName))]
private string lastName;

// Computed Property
public string FullName => $"{FirstName} {LastName}";
```

FirstName이나 LastName이 변경되면 FullName도 자동으로 UI 업데이트!

#### 4. Partial Methods (콜백)

```csharp
[ObservableProperty]
private double threshold;

// Source Generator가 이 메서드를 호출
partial void OnThresholdChanged(double value)
{
    // 유효성 검사
    if (value < 0)
        Threshold = 0;
}
```

---

## 실전 예제

### 예제 1: 간단한 카운터

#### Model
```csharp
// Models/Counter.cs
public class Counter
{
    public int Count { get; set; }
    public int Increment { get; set; } = 1;
}
```

#### ViewModel
```csharp
// ViewModels/CounterViewModel.cs
public partial class CounterViewModel : BaseViewModel
{
    private readonly Counter _model = new();
    
    [ObservableProperty]
    private int count;
    
    [ObservableProperty]
    private string displayText = "Count: 0";
    
    [RelayCommand]
    private void Increment()
    {
        Count++;
        _model.Count = Count;
        DisplayText = $"Count: {Count}";
    }
    
    [RelayCommand]
    private void Reset()
    {
        Count = 0;
        _model.Count = 0;
        DisplayText = "Count: 0";
    }
}
```

#### View
```xml
<!-- Views/CounterPage.xaml -->
<ContentPage x:DataType="vm:CounterViewModel">
    <VerticalStackLayout Padding="30" Spacing="20">
        
        <Label Text="{Binding DisplayText}" 
               FontSize="24" />
        
        <Button Text="증가" 
                Command="{Binding IncrementCommand}" />
        
        <Button Text="초기화" 
                Command="{Binding ResetCommand}" />
        
    </VerticalStackLayout>
</ContentPage>
```

### 예제 2: 설정 화면

#### ViewModel
```csharp
public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool notificationsEnabled;
    
    [ObservableProperty]
    private int fontSize = 14;
    
    [ObservableProperty]
    private string theme = "Light";
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        // 설정 저장
        await _settingsService.SaveAsync(new Settings
        {
            NotificationsEnabled = NotificationsEnabled,
            FontSize = FontSize,
            Theme = Theme
        });
        
        await Shell.Current.DisplayAlert("성공", "설정이 저장되었습니다.", "확인");
    }
    
    partial void OnFontSizeChanged(int value)
    {
        // 유효성 검사
        if (value < 10) FontSize = 10;
        if (value > 30) FontSize = 30;
    }
}
```

#### View
```xml
<ContentPage x:DataType="vm:SettingsViewModel">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="15">
            
            <!-- 알림 설정 -->
            <Grid ColumnDefinitions="*,Auto">
                <Label Text="알림 활성화" />
                <Switch Grid.Column="1" 
                        IsToggled="{Binding NotificationsEnabled}" />
            </Grid>
            
            <!-- 폰트 크기 -->
            <Label Text="{Binding FontSize, StringFormat='폰트 크기: {0}'}" />
            <Slider Minimum="10" Maximum="30" 
                    Value="{Binding FontSize}" />
            
            <!-- 테마 -->
            <Label Text="테마" />
            <Picker SelectedItem="{Binding Theme}">
                <Picker.ItemsSource>
                    <x:Array Type="{x:Type x:String}">
                        <x:String>Light</x:String>
                        <x:String>Dark</x:String>
                        <x:String>Auto</x:String>
                    </x:Array>
                </Picker.ItemsSource>
            </Picker>
            
            <!-- 저장 버튼 -->
            <Button Text="저장" 
                    Command="{Binding SaveCommand}" />
                    
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

---

## 베스트 프랙티스

### 1. ViewModel은 View를 알지 못해야 함 ❌

```csharp
// ❌ 나쁜 예
public class BadViewModel : BaseViewModel
{
    private readonly MainPage _page;
    
    [RelayCommand]
    private void DoSomething()
    {
        _page.ShowPopup(); // View에 직접 접근
    }
}

// ✅ 좋은 예
public partial class GoodViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task DoSomethingAsync()
    {
        await Shell.Current.DisplayAlert("알림", "완료", "확인");
    }
}
```

### 2. INotifyPropertyChanged는 자동으로 ✅

```csharp
// ✅ Source Generator 사용
[ObservableProperty]
private string name;

// Name 프로퍼티가 자동 생성되고 UI 자동 업데이트
```

### 3. Command는 비즈니스 로직에 집중 ✅

```csharp
[RelayCommand]
private async Task SaveAsync()
{
    try
    {
        IsBusy = true; // BaseViewModel의 프로퍼티
        
        // 비즈니스 로직
        await _service.SaveDataAsync();
        
        // 결과 처리
        await Shell.Current.DisplayAlert("성공", "저장 완료", "확인");
    }
    catch (Exception ex)
    {
        await Shell.Current.DisplayAlert("오류", ex.Message, "확인");
    }
    finally
    {
        IsBusy = false;
    }
}
```

### 4. Computed Properties 활용 ✅

```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(IsValid))]
private string email;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(IsValid))]
private string password;

// Computed Property
public bool IsValid => 
    !string.IsNullOrEmpty(Email) && 
    !string.IsNullOrEmpty(Password) &&
    Password.Length >= 8;
```

XAML에서:
```xml
<Button Text="로그인" 
        IsEnabled="{Binding IsValid}"
        Command="{Binding LoginCommand}" />
```

### 5. 의존성 주입 사용 ✅

```csharp
// MauiProgram.cs
builder.Services.AddSingleton<ICameraService, CameraService>();
builder.Services.AddSingleton<MainViewModel>();

// ViewModel
public partial class MainViewModel : BaseViewModel
{
    private readonly ICameraService _cameraService;
    
    // 생성자 주입
    public MainViewModel(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
}
```

### 6. Partial Methods로 검증 로직 ✅

```csharp
[ObservableProperty]
private int age;

partial void OnAgeChanged(int value)
{
    if (value < 0) Age = 0;
    if (value > 150) Age = 150;
}
```

---

## 요약

### MVVM 패턴의 핵심

1. **Model**: 데이터와 비즈니스 규칙
2. **View**: UI 표현 (XAML)
3. **ViewModel**: View와 Model 연결 (로직)

### CommunityToolkit.Mvvm의 이점

- ✅ **생산성 향상**: 보일러플레이트 코드 제거
- ✅ **성능**: 컴파일 타임 코드 생성 (런타임 오버헤드 없음)
- ✅ **유지보수성**: 깔끔하고 읽기 쉬운 코드
- ✅ **IntelliSense**: 자동 완성 지원

### 데이터 바인딩 흐름

```
User Action → View (XAML)
           ↓
        Command → ViewModel
                    ↓
                 Business Logic
                    ↓
                 Update Property
                    ↓
        INotifyPropertyChanged
                    ↓
               View 자동 업데이트
```

---

## 추가 리소스

- [Microsoft MAUI MVVM 문서](https://learn.microsoft.com/dotnet/maui/xaml/fundamentals/mvvm)
- [CommunityToolkit.Mvvm 문서](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [.NET MAUI 샘플](https://github.com/dotnet/maui-samples)

---

**작성일**: 2025년 10월  
**버전**: 1.0  
**프로젝트**: 화면 엿보기 방지 앱

