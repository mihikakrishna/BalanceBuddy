using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class TrackSpendingPage : UserControl, INavigable
{
    public event Action<UserControl>? RequestNavigate;

    public TrackSpendingPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new HomePage());
    }
}