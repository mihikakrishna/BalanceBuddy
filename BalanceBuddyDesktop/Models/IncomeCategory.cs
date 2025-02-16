using System;
using System.ComponentModel;
using Avalonia.Media;

namespace BalanceBuddyDesktop.Models;

public class IncomeCategory : INotifyPropertyChanged
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