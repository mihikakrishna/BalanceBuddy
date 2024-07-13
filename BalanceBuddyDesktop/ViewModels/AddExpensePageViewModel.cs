using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
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

        public AddExpensePageViewModel()
        {

        }

        [RelayCommand]
        private void AddExpense()
        {
            GlobalData.Instance.Expenses.Add(_newExpense);
            _newExpense = new Expense();
            OnPropertyChanged(nameof(_newExpense));
        }
    }
}
