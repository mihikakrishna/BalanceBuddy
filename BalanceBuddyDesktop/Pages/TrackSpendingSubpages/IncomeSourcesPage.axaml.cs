using System;
using System.Diagnostics;
using Avalonia.Controls;
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
    private void AddIncomeSource_Click(object sender, RoutedEventArgs e)
    {

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


    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }
}