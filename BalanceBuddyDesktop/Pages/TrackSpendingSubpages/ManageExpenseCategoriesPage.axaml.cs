using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Globalization;
using System.Linq;

namespace BalanceBuddyDesktop
{
    public partial class ManageExpenseCategoriesPage : UserControl, INavigable
    {
        public event Action<UserControl>? RequestNavigate;
        public ManageExpenseCategoriesPage()
        {
            InitializeComponent();
            DataContext = App.UserDataInstance ?? throw new InvalidOperationException("UserDataInstance is not initialized.");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RequestNavigate?.Invoke(new EditExpensesPage());
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var defaultCategoryName = "New Category";
            if (App.UserDataInstance.ExpenseCategories.Any(c => c.Name == defaultCategoryName))
            {
                return; // Don't add a duplicate category
            }
            App.UserDataInstance.AddExpenseCategory(defaultCategoryName, 0);
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ExpenseCategory category)
            {
                App.UserDataInstance.RemoveExpenseCategory(category);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is ExpenseCategory category)
            {
                if (textBox.Name == "NameTextBox")
                {
                    var newName = textBox.Text?.Trim();
                    if (string.IsNullOrWhiteSpace(newName) || App.UserDataInstance.ExpenseCategories.Any(c => c.Name == newName && c.Id != category.Id))
                    {
                        App.UserDataInstance.RemoveExpenseCategory(category);
                    }
                    else
                    {
                        App.UserDataInstance.UpdateExpenseCategory(category.Id, newName, category.Budget);
                    }
                }
                else if (textBox.Name == "BudgetTextBox" && decimal.TryParse(textBox.Text, NumberStyles.Currency, CultureInfo.InvariantCulture, out var newBudget))
                {
                    App.UserDataInstance.UpdateExpenseCategory(category.Id, category.Name, newBudget);
                }
            }
        }
    }
}
