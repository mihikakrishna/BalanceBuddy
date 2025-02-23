using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BalanceBuddyDesktop.ViewModels
{
    public interface IRefreshable
    {
        void Refresh();
    }
    public partial class AddTransactionPageViewModel : ViewModelBase, INotifyPropertyChanged, IRefreshable
    {
        [ObservableProperty]
        private int _selectedTabIndex;

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
        private ObservableCollection<Expense> _expenses;

        [ObservableProperty]
        private ObservableCollection<Income> _incomes;

        [ObservableProperty]
        private ObservableCollection<BankAccount> _bankAccounts;

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

        private readonly DatabaseService _databaseService = DatabaseService.Instance;

        [ObservableProperty]
        private string _selectedMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");

        [ObservableProperty]
        private bool _expenseSortAscending = true;

        [ObservableProperty]
        private bool _incomeSortAscending = true;

        [ObservableProperty]
        private bool _bankAccountSortAscending = true;

        public List<string> Months { get; } = new List<string> {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        public AddTransactionPageViewModel()
        {
            Expenses = new ObservableCollection<Expense>(
                GlobalData.Instance.Expenses.OrderByDescending(e => e.Date));
            Incomes = new ObservableCollection<Income>(
                GlobalData.Instance.Incomes.OrderByDescending(i => i.Date));
            BankAccounts = new ObservableCollection<BankAccount>(
                GlobalData.Instance.BankAccounts);

            SubscribeToCollection(Expenses);
            SubscribeToCollection(Incomes);
            SubscribeToCollection(BankAccounts);

            Expenses.CollectionChanged += (s, e) => HandleCollectionChanged(e, Expenses);
            Incomes.CollectionChanged += (s, e) => HandleCollectionChanged(e, Incomes);
            BankAccounts.CollectionChanged += (s, e) => HandleCollectionChanged(e, BankAccounts);

            FilterExpensesByMonth();
            FilterIncomesByMonth();
        }

        private void SubscribeToCollection<T>(ObservableCollection<T> collection) where T : INotifyPropertyChanged
        {
            foreach (var item in collection)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void HandleCollectionChanged<T>(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, ObservableCollection<T> collection) where T : INotifyPropertyChanged
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<INotifyPropertyChanged>())
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<INotifyPropertyChanged>())
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GlobalData.Instance.HasUnsavedChanges = true;
        }


        [RelayCommand]
        private void AddExpense()
        {
            TransactionService.AddExpense(NewExpense);
            Expenses.Insert(0, NewExpense);
            NewExpense = new Expense();
        }

        [RelayCommand]
        private void AddIncome()
        {
            TransactionService.AddIncome(NewIncome);
            Incomes.Insert(0, NewIncome);
            NewIncome = new Income();
        }

        [RelayCommand]
        private void AddBankAccount()
        {
            TransactionService.AddBankAccount(NewBankAccount);
            BankAccounts.Insert(0, NewBankAccount);
            NewBankAccount = new BankAccount();
        }

        [RelayCommand]
        private void DeleteExpense(Expense expense)
        {
            TransactionService.DeleteExpense(expense);
            if (Expenses.Contains(expense))
            {
                Expenses.Remove(expense);
            }
        }

        [RelayCommand]
        private void DeleteIncome(Income income)
        {
            TransactionService.DeleteIncome(income);
            if (Incomes.Contains(income))
            {
                Incomes.Remove(income);
            }
        }

        [RelayCommand]
        private void DeleteBankAccount(BankAccount bankAccount)
        {
            TransactionService.DeleteBankAccount(bankAccount);
            if (BankAccounts.Contains(bankAccount))
            {
                BankAccounts.Remove(bankAccount);
            }
        }

        [RelayCommand]
        private void DeleteSelectedExpenses()
        {
            if (SelectedExpenses == null)
            {
                return;
            }

            foreach (var expense in SelectedExpenses.ToList())
            {
                TransactionService.DeleteExpense(expense);
                Expenses.Remove(expense);
            }
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void DeleteSelectedIncomes()
        {
            if (SelectedIncomes == null)
            {
                return;
            }

            foreach (var income in SelectedIncomes.ToList())
            {
                TransactionService.DeleteIncome(income);
                Incomes.Remove(income);
            }
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void DeleteSelectedBankAccounts()
        {
            if (SelectedBankAccounts == null)
            {
                return;
            }

            foreach (var bankAccount in SelectedBankAccounts.ToList())
            {
                TransactionService.DeleteBankAccount(bankAccount);
                BankAccounts.Remove(bankAccount);
            }
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        public void Refresh()
        {
            RefreshExpenses();
            RefreshIncomes();
            RefreshBankAccounts();
        }

        [RelayCommand]
        public void RefreshExpenses()
        {
            IEnumerable<Expense> refreshedExpenses = GlobalData.Instance.Expenses;

            if (!string.IsNullOrEmpty(SelectedMonth))
            {
                int month = DateTime.ParseExact(SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month;
                int currentYear = DateTime.Now.Year;
                refreshedExpenses = refreshedExpenses.Where(e => e.Date.Month == month && e.Date.Year == currentYear);
            }

            else if (SelectedExpenseDates?.Count > 0)
            {
                DateTime minDate = SelectedExpenseDates.Min();
                DateTime maxDate = SelectedExpenseDates.Max();
                refreshedExpenses = refreshedExpenses.Where(e => e.Date >= minDate && e.Date <= maxDate);
            }

            if (ExpenseSortAscending)
            {
                refreshedExpenses = refreshedExpenses.OrderBy(e => e.Date).ThenBy(e => e.Amount);
            }
            else
            {
                refreshedExpenses = refreshedExpenses.OrderByDescending(e => e.Date).ThenByDescending(e => e.Amount);
            }

            UpdateCollection(Expenses, refreshedExpenses);
        }


        [RelayCommand]
        public void RefreshIncomes()
        {
            IEnumerable<Income> refreshedIncomes = GlobalData.Instance.Incomes;

            if (!string.IsNullOrEmpty(SelectedMonth))
            {
                int month = DateTime.ParseExact(SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month;
                int currentYear = DateTime.Now.Year;
                refreshedIncomes = refreshedIncomes.Where(i => i.Date.Month == month && i.Date.Year == currentYear);
            }
            else if (SelectedIncomeDates?.Count > 0)
            {
                DateTime minDate = SelectedIncomeDates.Min();
                DateTime maxDate = SelectedIncomeDates.Max();
                refreshedIncomes = refreshedIncomes.Where(i => i.Date >= minDate && i.Date <= maxDate);
            }

            if (IncomeSortAscending)
            {
                refreshedIncomes = refreshedIncomes.OrderBy(i => i.Date).ThenBy(i => i.Amount);
            }
            else
            {
                refreshedIncomes = refreshedIncomes.OrderByDescending(i => i.Date).ThenByDescending(i => i.Amount);
            }

            UpdateCollection(Incomes, refreshedIncomes);
        }

        [RelayCommand]
        public void RefreshBankAccounts()
        {
            IEnumerable<BankAccount> refreshedBankAccounts = GlobalData.Instance.BankAccounts;

            if (BankAccountSortAscending)
            {
                refreshedBankAccounts = refreshedBankAccounts.OrderBy(b => b.Balance).ThenBy(b => b.Name);
            }
            else
            {
                refreshedBankAccounts = refreshedBankAccounts.OrderByDescending(b => b.Balance).ThenByDescending(b => b.Name);
            }

            UpdateCollection(BankAccounts, refreshedBankAccounts);
        }


        private void UpdateCollection<T>(ObservableCollection<T> collection, IEnumerable<T> newItems)
        {
            collection.Clear();
            foreach (var item in newItems)
            {
                collection.Add(item);
            }
        }

        partial void OnSelectedMonthChanged(string value)
        {
            FilterExpensesByMonth();
            FilterIncomesByMonth();
        }

        [RelayCommand]
        public void FilterExpenses()
        {
            if (SelectedExpenseDates.Count > 0)
            {
                DateTime minDate = SelectedExpenseDates.Min();
                DateTime maxDate = SelectedExpenseDates.Max();

                var filteredExpenses = GlobalData.Instance.Expenses
                    .Where(e => e.Date >= minDate && e.Date <= maxDate)
                    .OrderByDescending(e => e.Date)
                    .ThenByDescending(e => e.Amount);

                Expenses = new ObservableCollection<Expense>(filteredExpenses);
            }
            else
            {
                RefreshExpenses();
            }
        }

        [RelayCommand]
        public void FilterIncomes()
        {
            if (SelectedIncomeDates.Count > 0)
            {
                DateTime minDate = SelectedIncomeDates.Min();
                DateTime maxDate = SelectedIncomeDates.Max();

                var filteredIncomes = GlobalData.Instance.Incomes
                    .Where(i => i.Date >= minDate && i.Date <= maxDate)
                    .OrderByDescending(i => i.Date)
                    .ThenByDescending(e => e.Amount);

                Incomes = new ObservableCollection<Income>(filteredIncomes);
            }
            else
            {
                RefreshIncomes();
            }
        }

        [RelayCommand]
        public void FilterExpensesByMonth()
        {
            if (!string.IsNullOrEmpty(SelectedMonth))
            {
                int month = DateTime.ParseExact(SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month;
                int currentYear = DateTime.Now.Year;
                var filteredExpenses = GlobalData.Instance.Expenses
                    .Where(e => e.Date.Month == month && e.Date.Year == currentYear)
                    .OrderByDescending(e => e.Date);
                Expenses = new ObservableCollection<Expense>(filteredExpenses);
            }
            else
            {
                RefreshExpenses();
            }
        }


        [RelayCommand]
        public void FilterIncomesByMonth()
        {
            if (!string.IsNullOrEmpty(SelectedMonth))
            {
                int month = DateTime.ParseExact(SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month;
                int currentYear = DateTime.Now.Year;
                var filteredIncomes = GlobalData.Instance.Incomes
                    .Where(e => e.Date.Month == month && e.Date.Year == currentYear)
                    .OrderByDescending(e => e.Date);
                Incomes = new ObservableCollection<Income>(filteredIncomes);
            }
            else
            {
                RefreshIncomes();
            }
        }

        public ICommand UndoCommand => new RelayCommand(() =>
        {
            switch (SelectedTabIndex)
            {
                case 0:
                    UndoExpense();
                    break;
                case 1:
                    UndoIncome();
                    break;
                case 2:
                    UndoBankAccount();
                    break;
            }
        });

        public ICommand RedoCommand => new RelayCommand(() =>
        {
            switch (SelectedTabIndex)
            {
                case 0:
                    RedoExpense();
                    break;
                case 1:
                    RedoIncome();
                    break;
                case 2:
                    RedoBankAccount();
                    break;
            }
        });

        [RelayCommand]
        public void UndoExpense()
        {
            TransactionService.UndoExpense();
            RefreshExpenses();
        }

        [RelayCommand]
        public void RedoExpense()
        {
            TransactionService.RedoExpense();
            RefreshExpenses();
        }

        [RelayCommand]
        public void UndoIncome()
        {
            TransactionService.UndoIncome();
            RefreshIncomes();
        }

        [RelayCommand]
        public void RedoIncome()
        {
            TransactionService.RedoIncome();
            RefreshIncomes();
        }

        [RelayCommand]
        public void UndoBankAccount()
        {
            TransactionService.UndoBankAccount();
            RefreshBankAccounts();
        }

        [RelayCommand]
        public void RedoBankAccount()
        {
            TransactionService.RedoBankAccount();
            RefreshBankAccounts();
        }


        [RelayCommand]
        public void ClearFilters()
        {
            SelectedExpenseDates.Clear();
            SelectedIncomeDates.Clear();
            RefreshExpenses();
            RefreshIncomes();
        }
    }
}
