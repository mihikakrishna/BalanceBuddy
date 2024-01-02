using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class HomePage : UserControl
{
    public event Action<UserControl>? RequestNavigate;

    public HomePage()
    {
        InitializeComponent();
    }

    private void TrackSpending_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }


    private void ViewSpending_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new ViewSpendingPage());
    }

    private void NavigateTo(UserControl newPage)
    {
        if (this.Parent is MainWindow mainWindow)
        {
            mainWindow.MainContent.Content = newPage;
        }
    }
}
