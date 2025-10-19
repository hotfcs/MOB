using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using MauiApp.Utils;
using SkiaSharp;

namespace MauiApp.Services;

/// <summary>
/// ONNX 기반 실제 얼굴 감지 서비스
/// Ultra-Light Face Detection 모델 사용
/// </summary>
public class OnnxFaceDetectionService : IFaceDetectionService
{
    private InferenceSession? _session;
    private bool _isInitialized;
    private readonly string _modelPath;
    
    // 모델 설정
    private const int ModelInputWidth = 320;
    private const int ModelInputHeight = 240;
    private const float ConfidenceThreshold = 0.7f;
    private const float IouThreshold = 0.3f;

    // ImageNet 정규화 파라미터
    private static readonly float[] Mean = { 127f, 127f, 127f };
    private static readonly float[] Std = { 128f, 128f, 128f };

    public bool IsInitialized => _isInitialized;

    public OnnxFaceDetectionService()
    {
        // 모델 파일 경로 (Resources/Raw에 저장)
        _modelPath = Path.Combine(FileSystem.AppDataDirectory, "version-RFB-320.onnx");
    }

    /// <summary>
    /// ONNX 모델 초기화
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        try
        {
            // 모델 파일이 없으면 Resources에서 복사
            if (!File.Exists(_modelPath))
            {
                await CopyModelFromResourcesAsync();
            }

            // ONNX Runtime 세션 생성
            var sessionOptions = new SessionOptions
            {
                // CPU에서 실행 (모바일에서는 CPU가 더 안정적)
                ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
            };

            _session = new InferenceSession(_modelPath, sessionOptions);
            _isInitialized = true;

            System.Diagnostics.Debug.WriteLine("ONNX Face Detection Model loaded successfully");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ONNX initialization error: {ex.Message}");
            _isInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// 얼굴 감지 수행
    /// </summary>
    public async Task<FaceDetectionResult> DetectFacesAsync(byte[] imageData)
    {
        if (!_isInitialized || _session == null)
        {
            return new FaceDetectionResult
            {
                FaceCount = 0,
                HasPeekingDetected = false
            };
        }

        return await Task.Run(() =>
        {
            try
            {
                // 1. 이미지 전처리
                using var bitmap = ImageProcessor.BytesToBitmap(imageData);
                if (bitmap == null)
                {
                    return new FaceDetectionResult { FaceCount = 0 };
                }

                var originalWidth = bitmap.Width;
                var originalHeight = bitmap.Height;

                // 2. 모델 입력 크기로 리사이즈
                using var resized = ImageProcessor.ResizeBitmap(bitmap, ModelInputWidth, ModelInputHeight);

                // 3. float 배열로 변환 및 정규화
                var inputData = ImageProcessor.BitmapToFloatArray(resized, normalize: false);
                
                // Mean/Std 정규화
                NormalizeInput(inputData, Mean, Std);

                // 4. ONNX Tensor 생성
                var inputTensor = new DenseTensor<float>(inputData, new[] { 1, 3, ModelInputHeight, ModelInputWidth });

                // 5. 추론 실행
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("input", inputTensor)
                };

                using var results = _session.Run(inputs);
                
                // 6. 결과 파싱
                var boxes = results.First(r => r.Name == "boxes").AsEnumerable<float>().ToArray();
                var scores = results.First(r => r.Name == "scores").AsEnumerable<float>().ToArray();

                // 7. 얼굴 박스 추출
                var detectedFaces = ParseDetections(boxes, scores, originalWidth, originalHeight);

                // 8. NMS (Non-Maximum Suppression) 적용
                var finalFaces = ApplyNMS(detectedFaces, IouThreshold);

                // 9. 결과 생성
                return CreateResult(finalFaces, originalWidth, originalHeight);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Face detection error: {ex.Message}");
                return new FaceDetectionResult { FaceCount = 0 };
            }
        });
    }

    /// <summary>
    /// 입력 데이터 정규화
    /// </summary>
    private void NormalizeInput(float[] data, float[] mean, float[] std)
    {
        int channelSize = ModelInputHeight * ModelInputWidth;
        
        for (int c = 0; c < 3; c++)
        {
            int offset = c * channelSize;
            for (int i = 0; i < channelSize; i++)
            {
                data[offset + i] = (data[offset + i] - mean[c]) / std[c];
            }
        }
    }

    /// <summary>
    /// 검출 결과 파싱
    /// </summary>
    private List<FaceBox> ParseDetections(float[] boxes, float[] scores, int imageWidth, int imageHeight)
    {
        var faces = new List<FaceBox>();
        int numDetections = scores.Length;

        for (int i = 0; i < numDetections; i++)
        {
            float confidence = scores[i];
            
            if (confidence > ConfidenceThreshold)
            {
                // 박스 좌표 추출 (정규화된 좌표)
                int boxOffset = i * 4;
                float x1 = boxes[boxOffset] * imageWidth;
                float y1 = boxes[boxOffset + 1] * imageHeight;
                float x2 = boxes[boxOffset + 2] * imageWidth;
                float y2 = boxes[boxOffset + 3] * imageHeight;

                faces.Add(new FaceBox
                {
                    X = x1,
                    Y = y1,
                    Width = x2 - x1,
                    Height = y2 - y1,
                    Confidence = confidence
                });
            }
        }

        return faces;
    }

    /// <summary>
    /// NMS (Non-Maximum Suppression) 적용
    /// 겹치는 박스 제거
    /// </summary>
    private List<FaceBox> ApplyNMS(List<FaceBox> boxes, float iouThreshold)
    {
        if (boxes.Count == 0) return boxes;

        // 신뢰도 기준 정렬
        var sorted = boxes.OrderByDescending(b => b.Confidence).ToList();
        var result = new List<FaceBox>();

        while (sorted.Count > 0)
        {
            var best = sorted[0];
            result.Add(best);
            sorted.RemoveAt(0);

            // IOU가 임계값 이상인 박스 제거
            sorted.RemoveAll(box => CalculateIOU(best, box) > iouThreshold);
        }

        return result;
    }

    /// <summary>
    /// IOU (Intersection over Union) 계산
    /// </summary>
    private float CalculateIOU(FaceBox box1, FaceBox box2)
    {
        float x1 = Math.Max(box1.X, box2.X);
        float y1 = Math.Max(box1.Y, box2.Y);
        float x2 = Math.Min(box1.X + box1.Width, box2.X + box2.Width);
        float y2 = Math.Min(box1.Y + box1.Height, box2.Y + box2.Height);

        float intersection = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
        float area1 = box1.Width * box1.Height;
        float area2 = box2.Width * box2.Height;
        float union = area1 + area2 - intersection;

        return union > 0 ? intersection / union : 0;
    }

    /// <summary>
    /// 감지 결과 생성
    /// </summary>
    private FaceDetectionResult CreateResult(List<FaceBox> faces, int imageWidth, int imageHeight)
    {
        var result = new FaceDetectionResult
        {
            FaceCount = faces.Count,
            Faces = new List<FaceInfo>()
        };

        // 화면 중앙 좌표
        float centerX = imageWidth / 2f;
        float centerY = imageHeight / 2f;

        foreach (var face in faces)
        {
            // 얼굴이 화면 중앙에서 얼마나 떨어져 있는지 계산
            float dx = face.CenterX - centerX;
            float dy = face.CenterY - centerY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            float angle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            // 정규화된 좌표
            var faceInfo = new FaceInfo
            {
                X = face.X / imageWidth,
                Y = face.Y / imageHeight,
                Width = face.Width / imageWidth,
                Height = face.Height / imageHeight,
                AngleFromCenter = angle,
                IsOwner = false // 추후 얼굴 인식으로 판별 가능
            };

            result.Faces.Add(faceInfo);
        }

        // 엿보기 감지 로직
        if (result.FaceCount > 1)
        {
            // 여러 얼굴이 감지되면 엿보기로 판단
            result.HasPeekingDetected = true;
        }
        else if (result.FaceCount == 1)
        {
            // 얼굴이 화면 중앙에서 많이 벗어나면 엿보기로 판단
            var face = result.Faces[0];
            float distanceFromCenter = (float)Math.Sqrt(
                Math.Pow(face.X + face.Width / 2 - 0.5, 2) +
                Math.Pow(face.Y + face.Height / 2 - 0.5, 2)
            );

            // 중앙에서 30% 이상 벗어나면 엿보기
            if (distanceFromCenter > 0.3)
            {
                result.HasPeekingDetected = true;
            }
        }

        return result;
    }

    /// <summary>
    /// Resources에서 모델 파일 복사
    /// </summary>
    private async Task CopyModelFromResourcesAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("version-RFB-320.onnx");
            using var fileStream = File.Create(_modelPath);
            await stream.CopyToAsync(fileStream);
            
            System.Diagnostics.Debug.WriteLine($"Model copied to: {_modelPath}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to copy model: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}

