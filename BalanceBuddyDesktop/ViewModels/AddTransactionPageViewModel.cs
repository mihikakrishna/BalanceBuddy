using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using BalanceBuddyDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BalanceBuddyDesktop.ViewModels
{
    public partial class AddTransactionPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private Expense _newExpense = new Expense();

        [ObservableProperty]
        private Income _newIncome = new Income();

        [ObservableProperty]
        private List<ExpenseCategory> _expenseCategories = GlobalData.Instance.ExpenseCategories;

        [ObservableProperty]
        private List<IncomeCategory> _incomeCategories = GlobalData.Instance.IncomeCategories;

        [ObservableProperty]
        private ObservableCollection<Expense> _expenses = new ObservableCollection<Expense>(GlobalData.Instance.Expenses);

        [ObservableProperty]
        private ObservableCollection<Income> _incomes = new ObservableCollection<Income>(GlobalData.Instance.Incomes);

        public FlatTreeDataGridSource<Expense> ExpenseDataGridSource { get; }

        public FlatTreeDataGridSource<Income> IncomeDataGridSource { get; }

        public AddTransactionPageViewModel()
        {
            ExpenseDataGridSource = new FlatTreeDataGridSource<Expense>(_expenses)
            {
                Columns =
                {
                    new TextColumn<Expense, decimal>("Amount", x => x.Amount),
                    new TextColumn<Expense, string>("Date", x => x.FormattedDate),
                    new TextColumn<Expense, string>("Category", x => x.Category.Name),
                    new TextColumn<Expense, string>("Description", x => x.Description), 
                }
            };

            IncomeDataGridSource = new FlatTreeDataGridSource<Income>(_incomes)
            {
                Columns =
                {
                    new TextColumn<Income, decimal>("Amount", x => x.Amount),
                    new TextColumn<Income, string>("Date", x => x.FormattedDate),
                    new TextColumn<Income, string>("Category", x => x.Category.Name),
                    new TextColumn<Income, string>("Description", x => x.Description),
                }
            };
        }

        [RelayCommand]
        private void AddExpense()
        {
            GlobalData.Instance.Expenses.Add(_newExpense);

            _expenses.Add(_newExpense);
            OnPropertyChanged(nameof(_expenses));

            NewExpense = new Expense();
        }

        [RelayCommand]
        private void AddIncome()
        {
            GlobalData.Instance.Incomes.Add(_newIncome);

            _incomes.Add(_newIncome);
            OnPropertyChanged(nameof(_incomes));

            NewIncome = new Income();
        }

    }
}
