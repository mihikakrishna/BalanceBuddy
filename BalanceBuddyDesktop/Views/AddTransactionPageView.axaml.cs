using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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

        private async void ExportExpensesButton_Clicked(object sender, RoutedEventArgs args)
        {
            var viewModel = DataContext as AddTransactionPageViewModel;

            var topLevel = TopLevel.GetTopLevel(this);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Expenses as CSV",
                SuggestedFileName = "Expenses.csv",
                DefaultExtension = ".csv"
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);

                await streamWriter.WriteLineAsync("Date,Category,Amount,Description");

                foreach (var expense in viewModel.Expenses)
                {
                    var date = expense.Date.ToString("yyyy-MM-dd");
                    var category = expense.Category;
                    var amount = expense.Amount.ToString("F2");
                    var description = expense.Description.Replace(",", ";");

                    var line = $"{date},{category},{amount},{description}";
                    await streamWriter.WriteLineAsync(line);
                }
            }
        }

    }
}
