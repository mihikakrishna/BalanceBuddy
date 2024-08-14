using System.Linq;
using Avalonia.Controls;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.ViewModels;

namespace BalanceBuddyDesktop.Views
{
    public partial class SettingsPageView : UserControl
    {
        public SettingsPageView()
        {
            InitializeComponent();
            ExpenseDataGrid.SelectionChanged += ExpenseDataGrid_SelectionChanged;
            IncomeDataGrid.SelectionChanged += IncomeDataGrid_SelectionChanged;
        }

        private void ExpenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is SettingsPageViewModel viewModel)
            {
                viewModel.SelectedExpenseCategories = ExpenseDataGrid.SelectedItems.Cast<ExpenseCategory>().ToList();
            }
        }

        private void IncomeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is SettingsPageViewModel viewModel)
            {
                viewModel.SelectedIncomeCategories = IncomeDataGrid.SelectedItems.Cast<IncomeCategory>().ToList();
            }
        }
    }

}
