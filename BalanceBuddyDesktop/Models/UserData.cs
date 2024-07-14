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
            new ExpenseCategory {Name = "Housing" },
            new ExpenseCategory {Name = "Food" },
            new ExpenseCategory {Name = "Travel" },
            new ExpenseCategory {Name = "Utilities" },
            new ExpenseCategory {Name = "Healthcare" },
            new ExpenseCategory {Name = "Entertainment" }
        };
    }
}
