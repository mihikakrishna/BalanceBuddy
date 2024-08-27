using System;
using System.Collections.Generic;
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
    private List<Stream> currentFileStreams = new List<Stream>();
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
            Title = "Open CSV File(s)",
            AllowMultiple = true,
            FileTypeFilter = new[] { CsvFileType }
        });

        if (files.Count >= 1)
        {
            SelectedFiles.Text = string.Join(",", files.Select(file => file.Name));
            currentFileStreams.Clear();

            foreach (var file in files)
            {
                var fileStream = await file.OpenReadAsync();
                currentFileStreams.Add(fileStream); // Store each stream for later processing
            }

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
            SelectedFiles.Text = "No file selected";
        }
    }

    private void ParseFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        if (currentFileStreams.Any() && !string.IsNullOrEmpty(selectedBank))
        {
            try
            {
                IBankStatementParser parser = BankStatementParserFactory.GetParser(selectedBank);

                foreach (var fileStream in currentFileStreams)
                {
                    parser.ParseStatement(fileStream);
                    fileStream.Dispose(); // Dispose each stream after processing
                }

                MessageTextBlock.Foreground = Brushes.Green;
                MessageTextBlock.Text = "Files have been parsed successfully.";
            }
            catch (Exception ex)
            {
                MessageTextBlock.Foreground = Brushes.Red;
                MessageTextBlock.Text = $"Error parsing files: {ex.Message}";
            }
            finally
            {
                currentFileStreams.Clear();
                ParseFileButton.IsEnabled = false;
                RefreshUI();
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
        SelectedFiles.Text = "No file selected";
    }
}
