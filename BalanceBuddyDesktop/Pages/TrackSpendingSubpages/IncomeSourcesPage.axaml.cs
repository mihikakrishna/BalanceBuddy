using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class IncomeSourcesPage : UserControl, INavigable
{
    public event Action<UserControl>? RequestNavigate;
    private readonly string defaultIncomeSourceName = "New Income Source";

    public IncomeSourcesPage()
    {
        InitializeComponent();
        DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            var incomeSource = textBox.DataContext as IncomeSource;

            // Update the property based on which TextBox lost focus
            if (textBox.Name == "NameTextBox")
            {
                var newName = textBox.Text?.Trim();

                // Check if the name is left blank or is a duplicate
                if (string.IsNullOrWhiteSpace(newName) || App.UserDataInstance.IncomeSources.Any(i => i.Name == newName && i.Id != incomeSource.Id))
                {
                    // Remove the income source if left blank or duplicate
                    App.UserDataInstance.RemoveIncomeSource(incomeSource);
                }
                else
                {
                    incomeSource.Name = newName;
                    App.UserDataInstance.UpdateIncomeSource(incomeSource.Id, incomeSource.Name, incomeSource.Balance);
                }
            }
            else if (textBox.Name == "BalanceTextBox" && decimal.TryParse(textBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal newBalance))
            {
                incomeSource.Balance = newBalance;
                App.UserDataInstance.UpdateIncomeSource(incomeSource.Id, incomeSource.Name, incomeSource.Balance);
            }
            else
            {
                // Handle parse error for BalanceTextBox
                return;
            }
        }
    }


    private void AddIncomeSource_Click(object sender, RoutedEventArgs e)
    {
        // Add income source with placeholder name
        if (App.UserDataInstance.IncomeSources.Any(a => a.Name == defaultIncomeSourceName))
            return;

        App.UserDataInstance.AddAccount(defaultIncomeSourceName, 0);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }
}