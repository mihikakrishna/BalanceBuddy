using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
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
            try
            {
                IBankStatementParser parser = BankStatementParserFactory.GetParser(selectedBank);
                parser.ParseStatement(currentFileStream);

                MessageTextBlock.Foreground = Brushes.Green;
                MessageTextBlock.Text = "File has been parsed successfully.";

                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageTextBlock.Foreground = Brushes.Red;
                MessageTextBlock.Text = $"Error parsing file: {ex.Message}";
            }
            finally
            {
                currentFileStream?.Dispose();
                currentFileStream = null;
                ParseFileButton.IsEnabled = false;
            }
        }
        else
        {
            MessageTextBlock.Foreground = Brushes.Red;
            MessageTextBlock.Text = "No file selected or bank not specified.";
        }
    }

    private void RefreshUI()
    {
        SelectedFileName.Text = "No file selected";
    }
}
