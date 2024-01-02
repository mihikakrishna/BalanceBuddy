using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class IncomeSourcesPage : UserControl, INavigable
{
    public event Action<UserControl>? RequestNavigate;

    public IncomeSourcesPage()
    {
        InitializeComponent();
        DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
    }
    private void AddIncomeSource()
    {

        IncomeSourceNameTextBox.IsVisible = false;

        if (App.UserDataInstance == null)
        {
            throw new InvalidOperationException("UserDataInstance is not initialized.");
        }

        if (this.FindControl<TextBox>("IncomeSourceNameTextBox")?.Text is { } incomeSourceName && !string.IsNullOrWhiteSpace(incomeSourceName))
        {
            try
            {
                App.UserDataInstance!.AddIncomeSource(incomeSourceName, 10000);
                this.FindControl<TextBox>("IncomeSourceNameTextBox").Text = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    private void IncomeSourceNameTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && IncomeSourceNameTextBox.IsVisible)
        {
            AddIncomeSource();
        }
    }

    private void ShowAddIncomeSource_Click(object sender, RoutedEventArgs e)
    {
        IncomeSourceNameTextBox.IsVisible = true;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }
}