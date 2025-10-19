using MauiApp.Models;

namespace MauiApp.Views.DisguiseScreens;

public partial class CalendarDisguiseView : ContentView
{
    public static readonly BindableProperty CurrentDateProperty =
        BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(CalendarDisguiseView), DateTime.Now);

    public DateTime CurrentDate
    {
        get => (DateTime)GetValue(CurrentDateProperty);
        set => SetValue(CurrentDateProperty, value);
    }

    public CalendarDisguiseView()
    {
        InitializeComponent();
        LoadEvents();
    }

    private void LoadEvents()
    {
        EventsCollection.ItemsSource = DisguiseSampleData.GetSampleEvents();
    }
}

