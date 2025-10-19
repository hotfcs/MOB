using MauiApp.Models;

namespace MauiApp.Views.DisguiseScreens;

public partial class NewsDisguiseView : ContentView
{
    public static readonly BindableProperty CurrentTimeProperty =
        BindableProperty.Create(nameof(CurrentTime), typeof(DateTime), typeof(NewsDisguiseView), DateTime.Now);

    public DateTime CurrentTime
    {
        get => (DateTime)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    public NewsDisguiseView()
    {
        InitializeComponent();
        LoadNews();
        
        // 시간 업데이트
        var timer = Application.Current?.Dispatcher.CreateTimer();
        if (timer != null)
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            timer.Start();
        }
    }

    private void LoadNews()
    {
        NewsCollection.ItemsSource = DisguiseSampleData.GetSampleNews();
    }
}

