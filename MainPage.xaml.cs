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
			System.Diagnostics.Debug.WriteLine("[Camera] Starting camera preview...");
			
			// CameraView 생성 및 설정 (전면 카메라, 전체 화면)
			_cameraView = new CameraView
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Colors.Black
			};

			// 프레임 캡처 이벤트 구독 (감지 상태 업데이트용)
			_cameraView.MediaCaptured += OnMediaCaptured;
			
			System.Diagnostics.Debug.WriteLine("[Camera] MediaCaptured event subscribed");

			// 카메라 프리뷰 표시 (전체 영역)
			CameraPreviewContainer.Content = _cameraView;
			
			System.Diagnostics.Debug.WriteLine("[Camera] Camera preview started (full screen)");
			
			// 백그라운드에서 주기적으로 감지 실행 (MediaCaptured가 안 될 경우를 대비)
			StartPeriodicDetection();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Camera] Error: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"[Camera] Stack trace: {ex.StackTrace}");
			
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

	private CancellationTokenSource? _detectionCancellation;

	private void StartPeriodicDetection()
	{
		_detectionCancellation = new CancellationTokenSource();
		var token = _detectionCancellation.Token;

		_ = Task.Run(async () =>
		{
			System.Diagnostics.Debug.WriteLine("[Detection] Periodic detection started");
			int frameCount = 0;

			while (!token.IsCancellationRequested && _viewModel.IsCameraModeVisible)
			{
				try
				{
					frameCount++;
					
					// 더미 데이터로 감지 (실제 카메라 프레임 사용 시 여기를 수정)
					var dummyData = new byte[320 * 240 * 3];
					await _viewModel.ProcessCameraFrameAsync(dummyData);
					
					if (frameCount % 10 == 0) // 10프레임마다 로그
					{
						System.Diagnostics.Debug.WriteLine($"[Detection] Processed {frameCount} frames");
					}
					
					// 초당 약 3-5회 감지
					await Task.Delay(300, token);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"[Detection] Error: {ex.Message}");
					await Task.Delay(1000, token);
				}
			}
			
			System.Diagnostics.Debug.WriteLine($"[Detection] Periodic detection stopped (Total frames: {frameCount})");
		}, token);
	}

	private async void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine($"[MediaCaptured] Event triggered at {DateTime.Now:HH:mm:ss.fff}");
			
			// 캡처된 미디어가 있는지 확인
			if (e?.Media == null)
			{
				System.Diagnostics.Debug.WriteLine("[MediaCaptured] No media data");
				
				// 데이터가 없어도 감지 시도 (더미 데이터로)
				var dummyData = new byte[320 * 240 * 3]; // RGB 데이터
				await _viewModel.ProcessCameraFrameAsync(dummyData);
				return;
			}

			System.Diagnostics.Debug.WriteLine($"[MediaCaptured] Media stream available");

			// 이미지 데이터를 바이트 배열로 변환
			byte[] imageData;
			using (var stream = e.Media)
			{
				using var memoryStream = new MemoryStream();
				await stream.CopyToAsync(memoryStream);
				imageData = memoryStream.ToArray();
			}

			System.Diagnostics.Debug.WriteLine($"[MediaCaptured] Image data size: {imageData.Length} bytes");

			// 실제 얼굴 감지 수행
			if (imageData.Length > 0)
			{
				await _viewModel.ProcessCameraFrameAsync(imageData);
			}
			else
			{
				// 빈 데이터면 더미 데이터로 대체
				var dummyData = new byte[320 * 240 * 3];
				await _viewModel.ProcessCameraFrameAsync(dummyData);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[MediaCaptured] Error: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"[MediaCaptured] Stack trace: {ex.StackTrace}");
			
			// 오류 발생 시에도 UI 업데이트
			MainThread.BeginInvokeOnMainThread(() =>
			{
				_viewModel.DetectionTestStatus = $"⚠️ 처리 오류: {ex.Message}";
			});
		}
	}

	private void StopCameraPreview()
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("[Camera] Stopping camera preview...");
			
			// 주기적 감지 중지
			_detectionCancellation?.Cancel();
			_detectionCancellation?.Dispose();
			_detectionCancellation = null;
			
			if (_cameraView != null)
			{
				_cameraView.MediaCaptured -= OnMediaCaptured;
				CameraPreviewContainer.Content = null;
				_cameraView = null;
				System.Diagnostics.Debug.WriteLine("[Camera] Camera preview stopped");
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Camera] Stop error: {ex.Message}");
		}
	}
}
