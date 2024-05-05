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
                    account.Name = textBox.Text;
                }
                else if (textBox.Name == "BalanceTextBox" && decimal.TryParse(textBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal newBalance))
                {
                    account.Balance = newBalance;
                }
                else
                {
                    // Handle parse error for BalanceTextBox
                    return;
                }

                // Save changes to the UserData instance
                App.UserDataInstance.UpdateAccount(account.Id, account.Name, account.Balance);
            }
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
}
