﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BalanceBuddyDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BalanceBuddyDesktop.ViewModels
{
    public partial class SettingsPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ExpenseCategory _newExpenseCategory = new ExpenseCategory();

        [ObservableProperty]
        private IncomeCategory _newIncomeCategory = new IncomeCategory();

        [ObservableProperty]
        private ObservableCollection<ExpenseCategory> _expenseCategories = new ObservableCollection<ExpenseCategory>(GlobalData.Instance.ExpenseCategories);

        [ObservableProperty]
        private ObservableCollection<IncomeCategory> _incomeCategories = new ObservableCollection<IncomeCategory>(GlobalData.Instance.IncomeCategories);

        [ObservableProperty]
        private IList<ExpenseCategory> _selectedExpenseCategories;

        [ObservableProperty]
        private IList<IncomeCategory> _selectedIncomeCategories;


        public SettingsPageViewModel()
        {
        }

        [RelayCommand]
        private void AddExpenseCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewExpenseCategory.Name))
            {
                GlobalData.Instance.ExpenseCategories.Add(NewExpenseCategory);
                ExpenseCategories.Add(NewExpenseCategory);
                NewExpenseCategory = new ExpenseCategory();
            }
        }

        [RelayCommand]
        private void AddIncomeCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewIncomeCategory.Name))
            {
                GlobalData.Instance.IncomeCategories.Add(NewIncomeCategory);
                IncomeCategories.Add(NewIncomeCategory);
                NewIncomeCategory = new IncomeCategory();
            }
        }


        [RelayCommand]
        private void DeleteExpenseCategory(ExpenseCategory expenseCategory)
        {
            if (ExpenseCategories.Contains(expenseCategory))
            {
                ExpenseCategories.Remove(expenseCategory);
                GlobalData.Instance.ExpenseCategories.Remove(expenseCategory);
            }
        }

        [RelayCommand]
        private void DeleteIncomeCategory(IncomeCategory incomeCategory)
        {
            if (IncomeCategories.Contains(incomeCategory))
            {
                IncomeCategories.Remove(incomeCategory);
                GlobalData.Instance.IncomeCategories.Remove(incomeCategory);
            }
        }

        [RelayCommand]
        private void DeleteSelectedExpenseCategories()
        {
            if (SelectedExpenseCategories == null)
            { return; }

            foreach (var expenseCategory in SelectedExpenseCategories.ToList())
            {
                DeleteExpenseCategory(expenseCategory);
            }
        }

        [RelayCommand]
        private void DeleteSelectedIncomeCategories()
        {
            if (SelectedIncomeCategories == null)
            { return; }

            foreach (var incomeCategory in SelectedIncomeCategories.ToList())
            {
                DeleteIncomeCategory(incomeCategory);
            }
        }

        [RelayCommand]
        public void RefreshExpenseCategories()
        {
            ExpenseCategories = new ObservableCollection<ExpenseCategory>(GlobalData.Instance.ExpenseCategories);
        }

        [RelayCommand]
        public void RefreshIncomeCategories()
        {

            IncomeCategories = new ObservableCollection<IncomeCategory>(GlobalData.Instance.IncomeCategories);
        }
    }
}
