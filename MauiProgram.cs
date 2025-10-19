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
		
		// 실제 ONNX 얼굴 감지 서비스 사용
		builder.Services.AddSingleton<IFaceDetectionService, OnnxFaceDetectionService>();
		// 시뮬레이션 모드로 테스트하려면 아래 주석 해제:
		// builder.Services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
		
		builder.Services.AddSingleton<IProtectionService, ProtectionService>();
		
		// ViewModels 등록
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<TestModeViewModel>();
		
		// Views 등록
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<TestModePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
