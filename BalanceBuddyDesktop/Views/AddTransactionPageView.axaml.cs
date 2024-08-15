using System;
using System.Linq;
using Avalonia.Controls;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.ViewModels;

namespace BalanceBuddyDesktop.Views
{
    public partial class AddTransactionPageView : UserControl
    {
        public AddTransactionPageView()
        {
            InitializeComponent();
            ExpenseDataGrid.SelectionChanged += ExpenseDataGrid_SelectionChanged;
            IncomeDataGrid.SelectionChanged += IncomeDataGrid_SelectionChanged;
            ExpenseFilterCalendar.SelectedDatesChanged += OnSelectedExpenseDatesChanged;
            IncomeFilterCalendar.SelectedDatesChanged += OnSelectedIncomeDatesChanged;
        }

        private void ExpenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is AddTransactionPageViewModel viewModel)
            {
                viewModel.SelectedExpenses = ExpenseDataGrid.SelectedItems.Cast<Expense>().ToList();
            }
        }

        private void IncomeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is AddTransactionPageViewModel viewModel)
            {
                viewModel.SelectedIncomes = IncomeDataGrid.SelectedItems.Cast<Income>().ToList();
            }
        }

        private void OnSelectedExpenseDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as AddTransactionPageViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedExpenseDates.Clear();

                foreach (DateTime date in ExpenseFilterCalendar.SelectedDates)
                {
                    viewModel.SelectedExpenseDates.Add(date);
                }
            }
        }

        private void OnSelectedIncomeDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as AddTransactionPageViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedIncomeDates.Clear();

                foreach (DateTime date in IncomeFilterCalendar.SelectedDates)
                {
                    viewModel.SelectedIncomeDates.Add(date);
                }
            }
        }
    }
}
