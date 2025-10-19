using SkiaSharp;

namespace MauiApp.Utils;

/// <summary>
/// 이미지 전처리 유틸리티
/// ONNX 모델 입력을 위한 이미지 변환 처리
/// </summary>
public static class ImageProcessor
{
    /// <summary>
    /// 바이트 배열을 SkiaSharp 비트맵으로 변환
    /// </summary>
    public static SKBitmap? BytesToBitmap(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            return null;

        try
        {
            using var ms = new MemoryStream(imageData);
            return SKBitmap.Decode(ms);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BytesToBitmap error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 비트맵을 지정된 크기로 리사이즈
    /// </summary>
    public static SKBitmap ResizeBitmap(SKBitmap original, int width, int height)
    {
        var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        return resized ?? original;
    }

    /// <summary>
    /// 비트맵을 정규화된 float 배열로 변환 (ONNX 입력용)
    /// 형식: [batch, channels, height, width]
    /// </summary>
    public static float[] BitmapToFloatArray(SKBitmap bitmap, bool normalize = true)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;
        var pixels = bitmap.Pixels;
        var floatArray = new float[3 * height * width]; // RGB channels

        int idx = 0;
        
        // CHW 형식으로 변환 (Channels, Height, Width)
        // Red channel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = pixels[y * width + x];
                floatArray[idx++] = normalize ? pixel.Red / 255f : pixel.Red;
            }
        }

        // Green channel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = pixels[y * width + x];
                floatArray[idx++] = normalize ? pixel.Green / 255f : pixel.Green;
            }
        }

        // Blue channel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = pixels[y * width + x];
                floatArray[idx++] = normalize ? pixel.Blue / 255f : pixel.Blue;
            }
        }

        return floatArray;
    }

    /// <summary>
    /// 평균과 표준편차로 정규화
    /// ImageNet 기준: mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225]
    /// </summary>
    public static void NormalizeWithMeanStd(float[] data, int channels, int height, int width,
        float[] mean, float[] std)
    {
        int channelSize = height * width;

        for (int c = 0; c < channels; c++)
        {
            int offset = c * channelSize;
            for (int i = 0; i < channelSize; i++)
            {
                data[offset + i] = (data[offset + i] - mean[c]) / std[c];
            }
        }
    }

    /// <summary>
    /// 비트맵에 얼굴 박스 그리기 (디버깅용)
    /// </summary>
    public static SKBitmap DrawFaceBoxes(SKBitmap original, List<FaceBox> faces)
    {
        var surface = SKSurface.Create(new SKImageInfo(original.Width, original.Height));
        var canvas = surface.Canvas;
        
        // 원본 이미지 그리기
        canvas.DrawBitmap(original, 0, 0);

        // 얼굴 박스 그리기
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 3,
            IsAntialias = true
        };

        foreach (var face in faces)
        {
            var rect = new SKRect(face.X, face.Y, face.X + face.Width, face.Y + face.Height);
            canvas.DrawRect(rect, paint);
            
            // 신뢰도 표시
            using var textPaint = new SKPaint
            {
                Color = SKColors.Red,
                TextSize = 20,
                IsAntialias = true
            };
            canvas.DrawText($"{face.Confidence:P0}", face.X, face.Y - 5, textPaint);
        }

        return SKBitmap.FromImage(surface.Snapshot());
    }
}

/// <summary>
/// 얼굴 박스 정보
/// </summary>
public class FaceBox
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Confidence { get; set; }

    public float CenterX => X + Width / 2;
    public float CenterY => Y + Height / 2;
}

