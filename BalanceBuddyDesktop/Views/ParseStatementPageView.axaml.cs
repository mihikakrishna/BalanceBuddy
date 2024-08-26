using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Parsers;

namespace BalanceBuddyDesktop.Views;

public partial class ParseStatementPageView : UserControl
{
    private Stream currentFileStream;
    private string selectedBank;

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
            currentFileStream = await files[0].OpenReadAsync(); // Store stream for later processing
            SelectedFileName.Text = files[0].Name;
            ParseFileButton.IsEnabled = true;

            var comboBoxItem = BankComboBox.SelectedItem as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var stackPanel = comboBoxItem.Content as StackPanel;
                var textBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault();
                selectedBank = textBlock?.Text.Trim();
            }
        }
        else
        {
            SelectedFileName.Text = "No file selected";
        }
    }

    private void ParseFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        if (currentFileStream != null && !string.IsNullOrEmpty(selectedBank))
        {
            // Create an instance of UserData or get it from somewhere if it needs to be shared
            UserData userData = new UserData();

            // Get the appropriate parser based on the selected bank
            IBankStatementParser parser = BankStatementParserFactory.GetParser(selectedBank);
            parser.ParseStatement(currentFileStream);

            // Logic after parsing: Possibly refresh UI, show success message, etc.
            Debug.WriteLine("File has been parsed successfully.");

            // Close and dispose the file stream
            currentFileStream.Dispose();
            currentFileStream = null;
        }
    }
}
