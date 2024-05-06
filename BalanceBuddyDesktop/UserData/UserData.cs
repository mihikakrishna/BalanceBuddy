using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace BalanceBuddyDesktop
{
    public class UserData
    {
        public ObservableCollection<IncomeSource> IncomeSources { get; } = new ObservableCollection<IncomeSource>();
        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();
        public ObservableCollection<ExpenseCategory> ExpenseCategories { get; } = new ObservableCollection<ExpenseCategory>();

        public void AddIncomeSource(string name, decimal balance)
        {
            var newIncomeSource = new IncomeSource(name, balance);
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
                throw new InvalidOperationException($"The income source '{newName}' is already in use.");
            }
            incomeSource.Name = newName;
            incomeSource.Balance = newBalance;
        }

        public void AddAccount(string name, decimal balance)
        {
            var newAccount = new Account(name, balance);
            if (Accounts.Contains(newAccount))
            {
                throw new InvalidOperationException($"The account '{name}' already exists.");
            }
            Accounts.Add(newAccount);
        }

        public void RemoveAccount(Account account)
        {
            if (!Accounts.Contains(account))
            {
                throw new InvalidOperationException($"The account '{account.Name}' does not exist.");
            }
            Accounts.Remove(account);
        }

        public void UpdateAccount(Guid id, string newName, decimal newBalance)
        {
            var account = Accounts.FirstOrDefault(source => source.Id == id) ?? throw new InvalidOperationException("Account not found.");
            account.Name = newName;
            account.Balance = newBalance;
        }

        public void AddExpenseCategory(string name, decimal budget)
        {
            var newCategory = new ExpenseCategory(name, budget);
            if (ExpenseCategories.Contains(newCategory))
            {
                throw new InvalidOperationException($"The category '{name}' already exists.");
            }
            ExpenseCategories.Add(newCategory);
        }

        public void RemoveExpenseCategory(ExpenseCategory category)
        {
            if (!ExpenseCategories.Contains(category))
            {
                throw new InvalidOperationException($"The category '{category.Name}' does not exist.");
            }
            ExpenseCategories.Remove(category);
        }

        public void UpdateExpenseCategory(Guid id, string newName, decimal newBudget)
        {
            var category = ExpenseCategories.FirstOrDefault(source => source.Id == id) ?? throw new InvalidOperationException("Category not found.");
            if (ExpenseCategories.Any(source => source.Name == newName && source.Id != id))
            {
                throw new InvalidOperationException($"The category '{newName}' is already in use.");
            }
            category.Name = newName;
            category.Budget = newBudget;
        }
    }
}
