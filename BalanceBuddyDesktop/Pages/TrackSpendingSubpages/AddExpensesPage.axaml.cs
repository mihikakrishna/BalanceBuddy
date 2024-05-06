using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;

namespace BalanceBuddyDesktop
{
    public partial class AddExpensesPage : UserControl, INavigable
    {
        public event Action<UserControl>? RequestNavigate;
        public ObservableCollection<ExpenseCategory> Categories { get; set; } = new ObservableCollection<ExpenseCategory>
        {
            new ExpenseCategory("Food"),
            new ExpenseCategory("Transport"),
            new ExpenseCategory("Entertainment"),
            new ExpenseCategory("Rent"),
            new ExpenseCategory("Utilities")
        };

        public AddExpensesPage()
        {
            InitializeComponent();
            DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RequestNavigate?.Invoke(new ManageExpenseCategoriesPage());
        }

        private void ClearAmount_Click(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = "0";
        }

        private void SubmitExpense_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                // Handle parse error
                return;
            }

            var selectedCategory = CategoryComboBox.SelectedItem as ExpenseCategory ?? new ExpenseCategory("Unknown");
            var date = DateInput.SelectedDate ?? DateTime.Now;

            // Handle submission logic here, e.g., add to a list or database
        }
    }
}
