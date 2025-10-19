# 화면 엿보기 방지 앱 (Anti-Peeking App)

.NET MAUI로 개발된 크로스 플랫폼 화면 엿보기 방지 애플리케이션입니다.

## 📱 주요 기능

### 1. 핵심 기능: 지능형 감지 및 실시간 반응

#### 엿보기 행동 감지
- **전면 카메라 AI 감지**: 전면 카메라와 머신러닝 모델을 활용하여 여러 얼굴이나 비정상적인 각도에서 화면을 보는 사람을 감지
- **시선 각도 분석**: 측면이나 후면에서 보는 시선을 감지
- **머리 자세 추정**: 일시적인 시선과 장시간 엿보기를 구분

#### 즉시 반응 메커니즘
- **화면 블러/가리기**: 엿보기 감지 시 즉시 화면 내용을 보호
- **위장 화면**: 뉴스, 전자책 등 사전 설정된 위장 화면으로 전환
- **경고 알림**: 진동 및 소리로 사용자에게 알림

### 2. 다중 시나리오 모드

#### 🚇 출퇴근 모드
- 지하철, 버스 등 혼잡한 환경에 최적화
- 최고 민감도로 동작
- 빠른 반응 시간

#### 💼 사무실 모드
- 사무실 환경에 최적화
- 측면 및 후면 감지에 집중
- 중간 민감도

#### 👥 회의 모드
- 회의실 환경에 최적화
- 진동 알림만 활성화
- 화면 전환 없음 (불필요한 주목 방지)

#### ⚙️ 사용자 정의 모드
- 사용자가 직접 민감도 및 동작 설정 가능

### 3. 보호 동작 옵션

- **화면 블러 처리**: 화면에 블러 효과 적용
- **위장 화면 표시**: 뉴스, 주식, 전자책 등으로 위장
- **화면 가리기**: 검은색 오버레이로 완전 차단
- **진동만**: 시각적 변화 없이 진동으로만 알림

## 🔒 프라이버시 보호

- **로컬 처리**: 모든 데이터는 기기 내부에서만 처리됩니다
- **서버 전송 없음**: 카메라 영상이나 개인정보가 외부로 전송되지 않습니다
- **사용자 제어**: 모든 기능은 사용자가 직접 활성화/비활성화 가능
- **투명성**: 카메라 사용 시 명확한 표시

## 🛠️ 기술 스택

- **.NET 9.0** - 최신 .NET 프레임워크
- **.NET MAUI** - 크로스 플랫폼 UI 프레임워크
- **CommunityToolkit.Maui** - 추가 UI 컴포넌트
- **CommunityToolkit.Maui.Camera** - 카메라 접근
- **CommunityToolkit.Mvvm** - Source Generator 기반 MVVM 구현
- **Microsoft.ML.OnnxRuntime** - 머신러닝 추론 (얼굴 감지용)
- **MVVM 패턴** - 현대적인 Source Generator 기반 아키텍처

## 📦 프로젝트 구조

```
MauiApp/
├── Models/              # 데이터 모델
│   ├── DetectionMode.cs      # 감지 모드 정의
│   ├── AppSettings.cs        # 앱 설정
│   └── PeekingEvent.cs       # 감지 이벤트
├── Services/            # 비즈니스 로직
│   ├── ICameraService.cs     # 카메라 서비스 인터페이스
│   ├── CameraService.cs      # 카메라 서비스 구현
│   ├── IFaceDetectionService.cs  # 얼굴 감지 인터페이스
│   ├── FaceDetectionService.cs   # 얼굴 감지 구현
│   ├── IProtectionService.cs     # 보호 서비스 인터페이스
│   └── ProtectionService.cs      # 보호 서비스 구현
├── ViewModels/          # 뷰모델 (MVVM)
│   ├── BaseViewModel.cs      # 기본 뷰모델
│   ├── MainViewModel.cs      # 메인 페이지 뷰모델
│   └── SettingsViewModel.cs  # 설정 페이지 뷰모델
├── Views/               # UI 페이지
│   ├── SettingsPage.xaml     # 설정 페이지
│   └── SettingsPage.xaml.cs
├── MainPage.xaml        # 메인 페이지
├── MainPage.xaml.cs
├── App.xaml             # 앱 진입점
├── AppShell.xaml        # 네비게이션
└── MauiProgram.cs       # 앱 구성
```

## 🚀 시작하기

### 필수 요구사항

- .NET 9.0 SDK
- Visual Studio 2022 (17.8 이상) 또는 Visual Studio Code
- MAUI 워크로드 설치

### 워크로드 설치

```bash
dotnet workload install maui
```

### 빌드 및 실행

#### Windows
```bash
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

#### Android
```bash
dotnet build -t:Run -f net9.0-android
```

#### iOS (macOS 필요)
```bash
dotnet build -t:Run -f net9.0-ios
```

#### macOS
```bash
dotnet build -t:Run -f net9.0-maccatalyst
```

## 🏗️ 아키텍처

### MVVM 패턴 (Source Generator 기반)

이 프로젝트는 **CommunityToolkit.Mvvm**을 사용하여 현대적인 MVVM 패턴을 구현합니다.

#### 주요 특징:

1. **[ObservableProperty]** - 2줄로 완전한 Observable Property 생성
   ```csharp
   [ObservableProperty]
   private string statusMessage;
   // Source Generator가 StatusMessage 프로퍼티 자동 생성
   ```

2. **[RelayCommand]** - 간단한 Command 구현
   ```csharp
   [RelayCommand]
   private async Task StartAsync() { }
   // Source Generator가 StartCommand 자동 생성
   ```

3. **Partial Methods** - 프로퍼티 변경 콜백
   ```csharp
   partial void OnStatusMessageChanged(string value)
   {
       // 검증 로직
   }
   ```

#### 자세한 내용:
- 📚 [MVVM 아키텍처 완전 가이드](Docs/MVVM_Architecture.md)
- 📊 [MVVM 개선 사항 상세](Docs/MVVM_Improvements.md)

### 🤖 실제 AI 얼굴 감지

**Ultra-Light Face Detection** ONNX 모델 통합:

```bash
# 모델 자동 다운로드 (Windows)
powershell -ExecutionPolicy Bypass -File Scripts/download-model.ps1

# 모델 자동 다운로드 (Linux/macOS)
bash Scripts/download-model.sh
```

**주요 특징**:
- ⚡ 빠른 추론 속도 (모바일 최적화)
- 🎯 높은 정확도 (신뢰도 70% 이상)
- 📦 가벼운 모델 크기 (1.2MB)
- 👥 다중 얼굴 동시 감지
- 🔄 NMS (Non-Maximum Suppression) 적용

**설정 가이드**: [ONNX 얼굴 감지 완전 가이드](Docs/ONNX_FaceDetection_Setup.md)

### 🧪 테스트 모드 (개발자 도구)

개발 및 디버깅을 위한 종합 테스트 도구:

**주요 기능**:
- 🎭 엿보기 감지 시뮬레이션
- 👁️ 5가지 위장 화면 미리보기
- 📷 카메라 권한 테스트
- 🤖 얼굴 감지 서비스 테스트
- 🎬 모든 동작 자동 테스트
- 📝 실시간 테스트 로그

**접근**: 메인 화면 하단 **🧪 테스트** 버튼

**상세 가이드**: [테스트 모드 완전 가이드](Docs/TestMode_Guide.md)

---

## 📱 사용 방법

1. **앱 실행**: 앱을 실행하고 카메라 권한을 허용합니다
2. **모드 선택**: 현재 상황에 맞는 감지 모드를 선택합니다
   - 지하철/버스 → 출퇴근 모드
   - 사무실 → 사무실 모드
   - 회의실 → 회의 모드
3. **감지 활성화**: 상단의 스위치를 켜서 감지를 시작합니다
4. **설정 조정**: 하단의 설정 버튼으로 세부 설정을 변경합니다
   - 보호 동작 방식
   - 감지 민감도
   - 알림 설정

## ⚠️ 주의사항

### 배터리 소모
- 지속적인 카메라 사용으로 배터리 소모가 있을 수 있습니다
- 필요할 때만 활성화하는 것을 권장합니다
- 간헐적 감지 메커니즘으로 배터리 효율 최적화

### 프라이버시
- 사진 촬영 기능은 기본적으로 비활성화되어 있습니다
- 활성화 시 현지 법률을 준수해야 합니다
- 타인의 사진을 무단으로 촬영하는 것은 불법일 수 있습니다

### 정확도
- ✅ **실제 ONNX 얼굴 감지 모델 통합 완료!**
- Ultra-Light Face Detection 모델 사용 (1.2MB)
- 조명 조건에 따라 감지 정확도가 달라질 수 있습니다
- 모델 설정: [ONNX 얼굴 감지 설정 가이드](Docs/ONNX_FaceDetection_Setup.md)

## 🔮 향후 개발 계획

### 단기
- [x] 실제 얼굴 감지 모델 통합 (ONNX) ✅
- [x] 위장 화면 템플릿 추가 ✅
- [x] 테스트 모드 (개발/디버깅 도구) ✅
- [ ] 감지 로그 및 통계
- [ ] 다크 모드 지원

### 중기
- [ ] 얼굴 인식으로 주인 식별
- [ ] 프라이버시 영역 지정 기능
- [ ] 클라우드 동기화 (설정만)
- [ ] 위젯 지원

### 장기
- [ ] 고급 시선 추적
- [ ] 거리 감지
- [ ] 머신러닝 모델 커스터마이징
- [ ] 웨어러블 통합

## 📄 라이선스

이 프로젝트는 교육 및 개인 사용 목적으로 개발되었습니다.

## 🤝 기여

버그 리포트, 기능 제안, 코드 기여를 환영합니다!

## 📧 문의

프로젝트 관련 문의사항이 있으시면 이슈를 생성해주세요.

---

**면책 조항**: 이 앱은 개인의 프라이버시 보호를 목적으로 개발되었습니다. 타인의 프라이버시를 침해하는 용도로 사용해서는 안 됩니다. 사용자는 현지 법률과 규정을 준수해야 할 책임이 있습니다.

