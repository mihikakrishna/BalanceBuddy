
using System;
using System.Collections.ObjectModel;
using System.Linq;
namespace BalanceBuddyDesktop;
public class UserData
{
    public ObservableCollection<IncomeSource> IncomeSources { get; } = new ObservableCollection<IncomeSource>();

    public void AddIncomeSource(string name, decimal balance)
    {
        IncomeSource newIncomeSource = new(name, balance);

        if (IncomeSources.Contains(newIncomeSource))
        {
            throw new InvalidOperationException($"The income source '{name}' already exists.");
        }

        IncomeSources.Add(newIncomeSource);
    }

    public void RemoveIncomeSource(IncomeSource incomeSource)
    {
        if (!IncomeSources.Contains(incomeSource))
        {
            throw new InvalidOperationException($"The income source '{incomeSource.Name}' does not exist.");
        }

        IncomeSources.Remove(incomeSource);
    }

    public void UpdateIncomeSource(Guid id, string newName, decimal newBalance)
    {
        var incomeSource = IncomeSources.FirstOrDefault(source => source.Id == id) ?? throw new InvalidOperationException("Income source not found.");

        if (IncomeSources.Any(source => source.Name == newName && source.Id != id))
        {
            throw new InvalidOperationException($"The income source name '{newName}' is already in use.");
        }

        incomeSource.Name = newName;
        incomeSource.Balance = newBalance;
    }
}