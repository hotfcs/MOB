# ONNX 얼굴 감지 모델 설정 가이드

이 문서는 실제 ONNX 얼굴 감지 모델을 프로젝트에 통합하는 방법을 설명합니다.

## 📋 목차

1. [모델 소개](#모델-소개)
2. [모델 다운로드](#모델-다운로드)
3. [프로젝트 설정](#프로젝트-설정)
4. [사용 방법](#사용-방법)
5. [성능 최적화](#성능-최적화)
6. [문제 해결](#문제-해결)

---

## 모델 소개

### Ultra-Light Face Detection

**프로젝트**: [Ultra-Light-Fast-Generic-Face-Detector-1MB](https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB)

**특징**:
- ✅ 매우 가벼움 (1.1MB)
- ✅ 빠른 추론 속도 (모바일 최적화)
- ✅ 높은 정확도
- ✅ 여러 얼굴 동시 감지
- ✅ ONNX 형식 지원

**모델 사양**:
- 입력: `[1, 3, 240, 320]` (NCHW 형식)
- 출력: 
  - `boxes`: 얼굴 바운딩 박스 좌표
  - `scores`: 신뢰도 점수
- 포맷: ONNX

---

## 모델 다운로드

### 방법 1: GitHub에서 직접 다운로드

1. 모델 다운로드:
   ```bash
   # PowerShell
   Invoke-WebRequest -Uri "https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB/raw/master/models/onnx/version-RFB-320.onnx" -OutFile "version-RFB-320.onnx"
   
   # 또는 curl (Git Bash/Linux/macOS)
   curl -L -o version-RFB-320.onnx "https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB/raw/master/models/onnx/version-RFB-320.onnx"
   ```

2. 다운로드한 파일을 프로젝트의 `Resources/Raw/` 폴더에 복사

### 방법 2: 수동 다운로드

1. GitHub 레포지토리 방문:
   https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB

2. `models/onnx/version-RFB-320.onnx` 파일 다운로드

3. `Resources/Raw/` 폴더에 저장

### 대체 모델 (선택사항)

더 정확한 감지가 필요하면:
- **version-RFB-640.onnx** (더 큰 입력 크기, 더 정확, 더 느림)
- **version-slim-320.onnx** (더 가벼움, 약간 낮은 정확도)

---

## 프로젝트 설정

### 1. 모델 파일 배치

```
Resources/
└── Raw/
    └── version-RFB-320.onnx  ← 여기에 복사
```

### 2. .csproj 파일 업데이트

모델 파일이 앱에 포함되도록 설정:

```xml
<ItemGroup>
  <MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
</ItemGroup>
```

이미 설정되어 있으므로 추가 작업 불필요!

### 3. 서비스 등록

`MauiProgram.cs`에서 서비스 변경:

```csharp
// 기존 (시뮬레이션)
builder.Services.AddSingleton<IFaceDetectionService, FaceDetectionService>();

// 변경 (실제 ONNX)
builder.Services.AddSingleton<IFaceDetectionService, OnnxFaceDetectionService>();
```

### 4. 빌드 및 배포

```bash
# 빌드
dotnet build

# Windows 실행
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# Android 실행
dotnet build -t:Run -f net9.0-android
```

---

## 사용 방법

### 기본 사용

서비스가 이미 의존성 주입으로 설정되어 있으므로, 추가 코드 없이 자동으로 작동합니다!

```csharp
// MainViewModel에서 자동으로 사용됨
public MainViewModel(
    ICameraService cameraService,
    IFaceDetectionService faceDetectionService,  // OnnxFaceDetectionService 주입
    IProtectionService protectionService)
{
    _faceDetectionService = faceDetectionService;
}
```

### 감지 설정 조정

`Services/OnnxFaceDetectionService.cs`에서:

```csharp
// 신뢰도 임계값 (낮을수록 더 많은 얼굴 감지, 오검출 증가)
private const float ConfidenceThreshold = 0.7f;  // 0.5~0.9 권장

// IoU 임계값 (중복 박스 제거)
private const float IouThreshold = 0.3f;  // 0.3~0.5 권장

// 엿보기 판단 기준 (중앙에서의 거리)
if (distanceFromCenter > 0.3)  // 30% 이상 벗어나면 엿보기
```

---

## 성능 최적화

### 1. 추론 빈도 조절

`Models/AppSettings.cs`:
```csharp
public int DetectionFrequency { get; set; } = 3;  // 초당 3회 감지
```

배터리 절약:
- 낮은 빈도: 1-2 FPS (배터리 절약)
- 중간 빈도: 3-5 FPS (균형)
- 높은 빈도: 10+ FPS (민감, 배터리 소모)

### 2. 입력 이미지 크기

더 작은 이미지로 더 빠른 처리:

```csharp
// OnnxFaceDetectionService.cs
private const int ModelInputWidth = 320;   // 기본값
private const int ModelInputHeight = 240;  // 기본값

// 더 빠르게: 160x120 (정확도 감소)
// 더 정확하게: 640x480 (속도 감소, version-RFB-640.onnx 필요)
```

### 3. 모바일 최적화

**Android**:
- GPU 가속 사용 (선택사항):
  ```csharp
  var sessionOptions = new SessionOptions();
  sessionOptions.AppendExecutionProvider_NNAPI();  // Android Neural Networks API
  ```

**iOS**:
- Core ML 백엔드 (선택사항):
  ```csharp
  sessionOptions.AppendExecutionProvider_CoreML();
  ```

### 4. 메모리 관리

이미지 처리 후 즉시 해제:
```csharp
using var bitmap = ImageProcessor.BytesToBitmap(imageData);
// 사용 후 자동 해제
```

---

## 성능 벤치마크

### 예상 성능 (참고용)

| 플랫폼 | 입력 크기 | FPS | 배터리 영향 |
|--------|-----------|-----|-------------|
| Windows (PC) | 320x240 | 30+ | 낮음 |
| Android (중급) | 320x240 | 15-20 | 중간 |
| Android (저급) | 320x240 | 5-10 | 중간 |
| iOS (최신) | 320x240 | 20-30 | 낮음 |

### 실제 측정 방법

```csharp
var sw = System.Diagnostics.Stopwatch.StartNew();
var result = await _faceDetectionService.DetectFacesAsync(frameData);
sw.Stop();
Debug.WriteLine($"Detection time: {sw.ElapsedMilliseconds}ms");
```

---

## 문제 해결

### 문제 1: 모델 파일을 찾을 수 없음

**증상**: `FileNotFoundException` 또는 초기화 실패

**해결**:
1. `Resources/Raw/version-RFB-320.onnx` 파일 존재 확인
2. 빌드 설정 확인:
   ```xml
   <MauiAsset Include="Resources\Raw\**" />
   ```
3. Clean & Rebuild:
   ```bash
   dotnet clean
   dotnet build
   ```

### 문제 2: 얼굴 감지가 너무 느림

**해결**:
1. 입력 이미지 크기 축소
2. 감지 빈도 낮추기 (DetectionFrequency = 1-2)
3. 신뢰도 임계값 높이기 (ConfidenceThreshold = 0.8)

### 문제 3: 오검출이 많음

**해결**:
1. 신뢰도 임계값 높이기: `ConfidenceThreshold = 0.8`
2. IoU 임계값 조정: `IouThreshold = 0.4`
3. 조명 조건 개선

### 문제 4: 얼굴 감지를 못함

**해결**:
1. 신뢰도 임계값 낮추기: `ConfidenceThreshold = 0.5`
2. 카메라 해상도 확인
3. 조명 조건 확인
4. 얼굴이 너무 작거나 큰지 확인

### 문제 5: 메모리 부족

**해결**:
1. 이미지 해상도 낮추기
2. 감지 빈도 줄이기
3. 사용 후 리소스 해제 확인
4. 백그라운드에서 GC.Collect() 주기적 호출

---

## 디버깅

### 얼굴 박스 시각화

```csharp
// ImageProcessor.cs 사용
var debugImage = ImageProcessor.DrawFaceBoxes(bitmap, detectedFaces);
// 이미지 저장 또는 표시
```

### 로그 확인

```csharp
System.Diagnostics.Debug.WriteLine($"Detected {result.FaceCount} faces");
foreach (var face in result.Faces)
{
    Debug.WriteLine($"Face at ({face.X}, {face.Y}) " +
                   $"size: {face.Width}x{face.Height} " +
                   $"angle: {face.AngleFromCenter}°");
}
```

---

## 추가 개선 사항

### 1. 얼굴 인식 추가

주인 얼굴 학습:
- FaceNet 또는 ArcFace 모델 사용
- 얼굴 임베딩 저장
- 코사인 유사도로 비교

### 2. 시선 추적

더 정확한 엿보기 감지:
- MediaPipe Iris 모델
- 눈동자 위치 추적
- 시선 방향 계산

### 3. 표정 인식

상황에 따른 반응:
- Emotion Detection 모델
- 놀람 표정 감지 시 경고
- 화난 표정 감지 시 조치

---

## 참고 자료

- **Ultra-Light Face Detection GitHub**:  
  https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB

- **ONNX Runtime 문서**:  
  https://onnxruntime.ai/docs/

- **MAUI File System API**:  
  https://learn.microsoft.com/dotnet/maui/platform-integration/storage/file-system-helpers

- **SkiaSharp 문서**:  
  https://learn.microsoft.com/xamarin/skiasharp/

---

## 라이선스

Ultra-Light Face Detection 모델은 MIT 라이선스입니다.  
상업적 사용 가능하지만, 원저작자 표기를 권장합니다.

---

**마지막 업데이트**: 2025년 10월  
**버전**: 1.0  
**작성자**: Anti-Peeking App Team

