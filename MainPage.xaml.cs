using MauiApp.ViewModels;
using MauiApp.Models;
using MauiApp.Views.DisguiseScreens;
using MauiApp.Services;

namespace MauiApp;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
		
		// 보호 활성화 이벤트 구독
		viewModel.ProtectionActivatedEvent += OnProtectionActivated;
		viewModel.ProtectionDeactivatedEvent += OnProtectionDeactivated;
	}

	private void OnProtectionActivated(ProtectionAction action, DisguiseType disguiseType)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			// 모든 오버레이 숨기기
			BlurOverlay.IsVisible = false;
			CoverOverlay.IsVisible = false;
			DisguiseContainer.IsVisible = false;
			DisguiseContainer.Content = null;

			// 보호 동작에 따라 표시
			switch (action)
			{
				case ProtectionAction.Blur:
					BlurOverlay.IsVisible = true;
					break;

				case ProtectionAction.Cover:
					CoverOverlay.IsVisible = true;
					break;

				case ProtectionAction.Disguise:
					LoadDisguiseScreen(disguiseType);
					DisguiseContainer.IsVisible = true;
					break;

				case ProtectionAction.VibrateOnly:
					// 진동만, 화면 변화 없음
					break;
			}
		});
	}

	private void OnProtectionDeactivated()
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			BlurOverlay.IsVisible = false;
			CoverOverlay.IsVisible = false;
			DisguiseContainer.IsVisible = false;
			DisguiseContainer.Content = null;
		});
	}

	private void LoadDisguiseScreen(DisguiseType disguiseType)
	{
		ContentView? disguiseView = disguiseType switch
		{
			DisguiseType.News => new NewsDisguiseView(),
			DisguiseType.StockMarket => new StockDisguiseView(),
			DisguiseType.EBook => new EBookDisguiseView(),
			DisguiseType.Calculator => new CalculatorDisguiseView(),
			DisguiseType.Calendar => new CalendarDisguiseView(),
			_ => new NewsDisguiseView()
		};

		DisguiseContainer.Content = disguiseView;
	}
}
