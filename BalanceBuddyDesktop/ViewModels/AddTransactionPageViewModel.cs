using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        [ObservableProperty]
        private IList<Expense> _selectedExpenses;

        [ObservableProperty]
        private IList<Income> _selectedIncomes;

        [ObservableProperty]
        private ObservableCollection<DateTime> _selectedDates = new ObservableCollection<DateTime>();


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
            foreach (var expense in _selectedExpenses)
            {
                DeleteExpense(expense);
            }
        }

        [RelayCommand]
        private void DeleteSelectedIncomes()
        {
            foreach (var income in _selectedIncomes)
            {
                DeleteIncome(income);
            }
        }

        [RelayCommand]
        public void RefreshExpenses()
        {
            Expenses = new ObservableCollection<Expense>(GlobalData.Instance.Expenses);
        }

        [RelayCommand]
        public void RefreshIncomes()
        {

            Incomes = new ObservableCollection<Income>(GlobalData.Instance.Incomes);
        }

        [RelayCommand]
        public void FilterExpenses()
        {
            if (_selectedDates.Count > 0)
            {
                DateTime minDate = _selectedDates.Min();
                DateTime maxDate = _selectedDates.Max();

                Expenses = new ObservableCollection<Expense>(
                    GlobalData.Instance.Expenses.Where(expense => expense.Date >= minDate && expense.Date <= maxDate));
            }
            else
            {
                Expenses = new ObservableCollection<Expense>(GlobalData.Instance.Expenses);
            }
        }

    }
}
