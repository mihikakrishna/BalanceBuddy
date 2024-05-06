using Avalonia.Controls;
using Avalonia.Interactivity;
using BalanceBuddyDesktop;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace BalanceBuddyDesktop
{
    public partial class AddExpensesPage : UserControl, INavigable
    {
        public event Action<UserControl>? RequestNavigate;
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();

        public AddExpensesPage()
        {
            InitializeComponent();
            DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RequestNavigate?.Invoke(new EditExpensesPage());
        }

        private void ClearAmount_Click(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = "0";
        }

        private void SubmitExpense_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal amount))
            {
                // Handle parse error
                return;
            }

            var selectedCategory = CategoryComboBox.SelectedItem as ExpenseCategory ?? new ExpenseCategory("Unknown");
            var date = DateInput.SelectedDate?.DateTime ?? DateTime.Now;

            var newExpense = new Expense(amount, selectedCategory, date);
            Expenses.Add(newExpense);

            // Optionally, clear the form
            ClearAmount_Click(sender, e);
            CategoryComboBox.SelectedIndex = 0;
            DateInput.SelectedDate = DateTime.Now;
        }
    }
}
