using MauiApp.ViewModels;
using MauiApp.Models;

namespace MauiApp.Views;

public partial class TestModePage : ContentPage
{
    private readonly TestModeViewModel _viewModel;

    public TestModePage(TestModeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // Picker 선택 변경 이벤트
        ActionPicker.SelectedIndexChanged += OnActionPickerChanged;
        DisguisePicker.SelectedIndexChanged += OnDisguisePickerChanged;
    }

    private void OnActionPickerChanged(object? sender, EventArgs e)
    {
        if (ActionPicker.SelectedIndex >= 0)
        {
            _viewModel.SelectedTestAction = (ProtectionAction)ActionPicker.SelectedIndex;
        }
    }

    private void OnDisguisePickerChanged(object? sender, EventArgs e)
    {
        if (DisguisePicker.SelectedIndex >= 0)
        {
            _viewModel.SelectedTestDisguise = (DisguiseType)DisguisePicker.SelectedIndex;
        }
    }
}

