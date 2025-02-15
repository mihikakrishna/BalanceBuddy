using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Services
{
    public static class TransactionService
    {
        public static void AddExpense(Expense expense)
        {
            GlobalData.Instance.Expenses.Add(expense);
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        public static void AddIncome(Income income)
        {
            GlobalData.Instance.Incomes.Add(income);
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        public static void AddBankAccount(BankAccount bankAccount)
        {
            GlobalData.Instance.BankAccounts.Add(bankAccount);
            GlobalData.Instance.HasUnsavedChanges = true;
        }

        public static void DeleteExpense(Expense expense)
        {
            if (GlobalData.Instance.Expenses.Contains(expense))
            {
                GlobalData.Instance.Expenses.Remove(expense);
                GlobalData.Instance.HasUnsavedChanges = true;
            }
        }

        public static void DeleteIncome(Income income)
        {
            if (GlobalData.Instance.Incomes.Contains(income))
            {
                GlobalData.Instance.Incomes.Remove(income);
                GlobalData.Instance.HasUnsavedChanges = true;
            }
        }

        public static void DeleteBankAccount(BankAccount bankAccount)
        {
            if (GlobalData.Instance.BankAccounts.Contains(bankAccount))
            {
                GlobalData.Instance.BankAccounts.Remove(bankAccount);
                GlobalData.Instance.HasUnsavedChanges = true;
            }
        }
    }
}
