using MauiApp.Models;

namespace MauiApp.Views.DisguiseScreens;

public partial class StockDisguiseView : ContentView
{
    public static readonly BindableProperty CurrentTimeProperty =
        BindableProperty.Create(nameof(CurrentTime), typeof(DateTime), typeof(StockDisguiseView), DateTime.Now);

    public DateTime CurrentTime
    {
        get => (DateTime)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    public StockDisguiseView()
    {
        InitializeComponent();
        LoadStocks();
        
        // 시간 업데이트
        var timer = Application.Current?.Dispatcher.CreateTimer();
        if (timer != null)
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            timer.Start();
        }
    }

    private void LoadStocks()
    {
        StockCollection.ItemsSource = DisguiseSampleData.GetSampleStocks();
    }
}

