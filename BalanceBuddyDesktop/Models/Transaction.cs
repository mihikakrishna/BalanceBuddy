using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using BalanceBuddyDesktop.Services;

namespace BalanceBuddyDesktop.Models
{
    public abstract class Transaction : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    var oldValue = _description;
                    _description = value;
                    OnPropertyChanged();
                    TransactionService.RecordEdit(this, nameof(Description), oldValue, value);
                }
            }
        }

        private string _bankIconPath;
        public string BankIconPath
        {
            get => _bankIconPath;
            set
            {
                if (_bankIconPath != value)
                {
                    _bankIconPath = value;
                    OnPropertyChanged(nameof(BankIconPath));
                    LoadBankIcon();
                }
            }
        }

        private Bitmap _bankIcon;
        public Bitmap BankIcon
        {
            get => _bankIcon;
            set
            {
                if (_bankIcon != value)
                {
                    _bankIcon = value;
                    OnPropertyChanged(nameof(BankIcon));
                }
            }
        }

        private void LoadBankIcon()
        {
            if (string.IsNullOrWhiteSpace(_bankIconPath))
            {
                BankIcon = null;
                return;
            }

            try
            {
                var resourceUri = new Uri(_bankIconPath);
                BankIcon = ImageHelper.LoadFromResource(resourceUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load icon from {_bankIconPath}: {ex.Message}");
                BankIcon = null;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Expense : Transaction
    {
        private ExpenseCategory _category;
        public ExpenseCategory Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }
    }

    public class Income : Transaction
    {
        private IncomeCategory _category;
        public IncomeCategory Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }
    }
}