using System.Globalization;

namespace MauiApp.Converters;

/// <summary>
/// 카메라 모드 상태에 따라 버튼 텍스트 변경
/// </summary>
public class BoolToCameraButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isVisible)
        {
            return isVisible ? "✖️ 카메라 종료" : "📹 카메라 모드";
        }
        return "📹 카메라 모드";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

