# ONNX 얼굴 감지 모델 다운로드 스크립트
# Ultra-Light Face Detection Model

$modelUrl = "https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB/raw/master/models/onnx/version-RFB-320.onnx"
$targetPath = "../Resources/Raw/version-RFB-320.onnx"

Write-Host "📥 ONNX 얼굴 감지 모델 다운로드 중..." -ForegroundColor Cyan
Write-Host "URL: $modelUrl" -ForegroundColor Gray

# Resources/Raw 디렉토리 생성
$rawDir = Split-Path -Parent $targetPath
if (!(Test-Path $rawDir)) {
    New-Item -ItemType Directory -Path $rawDir -Force | Out-Null
    Write-Host "✅ Resources/Raw 디렉토리 생성됨" -ForegroundColor Green
}

try {
    # 모델 다운로드
    Invoke-WebRequest -Uri $modelUrl -OutFile $targetPath -UseBasicParsing
    
    # 파일 크기 확인
    $fileInfo = Get-Item $targetPath
    $fileSizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
    
    Write-Host "✅ 모델 다운로드 완료!" -ForegroundColor Green
    Write-Host "📁 위치: $targetPath" -ForegroundColor Yellow
    Write-Host "📊 크기: $fileSizeMB MB" -ForegroundColor Yellow
    
    if ($fileSizeMB -lt 0.5) {
        Write-Host "⚠️  경고: 파일 크기가 너무 작습니다. 다운로드가 올바르지 않을 수 있습니다." -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ 다운로드 실패: $_" -ForegroundColor Red
    Write-Host "" 
    Write-Host "수동 다운로드 방법:" -ForegroundColor Yellow
    Write-Host "1. 브라우저에서 다음 URL로 이동:" -ForegroundColor Gray
    Write-Host "   $modelUrl" -ForegroundColor Cyan
    Write-Host "2. 파일을 다운로드하여 다음 경로에 저장:" -ForegroundColor Gray
    Write-Host "   $targetPath" -ForegroundColor Cyan
    exit 1
}

Write-Host ""
Write-Host "🎉 모델 설정 완료!" -ForegroundColor Green
Write-Host ""
Write-Host "다음 단계:" -ForegroundColor Yellow
Write-Host "1. 프로젝트 빌드: dotnet build" -ForegroundColor Gray
Write-Host "2. 앱 실행: dotnet build -t:Run -f net9.0-windows10.0.19041.0" -ForegroundColor Gray
Write-Host ""
Write-Host "자세한 정보는 Docs/ONNX_FaceDetection_Setup.md를 참조하세요." -ForegroundColor Gray

