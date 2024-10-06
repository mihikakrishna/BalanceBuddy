using System.Collections.Generic;

namespace BalanceBuddyDesktop.Models;

public class UserData
{
    public List<BankAccount> BankAccounts { get; set; }
    public List<Expense> Expenses { get; set; }
    public List<Income> Incomes { get; set; }
    public List<ExpenseCategory> ExpenseCategories { get; set; }
    public List<IncomeCategory> IncomeCategories { get; set; }

    public bool HasUnsavedChanges { get; set; }

    public UserData()
    {
        BankAccounts = [];Expenses = [];Incomes = [];
        ExpenseCategories =
        [
            new ExpenseCategory {Name = "Miscellaneous" },
            new ExpenseCategory {Name = "Housing" },
            new ExpenseCategory {Name = "Food" },
            new ExpenseCategory {Name = "Travel" },
            new ExpenseCategory {Name = "Utilities" },
            new ExpenseCategory {Name = "Healthcare" },
            new ExpenseCategory {Name = "Entertainment" }
        ];
        IncomeCategories =
        [
            new IncomeCategory {Name = "Other"},
            new IncomeCategory {Name = "Job"}
        ];
        HasUnsavedChanges = false;
    }
}
