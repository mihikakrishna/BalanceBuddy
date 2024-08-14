using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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


        public AddTransactionPageViewModel()
        {
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

        [RelayCommand]
        private void DeleteExpense(Expense expense)
        {
            if (_expenses.Contains(expense))
            {
                _expenses.Remove(expense);
                GlobalData.Instance.Expenses.Remove(expense);
            }
        }

        [RelayCommand]
        private void DeleteIncome(Income income)
        {
            if (_incomes.Contains(income))
            {
                _incomes.Remove(income);
                GlobalData.Instance.Incomes.Remove(income);
            }
        }

        [RelayCommand]
        private void DeleteSelectedExpenses()
        {
            var selectedExpenses = _expenses.Where(x => x.IsSelected).ToList();
            foreach (var expense in selectedExpenses)
            {
                DeleteExpense(expense);
            }
        }

        [RelayCommand]
        private void DeleteSelectedIncomes()
        {
            var selectedIncomes = _incomes.Where(x => x.IsSelected).ToList();
            foreach (var income in selectedIncomes)
            {
                DeleteIncome(income);
            }
        }

    }
}
