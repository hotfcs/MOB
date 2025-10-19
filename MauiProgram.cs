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
		builder.Services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
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
