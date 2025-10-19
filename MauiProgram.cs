using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp;

public static class MauiProgram
{
	public static Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
	{
		var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitCamera()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// 서비스 등록
		builder.Services.AddSingleton<ICameraService, CameraService>();
		
		// 시뮬레이션 모드로 테스트 (카메라 프레임 캡처 확인용)
		builder.Services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
		// 실제 ONNX 모델 사용하려면 아래 주석 해제:
		// builder.Services.AddSingleton<IFaceDetectionService, OnnxFaceDetectionService>();
		
		builder.Services.AddSingleton<IProtectionService, ProtectionService>();
		
		// ViewModels 등록
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();
		
		// Views 등록
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
