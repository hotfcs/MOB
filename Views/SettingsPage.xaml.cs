using MauiApp.ViewModels;
using MauiApp.Models;

namespace MauiApp.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;

	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
		
		// 초기 선택 설정
		SetInitialSelection();
	}

	private void SetInitialSelection()
	{
		// 보호 동작 초기 선택
		var protectionAction = (int)_viewModel.SelectedProtectionAction;
		foreach (var child in ((VerticalStackLayout)((Frame)((ScrollView)Content).Content).Content).Children)
		{
			if (child is Frame frame && frame.Content is VerticalStackLayout stack)
			{
				foreach (var item in stack.Children)
				{
					if (item is RadioButton rb && rb.GroupName == "ProtectionAction" && rb.Value.ToString() == protectionAction.ToString())
					{
						rb.IsChecked = true;
						break;
					}
				}
			}
		}

		// 위장 화면 타입 초기 선택
		var disguiseType = (int)_viewModel.SelectedDisguiseType;
		foreach (var child in DisguiseTypeFrame.Content is VerticalStackLayout typeStack ? typeStack.Children : Array.Empty<IView>())
		{
			if (child is RadioButton rb && rb.GroupName == "DisguiseType" && rb.Value.ToString() == disguiseType.ToString())
			{
				rb.IsChecked = true;
				break;
			}
		}

		// 위장 화면 선택 시에만 타입 프레임 표시
		UpdateDisguiseTypeVisibility();
	}

	private void OnProtectionActionChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (sender is RadioButton rb && rb.IsChecked && int.TryParse(rb.Value.ToString(), out int value))
		{
			_viewModel.SelectedProtectionAction = (ProtectionAction)value;
			UpdateDisguiseTypeVisibility();
		}
	}

	private void OnDisguiseTypeChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (sender is RadioButton rb && rb.IsChecked && int.TryParse(rb.Value.ToString(), out int value))
		{
			_viewModel.SelectedDisguiseType = (DisguiseType)value;
		}
	}

	private void UpdateDisguiseTypeVisibility()
	{
		// 위장 화면 선택 시에만 타입 선택 프레임 표시
		DisguiseTypeFrame.IsVisible = _viewModel.SelectedProtectionAction == ProtectionAction.Disguise;
	}
}

