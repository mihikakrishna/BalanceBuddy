using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            NewExpense = new Expense();
        }

        [RelayCommand]
        private void AddIncome()
        {
            GlobalData.Instance.Incomes.Add(_newIncome);
            Incomes.Add(_newIncome);
            NewIncome = new Income();
        }

        [RelayCommand]
        private void AddBankAccount()
        {
            GlobalData.Instance.BankAccounts.Add(_newBankAccount);
            BankAccounts.Add(_newBankAccount);
            NewBankAccount = new BankAccount();
        }

        [RelayCommand]
        private void DeleteExpense(Expense expense)
        {
            if (Expenses.Contains(expense))
            {
                Expenses.Remove(expense);
                GlobalData.Instance.Expenses.Remove(expense);
            }
        }

        [RelayCommand]
        private void DeleteIncome(Income income)
        {
            if (Incomes.Contains(income))
            {
                Incomes.Remove(income);
                GlobalData.Instance.Incomes.Remove(income);
            }
        }

        [RelayCommand]
        private void DeleteBankAccount(BankAccount bankAccount)
        {
            if (BankAccounts.Contains(bankAccount))
            {
                BankAccounts.Remove(bankAccount);
                GlobalData.Instance.BankAccounts.Remove(bankAccount);
            }
        }

        [RelayCommand]
        private void DeleteSelectedExpenses()
        {
            foreach (var expense in SelectedExpenses.ToList())
            {
                DeleteExpense(expense);
            }
        }

        [RelayCommand]
        private void DeleteSelectedIncomes()
        {
            foreach (var income in SelectedIncomes.ToList())
            {
                DeleteIncome(income);
            }
        }

        [RelayCommand]
        private void DeleteSelectedBankAccounts()
        {
            foreach (var bankAccount in SelectedBankAccounts.ToList())
            {
                DeleteBankAccount(bankAccount);
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

                Expenses.Clear();
                foreach (var expense in GlobalData.Instance.Expenses.Where(e => e.Date >= minDate && e.Date <= maxDate))
                {
                    Expenses.Add(expense);
                }
            }
            else
            {
                Expenses.Clear();
                foreach (var expense in GlobalData.Instance.Expenses)
                {
                    Expenses.Add(expense);
                }
            }
        }

        [RelayCommand]
        public void FilterIncomes()
        {
            if (SelectedIncomeDates.Count > 0)
            {
                DateTime minDate = SelectedIncomeDates.Min();
                DateTime maxDate = SelectedIncomeDates.Max();

                Incomes.Clear();
                foreach (var income in GlobalData.Instance.Incomes.Where(i => i.Date >= minDate && i.Date <= maxDate))
                {
                    Incomes.Add(income);
                }
            }
            else
            {
                Incomes.Clear();
                foreach (var income in GlobalData.Instance.Incomes)
                {
                    Incomes.Add(income);
                }
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
