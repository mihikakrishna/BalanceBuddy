using BalanceBuddyDesktop.Models;
using System.Collections.Generic;
using System;
using System.Linq;

public static class TransactionService
{
    private static readonly Stack<TransactionOperation> _expenseUndoStack = new();
    private static readonly Stack<TransactionOperation> _expenseRedoStack = new();

    private static readonly Stack<TransactionOperation> _incomeUndoStack = new();
    private static readonly Stack<TransactionOperation> _incomeRedoStack = new();

    private static readonly Stack<TransactionOperation> _bankAccountUndoStack = new();
    private static readonly Stack<TransactionOperation> _bankAccountRedoStack = new();


    private static void PushOperation(Stack<TransactionOperation> undoStack, Stack<TransactionOperation> redoStack, TransactionOperation op)
    {
        undoStack.Push(op);
        redoStack.Clear();
    }

    // Expense operations
    public static void AddExpense(Expense expense)
    {
        if (expense.Category == null)
        {
            expense.Category = GlobalData.Instance.ExpenseCategories.FirstOrDefault(c => c.Name == "Unreviewed");
        }

        GlobalData.Instance.Expenses.Add(expense);
        GlobalData.Instance.HasUnsavedChanges = true;

        PushOperation(_expenseUndoStack, _expenseRedoStack, new TransactionOperation
        {
            Undo = () =>
            {
                GlobalData.Instance.Expenses.Remove(expense);
                GlobalData.Instance.HasUnsavedChanges = true;
            },
            Redo = () =>
            {
                GlobalData.Instance.Expenses.Add(expense);
                GlobalData.Instance.HasUnsavedChanges = true;
            }
        });
    }

    public static void DeleteExpense(Expense expense)
    {
        if (GlobalData.Instance.Expenses.Contains(expense))
        {
            GlobalData.Instance.Expenses.Remove(expense);
            GlobalData.Instance.HasUnsavedChanges = true;

            PushOperation(_expenseUndoStack, _expenseRedoStack, new TransactionOperation
            {
                Undo = () =>
                {
                    GlobalData.Instance.Expenses.Add(expense);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                Redo = () =>
                {
                    GlobalData.Instance.Expenses.Remove(expense);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
        }
    }

    public static void UndoExpense()
    {
        if (_expenseUndoStack.Count > 0)
        {
            var operation = _expenseUndoStack.Pop();
            operation.Undo();
            _expenseRedoStack.Push(operation);
        }
    }

    public static void RedoExpense()
    {
        if (_expenseRedoStack.Count > 0)
        {
            var operation = _expenseRedoStack.Pop();
            operation.Redo();
            _expenseUndoStack.Push(operation);
        }
    }

    // Income operations
    public static void AddIncome(Income income)
    {
        if (income.Category == null)
        {
            income.Category = GlobalData.Instance.IncomeCategories.FirstOrDefault(c => c.Name == "Unreviewed");
        }

        GlobalData.Instance.Incomes.Add(income);
        GlobalData.Instance.HasUnsavedChanges = true;

        PushOperation(_incomeUndoStack, _incomeRedoStack, new TransactionOperation
        {
            Undo = () =>
            {
                GlobalData.Instance.Incomes.Remove(income);
                GlobalData.Instance.HasUnsavedChanges = true;
            },
            Redo = () =>
            {
                GlobalData.Instance.Incomes.Add(income);
                GlobalData.Instance.HasUnsavedChanges = true;
            }
        });
    }

    public static void DeleteIncome(Income income)
    {
        if (GlobalData.Instance.Incomes.Contains(income))
        {
            GlobalData.Instance.Incomes.Remove(income);
            GlobalData.Instance.HasUnsavedChanges = true;

            PushOperation(_incomeUndoStack, _incomeRedoStack, new TransactionOperation
            {
                Undo = () =>
                {
                    GlobalData.Instance.Incomes.Add(income);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                Redo = () =>
                {
                    GlobalData.Instance.Incomes.Remove(income);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
        }
    }

    public static void UndoIncome()
    {
        if (_incomeUndoStack.Count > 0)
        {
            var operation = _incomeUndoStack.Pop();
            operation.Undo();
            _incomeRedoStack.Push(operation);
        }
    }

    public static void RedoIncome()
    {
        if (_incomeRedoStack.Count > 0)
        {
            var operation = _incomeRedoStack.Pop();
            operation.Redo();
            _incomeUndoStack.Push(operation);
        }
    }

    // BankAccount operations
    public static void AddBankAccount(BankAccount bankAccount)
    {
        GlobalData.Instance.BankAccounts.Add(bankAccount);
        GlobalData.Instance.HasUnsavedChanges = true;

        PushOperation(_bankAccountUndoStack, _bankAccountRedoStack, new TransactionOperation
        {
            Undo = () =>
            {
                GlobalData.Instance.BankAccounts.Remove(bankAccount);
                GlobalData.Instance.HasUnsavedChanges = true;
            },
            Redo = () =>
            {
                GlobalData.Instance.BankAccounts.Add(bankAccount);
                GlobalData.Instance.HasUnsavedChanges = true;
            }
        });
    }

    public static void DeleteBankAccount(BankAccount bankAccount)
    {
        if (GlobalData.Instance.BankAccounts.Contains(bankAccount))
        {
            GlobalData.Instance.BankAccounts.Remove(bankAccount);
            GlobalData.Instance.HasUnsavedChanges = true;

            PushOperation(_bankAccountUndoStack, _bankAccountRedoStack, new TransactionOperation
            {
                Undo = () =>
                {
                    GlobalData.Instance.BankAccounts.Add(bankAccount);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                Redo = () =>
                {
                    GlobalData.Instance.BankAccounts.Remove(bankAccount);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
        }
    }

    public static void UndoBankAccount()
    {
        if (_bankAccountUndoStack.Count > 0)
        {
            var operation = _bankAccountUndoStack.Pop();
            operation.Undo();
            _bankAccountRedoStack.Push(operation);
        }
    }

    public static void RedoBankAccount()
    {
        if (_bankAccountRedoStack.Count > 0)
        {
            var operation = _bankAccountRedoStack.Pop();
            operation.Redo();
            _bankAccountUndoStack.Push(operation);
        }
    }

    public static void RecordEdit(object target, string propertyName, object oldValue, object newValue)
    {
        if (target is Expense)
        {
            PushOperation(_expenseUndoStack, _expenseRedoStack, new TransactionOperation
            {
                Undo = () =>
                {
                    var prop = target.GetType().GetProperty(propertyName);
                    prop.SetValue(target, oldValue);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                Redo = () =>
                {
                    var prop = target.GetType().GetProperty(propertyName);
                    prop.SetValue(target, newValue);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
        }
        else if (target is Income)
        {
            PushOperation(_incomeUndoStack, _incomeRedoStack, new TransactionOperation
            {
                Undo = () =>
                {
                    var prop = target.GetType().GetProperty(propertyName);
                    prop.SetValue(target, oldValue);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                Redo = () =>
                {
                    var prop = target.GetType().GetProperty(propertyName);
                    prop.SetValue(target, newValue);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
        }
        else if (target is BankAccount)
        {
            PushOperation(_bankAccountUndoStack, _bankAccountRedoStack, new TransactionOperation
            {
                Undo = () =>
                {
                    var prop = target.GetType().GetProperty(propertyName);
                    prop.SetValue(target, oldValue);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                Redo = () =>
                {
                    var prop = target.GetType().GetProperty(propertyName);
                    prop.SetValue(target, newValue);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
        }
    }

    // Optionally, if you want to clear history separately
    public static void ClearHistory()
    {
        _expenseUndoStack.Clear();
        _expenseRedoStack.Clear();
        _incomeUndoStack.Clear();
        _incomeRedoStack.Clear();
        _bankAccountUndoStack.Clear();
        _bankAccountRedoStack.Clear();
    }
}

public class TransactionOperation
{
    public Action Undo { get; set; }
    public Action Redo { get; set; }
}
