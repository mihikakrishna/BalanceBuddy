using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace BalanceBuddyDesktop;

public partial class AccountsPage : UserControl, INavigable
{
    public event Action<UserControl>? RequestNavigate;

    public AccountsPage()
    {
        InitializeComponent();
        DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
    }

    private void SaveAccount_Click(object sender, RoutedEventArgs e)
    {
        var saveButton = (Button)sender;
        var account = (Account)saveButton.DataContext;

        var nameTextBox = FindNameTextBox(saveButton);
        var balanceTextBox = FindBalanceTextBox(saveButton);


        var newName = nameTextBox.Text;
        account.Name = newName;

        if (decimal.TryParse(balanceTextBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal newBalance))
        {
            account.Balance = newBalance;
        }
        else
        {
            // handle parse error
            return;
        }

        if (account.Name.Equals(""))
        {
            App.UserDataInstance.RemoveAccount(account);
            return;
        }

        App.UserDataInstance.UpdateAccount(account.Id, account.Name, account.Balance);
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

    private void AddAccount_Click(object sender, RoutedEventArgs e)
    {
        App.UserDataInstance.AddAccount("", 0);
    }


    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        RequestNavigate?.Invoke(new TrackSpendingPage());
    }
}