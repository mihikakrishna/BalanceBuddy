using System.Collections.Generic;
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
        private List<ExpenseCategory> _expenseCategories = GlobalData.Instance.ExpenseCategories;

        [ObservableProperty]
        private List<IncomeCategory> _incomeCategories = GlobalData.Instance.IncomeCategories;


        public SettingsPageViewModel()
        {
        }

        [RelayCommand]
        private void AddExpenseCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewExpenseCategory.Name))
            {
                GlobalData.Instance.ExpenseCategories.Add(NewExpenseCategory);
                NewExpenseCategory = new ExpenseCategory();
            }
        }

        [RelayCommand]
        private void AddIncomeCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewIncomeCategory.Name))
            {
                GlobalData.Instance.IncomeCategories.Add(NewIncomeCategory);
                NewIncomeCategory = new IncomeCategory();
            }
        }


        [RelayCommand]
        private void DeleteExpenseCategory(ExpenseCategory expenseCategory)
        {
            if (_expenseCategories.Contains(expenseCategory))
            {
                _expenseCategories.Remove(expenseCategory);
                GlobalData.Instance.ExpenseCategories.Remove(expenseCategory);
            }
        }

        [RelayCommand]
        private void DeleteIncomeCategory(IncomeCategory incomeCategory)
        {
            if (_incomeCategories.Contains(incomeCategory))
            {
                _incomeCategories.Remove(incomeCategory);
                GlobalData.Instance.IncomeCategories.Remove(incomeCategory);
            }
        }

        [RelayCommand]
        private void DeleteSelectedExpenseCategories()
        {
            var selectedExpenseCategories = _expenseCategories.Where(x => x.IsSelected).ToList();
            foreach (var expenseCategory in selectedExpenseCategories)
            {
                DeleteExpenseCategory(expenseCategory);
            }
        }

        [RelayCommand]
        private void DeleteSelectedIncomeCategories()
        {
            var selectedIncomeCategories = _incomeCategories.Where(x => x.IsSelected).ToList();
            foreach (var incomeCategory in selectedIncomeCategories)
            {
                DeleteIncomeCategory(incomeCategory);
            }
        }
    }
}
