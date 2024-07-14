﻿using System;
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
    public partial class AddExpensePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private Expense _newExpense = new Expense();

        [ObservableProperty]
        private List<ExpenseCategory> _categories = GlobalData.Instance.ExpenseCategories;

        [ObservableProperty]
        private ObservableCollection<Expense> _expenses = new ObservableCollection<Expense>(GlobalData.Instance.Expenses);

        public FlatTreeDataGridSource<Expense> Source { get; }

        public AddExpensePageViewModel()
        {
            Source = new FlatTreeDataGridSource<Expense>(_expenses)
            {
                Columns =
                {
                    new TextColumn<Expense, decimal>("Amount", x => x.Amount),
                    new TextColumn<Expense, string>("Date", x => x.FormattedDate),
                    new TextColumn<Expense, string>("Category", x => x.Category.Name),
                    new TextColumn<Expense, string>("Description", x => x.Description), 
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

    }
}
