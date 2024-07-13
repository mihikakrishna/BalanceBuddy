using System.Collections.Generic;

namespace BalanceBuddyDesktop.Models;

public class UserData
{
    public List<BankAccount> BankAccounts { get; set; }
    public List<Expense> Expenses { get; set; }
    public List<ExpenseCategory> ExpenseCategories { get; set; }

    public UserData()
    {
        BankAccounts = new List<BankAccount>();
        Expenses = new List<Expense>();
        ExpenseCategories = new List<ExpenseCategory>
        {
            new ExpenseCategory { Id = 1, Name = "Housing" },
            new ExpenseCategory { Id = 2, Name = "Food" },
            new ExpenseCategory { Id = 3, Name = "Travel" },
            new ExpenseCategory { Id = 4, Name = "Utilities" },
            new ExpenseCategory { Id = 5, Name = "Healthcare" },
            new ExpenseCategory { Id = 6, Name = "Entertainment" }
        };
    }
}
