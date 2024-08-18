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
        private BankAccount _newBankAccount = new BankAccount();

        [ObservableProperty]
        private List<ExpenseCategory> _expenseCategories = GlobalData.Instance.ExpenseCategories;

        [ObservableProperty]
        private List<IncomeCategory> _incomeCategories = GlobalData.Instance.IncomeCategories;

        [ObservableProperty]
        private ObservableCollection<Expense> _expenses = new ObservableCollection<Expense>(GlobalData.Instance.Expenses);

        [ObservableProperty]
        private ObservableCollection<Income> _incomes = new ObservableCollection<Income>(GlobalData.Instance.Incomes);

        [ObservableProperty]
        private ObservableCollection<BankAccount> _bankAccounts = new ObservableCollection<BankAccount>(GlobalData.Instance.BankAccounts);

        [ObservableProperty]
        private IList<Expense> _selectedExpenses;

        [ObservableProperty]
        private IList<Income> _selectedIncomes;

        [ObservableProperty]
        private IList<BankAccount> _selectedBankAccounts;

        [ObservableProperty]
        private ObservableCollection<DateTime> _selectedExpenseDates = new ObservableCollection<DateTime>();

        [ObservableProperty]
        private ObservableCollection<DateTime> _selectedIncomeDates = new ObservableCollection<DateTime>();


        public AddTransactionPageViewModel()
        {
        }

        [RelayCommand]
        private void AddExpense()
        {
            GlobalData.Instance.Expenses.Add(_newExpense);
            Expenses.Add(_newExpense);
            RefreshExpenses();
            NewExpense = new Expense();
        }

        [RelayCommand]
        private void AddIncome()
        {
            GlobalData.Instance.Incomes.Add(_newIncome);
            _incomes.Add(_newIncome);
            RefreshIncomes();
            NewIncome = new Income();
        }

        [RelayCommand]
        private void AddBankAccount()
        {
            GlobalData.Instance.BankAccounts.Add(_newBankAccount);
            _bankAccounts.Add(_newBankAccount);
            RefreshBankAccounts();
            NewBankAccount = new BankAccount();
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
        private void DeleteBankAccount(BankAccount bankAccount)
        {
            if (_bankAccounts.Contains(bankAccount))
            {
                _bankAccounts.Remove(bankAccount);
                GlobalData.Instance.BankAccounts.Remove(bankAccount);
            }
        }

        [RelayCommand]
        private void DeleteSelectedExpenses()
        {
            foreach (var expense in _selectedExpenses)
            {
                DeleteExpense(expense);
            }

            RefreshExpenses();
        }

        [RelayCommand]
        private void DeleteSelectedIncomes()
        {
            foreach (var income in _selectedIncomes)
            {
                DeleteIncome(income);
            }

            RefreshIncomes();
        }

        [RelayCommand]
        private void DeleteSelectedBankAccounts()
        {
            foreach (var bankAccount in _selectedBankAccounts)
            {
                DeleteBankAccount(bankAccount);
            }

            RefreshBankAccounts();
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
        public void RefreshBankAccounts()
        {
            BankAccounts = new ObservableCollection<BankAccount>(GlobalData.Instance.BankAccounts);
        }

        [RelayCommand]
        public void FilterExpenses()
        {
            if (SelectedExpenseDates.Count > 0)
            {
                DateTime minDate = SelectedExpenseDates.Min();
                DateTime maxDate = SelectedExpenseDates.Max();

                Expenses = new ObservableCollection<Expense>(
                    GlobalData.Instance.Expenses.Where(expense => expense.Date >= minDate && expense.Date <= maxDate));
            }
            else
            {
                Expenses = new ObservableCollection<Expense>(GlobalData.Instance.Expenses);
            }
        }

        [RelayCommand]
        public void FilterIncomes()
        {
            if (SelectedIncomeDates.Count > 0)
            {
                DateTime minDate = SelectedIncomeDates.Min();
                DateTime maxDate = SelectedIncomeDates.Max();

                Incomes = new ObservableCollection<Income>(
                    GlobalData.Instance.Incomes.Where(income => income.Date >= minDate && income.Date <= maxDate));
            }
            else
            {
                Incomes = new ObservableCollection<Income>(GlobalData.Instance.Incomes);
            }
        }

        [RelayCommand]
        public void ClearFilters()
        {
            SelectedExpenseDates.Clear();
            SelectedIncomeDates.Clear();
            FilterExpenses();
            FilterIncomes();
        }
    }
}
