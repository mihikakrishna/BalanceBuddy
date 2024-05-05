using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BalanceBuddyDesktop
{
    public partial class EditExpensesPage : UserControl, INavigable
    {
        public event Action<UserControl>? RequestNavigate;

        public EditExpensesPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle back button click
        }

        private void ManageExpenseCategories_Click(object sender, RoutedEventArgs e)
        {
            RequestNavigate?.Invoke(new ManageExpenseCategoriesPage());
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            RequestNavigate?.Invoke(new AddExpensesPage());
        }
    }
}
