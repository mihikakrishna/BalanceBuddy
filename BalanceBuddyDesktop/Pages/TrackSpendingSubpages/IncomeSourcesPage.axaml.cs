using System;
using System.Globalization;
using System.Linq;
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

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            var incomeSource = (IncomeSource)textBox.DataContext;

            // Update the property based on which TextBox lost focus
            if (textBox.Name == "NameTextBox")
            {
                incomeSource.Name = textBox.Text;
            }
            else if (textBox.Name == "BalanceTextBox" && decimal.TryParse(textBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal newBalance))
            {
                incomeSource.Balance = newBalance;
            }
            else
            {
                // Handle parse error for BalanceTextBox
                return;
            }

            // Save changes to the UserData instance
            App.UserDataInstance.UpdateIncomeSource(incomeSource.Id, incomeSource.Name, incomeSource.Balance);
        }
    }


    private TextBox FindNameTextBox(Button saveButton)
    {
        var container = saveButton.Parent as StackPanel;
        return container.Children.OfType<TextBox>().First(t => t.Name == "NameTextBox");
    }

    private TextBox FindBalanceTextBox(Button saveButton)
    {
        var container = saveButton.Parent as StackPanel;
        return container.Children.OfType<TextBox>().First(t => t.Name == "BalanceTextBox");
    }

    private void AddIncomeSource_Click(object sender, RoutedEventArgs e)
    {
        App.UserDataInstance.AddIncomeSource("", 0);
    }


    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }
}