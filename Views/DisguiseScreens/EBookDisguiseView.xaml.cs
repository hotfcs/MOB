using MauiApp.Models;

namespace MauiApp.Views.DisguiseScreens;

public partial class EBookDisguiseView : ContentView
{
    public EBookDisguiseView()
    {
        InitializeComponent();
        LoadBook();
    }

    private void LoadBook()
    {
        var book = DisguiseSampleData.GetSampleEBook();
        
        BookTitleLabel.Text = book.Title;
        ChapterLabel.Text = book.Chapter;
        PageLabel.Text = book.Progress;
        ContentLabel.Text = book.Content;
        ProgressLabel.Text = $"{book.PageNumber} / {book.TotalPages} ({book.ProgressPercent:F0}%)";
        ProgressBar.Progress = book.ProgressPercent / 100.0;
    }
}

