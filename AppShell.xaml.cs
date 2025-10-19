namespace MauiApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// 라우팅 등록
		Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
		Routing.RegisterRoute(nameof(Views.TestModePage), typeof(Views.TestModePage));
	}
}
