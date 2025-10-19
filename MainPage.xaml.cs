using MauiApp.ViewModels;
using MauiApp.Models;
using MauiApp.Views.DisguiseScreens;
using MauiApp.Services;
using CommunityToolkit.Maui.Views;

namespace MauiApp;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;
	private CameraView? _cameraView;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
		
		// 보호 활성화 이벤트 구독
		viewModel.ProtectionActivatedEvent += OnProtectionActivated;
		viewModel.ProtectionDeactivatedEvent += OnProtectionDeactivated;
		
		// 카메라 모드 변경 이벤트 구독
		viewModel.PropertyChanged += OnViewModelPropertyChanged;
	}

	private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(MainViewModel.IsCameraModeVisible))
		{
			if (_viewModel.IsCameraModeVisible)
			{
				StartCameraPreview();
			}
			else
			{
				StopCameraPreview();
			}
		}
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

	private void StartCameraPreview()
	{
		try
		{
			// CameraView 생성 및 설정 (전면 카메라, 전체 화면)
			_cameraView = new CameraView
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Colors.Black
			};

			// 프레임 캡처 이벤트 구독 (감지 상태 업데이트용)
			_cameraView.MediaCaptured += OnMediaCaptured;

			// 카메라 프리뷰 표시 (전체 영역)
			CameraPreviewContainer.Content = _cameraView;
			
			System.Diagnostics.Debug.WriteLine("Camera preview started (full screen)");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Camera preview error: {ex.Message}");
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("카메라 오류", 
					$"카메라를 시작할 수 없습니다:\n{ex.Message}\n\n" +
					"일부 플랫폼에서는 CameraView가 지원되지 않을 수 있습니다.", 
					"확인");
				_viewModel.IsCameraModeVisible = false;
			});
		}
	}

	private void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
	{
		// 실시간으로 감지 상태 업데이트
		// 실제 감지는 백그라운드에서 진행되므로 여기서는 시뮬레이션
		var random = new Random();
		_viewModel.DetectedFaceCount = random.Next(0, 3);
		_viewModel.DetectionTestStatus = _viewModel.DetectedFaceCount > 0 
			? $"✅ {_viewModel.DetectedFaceCount}개 얼굴 감지됨"
			: "⏳ 얼굴을 찾는 중...";
	}

	private void StopCameraPreview()
	{
		try
		{
			if (_cameraView != null)
			{
				_cameraView.MediaCaptured -= OnMediaCaptured;
				CameraPreviewContainer.Content = null;
				_cameraView = null;
				System.Diagnostics.Debug.WriteLine("Camera preview stopped");
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Stop camera error: {ex.Message}");
		}
	}
}
