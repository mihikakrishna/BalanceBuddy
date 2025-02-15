using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.ViewModels;

namespace BalanceBuddyDesktop.Views
{
    public partial class AddTransactionPageView : UserControl
    {
        private bool _expenseSortAscending = true;
        private bool _incomeSortAscending = true;
        private bool _bankAccountSortAscending = true;

        public AddTransactionPageView()
        {
            InitializeComponent();
            ExpenseDataGrid.SelectionChanged += ExpenseDataGrid_SelectionChanged;
            IncomeDataGrid.SelectionChanged += IncomeDataGrid_SelectionChanged;
            BankAccountDataGrid.SelectionChanged += BankAccountDataGrid_SelectionChanged;
            ExpenseFilterCalendar.SelectedDatesChanged += OnSelectedExpenseDatesChanged;
            IncomeFilterCalendar.SelectedDatesChanged += OnSelectedIncomeDatesChanged;

            ExpenseDataGrid.Sorting += ExpenseDataGrid_Sorting;
            IncomeDataGrid.Sorting += IncomeDataGrid_Sorting;
            BankAccountDataGrid.Sorting += BankAccountDataGrid_Sorting;
        }

        private void HandleSorting<T>(
                DataGridColumnEventArgs e,
                ObservableCollection<T> currentCollection,
                Action<ObservableCollection<T>> updateCollection,
                ref bool sortAscending)
        {
            e.Handled = true;
            string sortMember = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(sortMember))
            {
                sortMember = e.Column.Header?.ToString();
            }
            if (string.IsNullOrEmpty(sortMember))
            {
                return;
            }
            sortAscending = !sortAscending;
            updateCollection(SortCollection(currentCollection, sortMember, sortAscending));
        }

        private ObservableCollection<T> SortCollection<T>(
            ObservableCollection<T> collection, string propertyName, bool ascending)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);
            if (prop == null)
            {
                return new ObservableCollection<T>(collection);
            }
            var sorted = ascending
                ? collection.OrderBy(item => prop.GetValue(item))
                : collection.OrderByDescending(item => prop.GetValue(item));
            return new ObservableCollection<T>(sorted);
        }

        private void ExpenseDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (DataContext is AddTransactionPageViewModel viewModel)
            {
                HandleSorting<Expense>(e, viewModel.Expenses, sorted => viewModel.Expenses = sorted, ref _expenseSortAscending);
            }
        }

        private void IncomeDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (DataContext is AddTransactionPageViewModel viewModel)
            {
                HandleSorting<Income>(e, viewModel.Incomes, sorted => viewModel.Incomes = sorted, ref _incomeSortAscending);
            }
        }

        private void BankAccountDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (DataContext is AddTransactionPageViewModel viewModel)
            {
                HandleSorting<BankAccount>(e, viewModel.BankAccounts, sorted => viewModel.BankAccounts = sorted, ref _bankAccountSortAscending);
            }
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

        private void BankAccountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is AddTransactionPageViewModel viewModel)
            {
                viewModel.SelectedBankAccounts = BankAccountDataGrid.SelectedItems.Cast<BankAccount>().ToList();
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
                    var description = "";
                    if (expense.Description != null)
                        expense.Description.Replace(",", ";");

                    var line = $"{date},{category},{amount},{description}";
                    await streamWriter.WriteLineAsync(line);
                }
            }
        }

        private async void ExportIncomesButton_Clicked(object sender, RoutedEventArgs args)
        {
            var viewModel = DataContext as AddTransactionPageViewModel;

            var topLevel = TopLevel.GetTopLevel(this);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Incomes as CSV",
                SuggestedFileName = "Incomes.csv",
                DefaultExtension = ".csv"
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);

                await streamWriter.WriteLineAsync("Date,Category,Amount,Description");

                foreach (var income in viewModel.Incomes)
                {
                    var date = income.Date.ToString("yyyy-MM-dd");
                    var category = income.Category;
                    var amount = income.Amount.ToString("F2");
                    var description = "";
                    if (income.Description != null)
                        description = income.Description.Replace(",", ";");

                    var line = $"{date},{category},{amount},{description}";
                    await streamWriter.WriteLineAsync(line);
                }
            }
        }

        private async void ExportBankAccountsButton_Clicked(object sender, RoutedEventArgs args)
        {
            var viewModel = DataContext as AddTransactionPageViewModel;

            var topLevel = TopLevel.GetTopLevel(this);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Bank Accounts as CSV",
                SuggestedFileName = "BankAccounts.csv",
                DefaultExtension = ".csv"
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);

                await streamWriter.WriteLineAsync("Name,Balance,Description");

                foreach (var bankAccount in viewModel.BankAccounts)
                {
                    var name = bankAccount.Name;
                    var balance = bankAccount.Balance.ToString("F2");
                    var description = "";
                    if (bankAccount.Description != null)
                        description = bankAccount.Description.Replace(",", ";");

                    var line = $"{name},{balance},{description}";
                    await streamWriter.WriteLineAsync(line);
                }
            }
        }
        private void DeleteExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Expense expense)
            {
                if (DataContext is AddTransactionPageViewModel viewModel)
                {
                    if (viewModel.SelectedExpenses == null)
                        viewModel.SelectedExpenses = new List<Expense>();
                    if (!viewModel.SelectedExpenses.Contains(expense))
                        viewModel.SelectedExpenses.Add(expense);
                    if (viewModel.DeleteSelectedExpensesCommand.CanExecute(null))
                        viewModel.DeleteSelectedExpensesCommand.Execute(null);
                }
            }
        }
        private void DeleteIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Income income)
            {
                if (DataContext is AddTransactionPageViewModel viewModel)
                {
                    if (viewModel.SelectedIncomes == null)
                        viewModel.SelectedIncomes = new List<Income>();
                    if (!viewModel.SelectedIncomes.Contains(income))
                        viewModel.SelectedIncomes.Add(income);
                    if (viewModel.DeleteSelectedIncomesCommand.CanExecute(null))
                        viewModel.DeleteSelectedIncomesCommand.Execute(null);
                }
            }
        }
        private void DeleteBankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BankAccount bankAccount)
            {
                if (DataContext is AddTransactionPageViewModel viewModel)
                {
                    if (viewModel.SelectedBankAccounts == null)
                        viewModel.SelectedBankAccounts = new List<BankAccount>();
                    if (!viewModel.SelectedBankAccounts.Contains(bankAccount))
                        viewModel.SelectedBankAccounts.Add(bankAccount);
                    if (viewModel.DeleteSelectedBankAccountsCommand.CanExecute(null))
                        viewModel.DeleteSelectedBankAccountsCommand.Execute(null);
                }
            }
        }
    }
}
