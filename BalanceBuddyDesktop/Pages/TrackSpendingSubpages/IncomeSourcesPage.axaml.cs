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

    private void SaveIncomeSource_Click(object sender, RoutedEventArgs e)
    {
        var saveButton = (Button)sender;
        var incomeSource = (IncomeSource)saveButton.DataContext;

        var nameTextBox = FindNameTextBox(saveButton);
        var balanceTextBox = FindBalanceTextBox(saveButton);


        var newName = nameTextBox.Text;
        incomeSource.Name = newName;

        if (decimal.TryParse(balanceTextBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal newBalance))
        {
            incomeSource.Balance = newBalance;
        }
        else
        {
            // handle parse error
            return;
        }

        if (incomeSource.Name.Equals(""))
        {
            App.UserDataInstance.RemoveIncomeSource(incomeSource);
            return;
        }

        App.UserDataInstance.UpdateIncomeSource(incomeSource.Id, incomeSource.Name, incomeSource.Balance);
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