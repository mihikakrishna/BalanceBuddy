using System;
using System.Collections.Generic;
using BalanceBuddyDesktop.Models;

namespace BalanceBuddyDesktop.Services
{
    public static class TransactionService
    {
        private static readonly Stack<TransactionOperation> _undoStack = new();
        private static readonly Stack<TransactionOperation> _redoStack = new();

        public static bool CanUndo => _undoStack.Count > 0;
        public static bool CanRedo => _redoStack.Count > 0;

        // ADD OPERATIONS

        public static void AddExpense(Expense expense)
        {
            GlobalData.Instance.Expenses.Add(expense);
            GlobalData.Instance.HasUnsavedChanges = true;

            _undoStack.Push(new TransactionOperation
            {
                // To undo an add, remove the expense.
                Undo = () =>
                {
                    GlobalData.Instance.Expenses.Remove(expense);
                    GlobalData.Instance.HasUnsavedChanges = true;
                },
                // To redo, add it back.
                Redo = () =>
                {
                    GlobalData.Instance.Expenses.Add(expense);
                    GlobalData.Instance.HasUnsavedChanges = true;
                }
            });
            _redoStack.Clear();
        }

        public static void AddIncome(Income income)
        {
            GlobalData.Instance.Incomes.Add(income);
            GlobalData.Instance.HasUnsavedChanges = true;

            _undoStack.Push(new TransactionOperation
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
            _redoStack.Clear();
        }

        public static void AddBankAccount(BankAccount bankAccount)
        {
            GlobalData.Instance.BankAccounts.Add(bankAccount);
            GlobalData.Instance.HasUnsavedChanges = true;

            _undoStack.Push(new TransactionOperation
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
            _redoStack.Clear();
        }

        // DELETE OPERATIONS

        public static void DeleteExpense(Expense expense)
        {
            if (GlobalData.Instance.Expenses.Contains(expense))
            {
                GlobalData.Instance.Expenses.Remove(expense);
                GlobalData.Instance.HasUnsavedChanges = true;

                _undoStack.Push(new TransactionOperation
                {
                    // Undoing a delete means re-adding the expense.
                    Undo = () =>
                    {
                        GlobalData.Instance.Expenses.Add(expense);
                        GlobalData.Instance.HasUnsavedChanges = true;
                    },
                    // Redo the delete.
                    Redo = () =>
                    {
                        GlobalData.Instance.Expenses.Remove(expense);
                        GlobalData.Instance.HasUnsavedChanges = true;
                    }
                });
                _redoStack.Clear();
            }
        }

        public static void DeleteIncome(Income income)
        {
            if (GlobalData.Instance.Incomes.Contains(income))
            {
                GlobalData.Instance.Incomes.Remove(income);
                GlobalData.Instance.HasUnsavedChanges = true;

                _undoStack.Push(new TransactionOperation
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
                _redoStack.Clear();
            }
        }

        public static void DeleteBankAccount(BankAccount bankAccount)
        {
            if (GlobalData.Instance.BankAccounts.Contains(bankAccount))
            {
                GlobalData.Instance.BankAccounts.Remove(bankAccount);
                GlobalData.Instance.HasUnsavedChanges = true;

                _undoStack.Push(new TransactionOperation
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
                _redoStack.Clear();
            }
        }

        public static void RecordEdit(object target, string propertyName, object oldValue, object newValue)
        {
            _undoStack.Push(new TransactionOperation
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
            _redoStack.Clear();
        }


        // UNDO/REDO OPERATIONS

        public static void Undo()
        {
            if (CanUndo)
            {
                var operation = _undoStack.Pop();
                operation.Undo();
                _redoStack.Push(operation);
            }
        }

        public static void Redo()
        {
            if (CanRedo)
            {
                var operation = _redoStack.Pop();
                operation.Redo();
                _undoStack.Push(operation);
            }
        }

        // Optionally, add a method to clear history.
        public static void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }

    public class TransactionOperation
    {
        public Action Undo { get; set; }
        public Action Redo { get; set; }
    }

}
