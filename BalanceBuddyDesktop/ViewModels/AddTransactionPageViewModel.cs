﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

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
            GlobalData.Instance.Expenses.Add(_newExpense);
            Expenses.Insert(0, _newExpense);
            NewExpense = new Expense();
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void AddIncome()
        {
            GlobalData.Instance.Incomes.Add(_newIncome);
            Incomes.Insert(0, _newIncome);
            NewIncome = new Income();
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void AddBankAccount()
        {
            GlobalData.Instance.BankAccounts.Add(_newBankAccount);
            BankAccounts.Insert(0, _newBankAccount);
            NewBankAccount = new BankAccount();
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        public async void SaveAll()
        {
            _databaseService.SaveUserData(GlobalData.Instance);
            GlobalData.Instance.HasUnsavedChanges = false;
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = "Save All",
                ContentMessage = "Your changes have been saved successfully!",
                Icon = Icon.Success,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });
            var result = await messageBoxStandardWindow.ShowAsync();
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
            if (SelectedExpenses == null)
            {
                return;
            }

            foreach (var expense in SelectedExpenses.ToList())
            {
                DeleteExpense(expense);
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
                DeleteIncome(income);
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
                DeleteBankAccount(bankAccount);
            }
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        public void RefreshExpenses()
        {
            Expenses = new ObservableCollection<Expense>(
                GlobalData.Instance.Expenses.OrderByDescending(e => e.Date));
        }

        [RelayCommand]
        public void RefreshIncomes()
        {
            Incomes = new ObservableCollection<Income>(
                GlobalData.Instance.Incomes.OrderByDescending(i => i.Date));
        }

        [RelayCommand]
        public void RefreshBankAccounts()
        {
            BankAccounts = new ObservableCollection<BankAccount>(
                GlobalData.Instance.BankAccounts);
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
                    .OrderByDescending(e => e.Date);

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
                    .OrderByDescending(i => i.Date);

                Incomes = new ObservableCollection<Income>(filteredIncomes);
            }
            else
            {
                RefreshIncomes();
            }
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
