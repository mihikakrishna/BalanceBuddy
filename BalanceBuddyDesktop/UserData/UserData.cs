
using System;
using System.Collections.ObjectModel;
namespace BalanceBuddyDesktop;
public class UserData
{
    public ObservableCollection<IncomeSource> IncomeSources { get; } = new ObservableCollection<IncomeSource>();

    public void AddIncomeSource(string name, decimal balance)
    {
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Income source cannot be null or whitespace.", nameof(name));
            }

            IncomeSource newIncomeSource = new(name, balance);

            if (IncomeSources.Contains(newIncomeSource))
            {
                throw new InvalidOperationException($"The income source '{name}' already exists.");
            }

            IncomeSources.Add(newIncomeSource);
        }
    }
}