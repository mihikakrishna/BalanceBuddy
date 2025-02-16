using System;
using System.ComponentModel;
using Avalonia.Media;

namespace BalanceBuddyDesktop.Models
{
    public class ExpenseCategory : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(CellColor));
                }
            }
        }

        private decimal? _budget;
        public decimal? Budget
        {
            get => _budget;
            set
            {
                if (_budget != value)
                {
                    _budget = value;
                    OnPropertyChanged(nameof(Budget));
                }
            }
        }

        public IBrush CellColor
        {
            get
            {
                return string.Equals(Name, "Unreviewed", StringComparison.OrdinalIgnoreCase)
                    ? Brushes.RosyBrown
                    : Brushes.Transparent;
            }
        }
    }
}