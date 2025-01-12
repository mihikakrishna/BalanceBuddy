using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

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

        private readonly DatabaseService _databaseService = DatabaseService.Instance;

        public SettingsPageViewModel()
        {
            SubscribeToCollection(ExpenseCategories);
            SubscribeToCollection(IncomeCategories);

            ExpenseCategories.CollectionChanged += (s, e) => HandleCollectionChanged(e, ExpenseCategories);
            IncomeCategories.CollectionChanged += (s, e) => HandleCollectionChanged(e, IncomeCategories);
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
        private void AddExpenseCategory()
        {
            if (string.IsNullOrWhiteSpace(NewExpenseCategory.Name))
            {
                return;
            }

            if (NewExpenseCategory.Budget.HasValue && NewExpenseCategory.Budget < 0)
            {
                return;
            }

            GlobalData.Instance.ExpenseCategories.Add(NewExpenseCategory);
            ExpenseCategories.Add(NewExpenseCategory);
            NewExpenseCategory = new ExpenseCategory();
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void AddIncomeCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewIncomeCategory.Name))
            {
                GlobalData.Instance.IncomeCategories.Add(NewIncomeCategory);
                IncomeCategories.Add(NewIncomeCategory);
                NewIncomeCategory = new IncomeCategory();
                GlobalData.Instance.HasUnsavedChanges = true;
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
            GlobalData.Instance.HasUnsavedChanges = true;
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
            GlobalData.Instance.HasUnsavedChanges = true;
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
