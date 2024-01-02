using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class ViewSpendingPage : UserControl, INavigable
{
    public event Action<UserControl>? RequestNavigate;

    public ViewSpendingPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new HomePage());
    }
}