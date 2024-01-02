using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class AddIncomeSourcePage : UserControl, INavigable
{
    public event Action<UserControl>? RequestNavigate;

    public AddIncomeSourcePage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }
}