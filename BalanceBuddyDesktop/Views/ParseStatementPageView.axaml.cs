using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace BalanceBuddyDesktop.Views;

public partial class ParseStatementPageView : UserControl
{
    public ParseStatementPageView()
    {
        InitializeComponent();
    }

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var fileContent = await streamReader.ReadToEndAsync();
            SelectedFileName.Text = files[0].Name; // Update the label with the selected file's name
        }
        else
        {
            SelectedFileName.Text = "No file selected";
        }
    }

    private void ParseFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        // Your logic to parse the selected file goes here
    }
}