#!/bin/bash
# ONNX 얼굴 감지 모델 다운로드 스크립트
# Ultra-Light Face Detection Model

MODEL_URL="https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB/raw/master/models/onnx/version-RFB-320.onnx"
TARGET_PATH="../Resources/Raw/version-RFB-320.onnx"

echo "📥 ONNX 얼굴 감지 모델 다운로드 중..."
echo "URL: $MODEL_URL"

# Resources/Raw 디렉토리 생성
RAW_DIR=$(dirname "$TARGET_PATH")
if [ ! -d "$RAW_DIR" ]; then
    mkdir -p "$RAW_DIR"
    echo "✅ Resources/Raw 디렉토리 생성됨"
fi

# 모델 다운로드
if command -v curl &> /dev/null; then
    curl -L -o "$TARGET_PATH" "$MODEL_URL"
elif command -v wget &> /dev/null; then
    wget -O "$TARGET_PATH" "$MODEL_URL"
else
    echo "❌ curl 또는 wget이 필요합니다."
    exit 1
fi

# 파일 크기 확인
if [ -f "$TARGET_PATH" ]; then
    FILE_SIZE=$(du -h "$TARGET_PATH" | cut -f1)
    echo "✅ 모델 다운로드 완료!"
    echo "📁 위치: $TARGET_PATH"
    echo "📊 크기: $FILE_SIZE"
    echo ""
    echo "🎉 모델 설정 완료!"
    echo ""
    echo "다음 단계:"
    echo "1. 프로젝트 빌드: dotnet build"
    echo "2. 앱 실행: dotnet build -t:Run -f net9.0-windows10.0.19041.0"
    echo ""
    echo "자세한 정보는 Docs/ONNX_FaceDetection_Setup.md를 참조하세요."
else
    echo "❌ 다운로드 실패"
    exit 1
fi

