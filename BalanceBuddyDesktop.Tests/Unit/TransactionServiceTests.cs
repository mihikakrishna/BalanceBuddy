using NUnit.Framework;
using System;
using System.Linq;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;

namespace BalanceBuddyDesktop.Tests
{
    [TestFixture]
    public class TransactionServiceTests
    {
        [SetUp]
        public void Setup()
        {
            // Clear undo/redo history and global data before each test.
            TransactionService.ClearHistory();
            GlobalData.Instance.Expenses.Clear();
            GlobalData.Instance.Incomes.Clear();
            GlobalData.Instance.BankAccounts.Clear();
            GlobalData.Instance.HasUnsavedChanges = false;
        }

        [Test]
        public void AddExpense_ShouldAddExpense_AndSupportUndoRedo()
        {
            // Arrange
            var expense = new Expense { Amount = 100, Date = DateTime.Now, Description = "Test Expense" };

            // Act
            TransactionService.AddExpense(expense);

            // Assert: Expense is added.
            Assert.That(GlobalData.Instance.Expenses.Contains(expense), Is.True);
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True);
            Assert.That(TransactionService.CanUndo, Is.True);
            Assert.That(TransactionService.CanRedo, Is.False);

            // Act: Undo the addition.
            TransactionService.Undo();

            // Assert: Expense is removed.
            Assert.That(GlobalData.Instance.Expenses.Contains(expense), Is.False);
            Assert.That(TransactionService.CanRedo, Is.True);

            // Act: Redo the addition.
            TransactionService.Redo();

            // Assert: Expense is added again.
            Assert.That(GlobalData.Instance.Expenses.Contains(expense), Is.True);
        }

        [Test]
        public void DeleteExpense_ShouldRemoveExpense_AndSupportUndoRedo()
        {
            // Arrange
            var expense = new Expense { Amount = 50, Date = DateTime.Now, Description = "Expense to Delete" };
            GlobalData.Instance.Expenses.Add(expense);

            // Act
            TransactionService.DeleteExpense(expense);

            // Assert: Expense is removed.
            Assert.That(GlobalData.Instance.Expenses.Contains(expense), Is.False);
            Assert.That(TransactionService.CanUndo, Is.True);
            Assert.That(TransactionService.CanRedo, Is.False);

            // Act: Undo the deletion.
            TransactionService.Undo();

            // Assert: Expense is restored.
            Assert.That(GlobalData.Instance.Expenses.Contains(expense), Is.True);
            Assert.That(TransactionService.CanRedo, Is.True);

            // Act: Redo the deletion.
            TransactionService.Redo();

            // Assert: Expense is removed again.
            Assert.That(GlobalData.Instance.Expenses.Contains(expense), Is.False);
        }

        [Test]
        public void AddIncome_ShouldAddIncome_AndSupportUndoRedo()
        {
            // Arrange
            var income = new Income { Amount = 200, Date = DateTime.Now, Description = "Test Income" };

            // Act
            TransactionService.AddIncome(income);

            // Assert: Income is added.
            Assert.That(GlobalData.Instance.Incomes.Contains(income), Is.True);
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True);
            Assert.That(TransactionService.CanUndo, Is.True);
            Assert.That(TransactionService.CanRedo, Is.False);

            // Act: Undo the addition.
            TransactionService.Undo();

            // Assert: Income is removed.
            Assert.That(GlobalData.Instance.Incomes.Contains(income), Is.False);
            Assert.That(TransactionService.CanRedo, Is.True);

            // Act: Redo the addition.
            TransactionService.Redo();

            // Assert: Income is added again.
            Assert.That(GlobalData.Instance.Incomes.Contains(income), Is.True);
        }

        [Test]
        public void DeleteIncome_ShouldRemoveIncome_AndSupportUndoRedo()
        {
            // Arrange
            var income = new Income { Amount = 300, Date = DateTime.Now, Description = "Income to Delete" };
            GlobalData.Instance.Incomes.Add(income);

            // Act
            TransactionService.DeleteIncome(income);

            // Assert: Income is removed.
            Assert.That(GlobalData.Instance.Incomes.Contains(income), Is.False);
            Assert.That(TransactionService.CanUndo, Is.True);
            Assert.That(TransactionService.CanRedo, Is.False);

            // Act: Undo the deletion.
            TransactionService.Undo();

            // Assert: Income is restored.
            Assert.That(GlobalData.Instance.Incomes.Contains(income), Is.True);
            Assert.That(TransactionService.CanRedo, Is.True);

            // Act: Redo the deletion.
            TransactionService.Redo();

            // Assert: Income is removed again.
            Assert.That(GlobalData.Instance.Incomes.Contains(income), Is.False);
        }

        [Test]
        public void AddBankAccount_ShouldAddBankAccount_AndSupportUndoRedo()
        {
            // Arrange
            var bankAccount = new BankAccount { Name = "Test Bank", Balance = 1000 };

            // Act
            TransactionService.AddBankAccount(bankAccount);

            // Assert: BankAccount is added.
            Assert.That(GlobalData.Instance.BankAccounts.Contains(bankAccount), Is.True);
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True);
            Assert.That(TransactionService.CanUndo, Is.True);
            Assert.That(TransactionService.CanRedo, Is.False);

            // Act: Undo the addition.
            TransactionService.Undo();

            // Assert: BankAccount is removed.
            Assert.That(GlobalData.Instance.BankAccounts.Contains(bankAccount), Is.False);
            Assert.That(TransactionService.CanRedo, Is.True);

            // Act: Redo the addition.
            TransactionService.Redo();

            // Assert: BankAccount is added again.
            Assert.That(GlobalData.Instance.BankAccounts.Contains(bankAccount), Is.True);
        }

        [Test]
        public void DeleteBankAccount_ShouldRemoveBankAccount_AndSupportUndoRedo()
        {
            // Arrange
            var bankAccount = new BankAccount { Name = "Bank to Delete", Balance = 500 };
            GlobalData.Instance.BankAccounts.Add(bankAccount);

            // Act
            TransactionService.DeleteBankAccount(bankAccount);

            // Assert: BankAccount is removed.
            Assert.That(GlobalData.Instance.BankAccounts.Contains(bankAccount), Is.False);
            Assert.That(TransactionService.CanUndo, Is.True);
            Assert.That(TransactionService.CanRedo, Is.False);

            // Act: Undo the deletion.
            TransactionService.Undo();

            // Assert: BankAccount is restored.
            Assert.That(GlobalData.Instance.BankAccounts.Contains(bankAccount), Is.True);
            Assert.That(TransactionService.CanRedo, Is.True);

            // Act: Redo the deletion.
            TransactionService.Redo();

            // Assert: BankAccount is removed again.
            Assert.That(GlobalData.Instance.BankAccounts.Contains(bankAccount), Is.False);
        }
    }
}
