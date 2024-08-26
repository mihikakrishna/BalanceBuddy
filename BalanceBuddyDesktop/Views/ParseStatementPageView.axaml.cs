using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace BalanceBuddyDesktop.Views;

public partial class ParseStatementPageView : UserControl
{
    // Define the CSV file type statically if this is within the proper lifecycle scope of Avalonia
    private static readonly FilePickerFileType CsvFileType = new FilePickerFileType("CSV Files (*.csv)")
    {
        Patterns = new[] { "*.csv" },
        MimeTypes = new[] { "text/csv" }
    };

    public ParseStatementPageView()
    {
        InitializeComponent();
    }

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open CSV File",
            AllowMultiple = false,
            FileTypeFilter = new[] { CsvFileType } 
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var fileContent = await streamReader.ReadToEndAsync();
            SelectedFileName.Text = files[0].Name;
            ParseFileButton.IsEnabled = true;
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
