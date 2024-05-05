using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BalanceBuddyDesktop
{
    public partial class AccountsPage : UserControl, INavigable
    {
        public event Action<UserControl>? RequestNavigate;
        private string defaultAccountName = "New Account";

        public AccountsPage()
        {
            InitializeComponent();
            DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var account = textBox.DataContext as Account;

                // Update the property based on which TextBox lost focus
                if (textBox.Name == "NameTextBox")
                {
                    var newName = textBox.Text?.Trim();

                    // Check if the name is left blank or is a duplicate
                    if (string.IsNullOrWhiteSpace(newName) || newName.Equals("New Account") || App.UserDataInstance.Accounts.Any(a => a.Name == newName && a.Id != account.Id))
                    {
                        // Remove the account if left blank or duplicate
                        App.UserDataInstance.RemoveAccount(account);
                    }
                    else
                    {
                        account.Name = newName;
                        App.UserDataInstance.UpdateAccount(account.Id, account.Name, account.Balance);
                    }
                }
                else if (textBox.Name == "BalanceTextBox" && decimal.TryParse(textBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal newBalance))
                {
                    account.Balance = newBalance;
                    App.UserDataInstance.UpdateAccount(account.Id, account.Name, account.Balance);
                }
                else
                {
                    // Handle parse error for BalanceTextBox
                    return;
                }
            }
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            // Add account with placeholder name
            if (App.UserDataInstance.Accounts.Any(a => a.Name == defaultAccountName))
                return;

            App.UserDataInstance.AddAccount(defaultAccountName, 0);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RequestNavigate?.Invoke(new TrackSpendingPage());
        }
    }
}
