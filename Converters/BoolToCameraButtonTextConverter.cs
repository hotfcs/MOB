using System.Globalization;

namespace MauiApp.Converters;

/// <summary>
/// 감지 테스트 상태에 따라 버튼 텍스트 변경
/// </summary>
public class BoolToCameraButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isVisible)
        {
            return isVisible ? "✖️ 테스트 종료" : "🔍 감지 테스트";
        }
        return "🔍 감지 테스트";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

